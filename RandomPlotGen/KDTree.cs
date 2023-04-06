
#define DEBUG
#undef DEBUG


using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator
{


    /* Class: KDTree

    This class stores a list of points in a binary tree structure, where each node partitions the plane 
    in alternating dimensions. This structure is efficient at nearest-neighbour searches, which is why it
    is used when sampling with the Local Pivotal Method.

    The author is being more fancy than he needs to be. This library only really needs a KDTree 
    implementation that works with the SampleUnit class. However, the only method that is unique to a
    SampleUnit is the SearchNNNode method so by creating this class as a generic, with a type that must
    inherit from Coordinate, we get a KDTree implementation for Coordinate. This comes at a cost, the 
    actual class implementing a KDTree for a SampleUnit has to inherit from KDTree<SampleUnit> and 
    override the SearchNNNode method, so that the features of SampleUnit that are not present in 
    Coordinate can be utilised; i.e. excluding SampleUnits with a status of 'Excluded' from the search.

     */
    public class KDTree<T> where T : Coordinate
    {

        private KDNode root;
        private readonly int DefaultLeafSize = 40;



        /* Construct a KD-Tree from a set of points */
        public KDTree(List<T> points, int? leafsize)
        {

            if (leafsize is null)
            {
                leafsize = DefaultLeafSize;
            }

            root = BuildNode(points, 0, (int)leafsize);

        }


        /* Construct a node in the KD-Tree

        This implementation sorts points along the dimension being split and 
        constructs a node from the median point. When the number of points
        falls below the leaf size, the remaining points are simply stored
        in a list.
        */
        private KDNode BuildNode(List<T> points, int depth, int leafsize)
        {

            if (points.Count == 0)
            {
                throw new System.ArgumentException("No points provided to BuildNode!");
            }


            int dimension = depth % 2;  // Limiting this code to 2D
            points.Sort(new TComparer(dimension));

            int medianIdx = (int)points.Count / 2;
            KDNode node = new KDNode(points[medianIdx], depth);


            /* If the number of points remaining is less than the maximum leaf size, 
            then assign the remaining points to the leaves. */

            if (points.Count <= leafsize)
            {

                node.leaves.AddRange(points.GetRange(0, medianIdx));
                node.leaves.AddRange(points.GetRange(medianIdx + 1, points.Count - medianIdx - 1));

            }
            else
            {

                node.left = BuildNode(points.GetRange(0, medianIdx), depth + 1, leafsize);
                node.right = BuildNode(points.GetRange(medianIdx + 1, points.Count - medianIdx - 1), depth + 1, leafsize);

            }


            return node;

        }


        /* Search for the nearest non-trivial neighbour

        Of the two components of the Local Pivotal Method, this is certainly one of them. Comes
        with the (highly suggested) option to restrict the search to points that have not already
        been excluded from the search; pretty much all the 'T.status' member is good for.
        */
        public void SearchNN(in T query, bool onlyundefined, out T result)
        {

            if (root is null)
            {
                throw new ArgumentException("KDTree root node is null!");
            }

            double bestdist2 = Double.MaxValue;
            result = query;

            SearchNNNode(root, in query, onlyundefined, ref bestdist2, ref result);

            return;

        }


        /* Is this node the one you're looking for? */
        public virtual void SearchNNNode(KDNode node, in T query, bool onlyundefined, ref double bestdist2, ref T result)
        {

            double distance2 = Distance2(node.point, query);

            /* Exclude the trivial result from the search by checking whether query and 
            node.point refer to the same memory address. */

            if (!Object.ReferenceEquals(query, node.point) &&
                (distance2 <= bestdist2))
            {

                if (distance2 < bestdist2)
                {

                    bestdist2 = distance2;
                    result = node.point;

                }
                else if ((distance2 == bestdist2) && BreakTie())
                {

                    bestdist2 = distance2;
                    result = node.point;

                }

            }


            if (node.IsLeafNode())
            {

                for (int i = 0; i < node.leaves.Count; i++)
                {

                    if (Object.ReferenceEquals(query, node.leaves[i]))
                    {
                        continue;
                    }

                    double leafdist2 = Distance2(query, node.leaves[i]);

                    if (leafdist2 < bestdist2)
                    {

                        bestdist2 = leafdist2;
                        result = node.leaves[i];

                    }
                    else if ((leafdist2 == bestdist2) && BreakTie())
                    {

                        bestdist2 = leafdist2;
                        result = node.leaves[i];

                    }

                }

            }
            else
            {

                int dimension = node.GetDimension();
                double linedist2 = Math.Pow(node.point[dimension] - query[dimension], 2.0);


                /* Unlike in FindNode, need to search both branches of the tree when the split point is in range */

                if (linedist2 <= bestdist2)
                {

                    if (node.left != null)
                    {

                        SearchNNNode(node.left, in query, onlyundefined, ref bestdist2, ref result);

                    }

                    if (node.right != null)
                    {

                        SearchNNNode(node.right, in query, onlyundefined, ref bestdist2, ref result);

                    }


                }

            }


            return;

        }


        /* Sometimes two nodes are the same distance apart, so flip a coin.
        This happens a lot when the sampling units form a grid. */
        public bool BreakTie()
        {

            Random rand = new Random();

            return rand.NextDouble() > 0.5;

        }


        /* Need to tell List.Sort how to compare Ts */
        public class TComparer : IComparer<T>
        {

            public readonly int dimension;

            public TComparer(int dimension)
            {
                this.dimension = dimension;
            }

            public int Compare(T point1, T point2)
            {

                if ((point1 is null) || (point2 is null))
                {

                    throw new ArgumentException("Ts cannot be null!");

                }

                int order = point1[dimension].CompareTo(point2[dimension]);

                if (order == 0)
                {

                    int newdim = (dimension + 1) % 2;

                    order = point1[newdim].CompareTo(point2[newdim]);

                }

                return order;

            }

        }


        public static double Distance2(T point1, T point2)
        {

            return Math.Pow(point1[0] - point2[0], 2.0) + Math.Pow(point1[1] - point2[1], 2.0);

        }


        /* Class: KDNode

        Pretty much as it sounds, a node in the KD-Tree. Stores a point and child nodes, unless it has leaves.
        */
        public class KDNode
        {

            public T point;
            public int depth;
            public KDNode left;
            public KDNode right;
            public List<T> leaves;
            public TComparer comp;


            public KDNode(T point, int depth)
            {

                this.point = point;
                this.depth = depth;
                this.comp = new TComparer(depth % 2);
                this.leaves = new List<T>();

            }


            public int Compare(T query)
            {

                return comp.Compare(query, point);

            }

            public int GetDimension()
            {

                return comp.dimension;

            }


            public bool IsLeafNode()
            {

                return leaves.Count > 0;

            }

        }

    }
}