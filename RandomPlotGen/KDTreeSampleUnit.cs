
#define DEBUG
#undef DEBUG


using System;
using System.Collections.Generic;

namespace RandomPlotGenerator
{


    /* Class: KDTree

    This class stores a list of points in a binary tree structure, where each node partitions the plane 
    in alternating dimensions. This structure is efficient at nearest-neighbour searches, which is why it
    is used when sampling with the Local Pivotal Method.
     */

    public class KDTreeSampleUnit : KDTree<SampleUnit>
    {


        public KDTreeSampleUnit(List<SampleUnit> points, int? leafsize) : base(points, leafsize)
        {


        }


        public override void SearchNNNode(KDNode node, in SampleUnit query, bool onlyundefined, ref double bestdist2, ref SampleUnit result)
        {

            double distance2 = node.point.Distance2(query);

            /* Exclude the trivial result from the search by checking whether query and 
            node.point refer to the same memory address. */

            if (!System.Object.ReferenceEquals(query, node.point) &&
                (!onlyundefined || node.point.IsUndefined()) &&
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

                    if (Object.ReferenceEquals(query, node.leaves[i]) ||
                        (onlyundefined && !node.leaves[i].IsUndefined()))
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

        }
    }
}