
#define DEBUG
#undef DEBUG


using System;
using System.Collections.Generic;


namespace RandomPlotGenerator;




public class KDTree {

    private KDNode? root;
    private int Count;
    private readonly int DefaultLeafSize = 5;
    private readonly double DefaultThreshold = 0.1;


    // Build (root)

    public void Build(List<Point> points, int? leafsize){

        if(leafsize is null){
           leafsize = DefaultLeafSize; 
        }

        root = BuildNode(points, 0, (int) leafsize);
        Count = points.Count;

    }


    // BuildNode

    private KDNode BuildNode(List<Point> points, int depth, int leafsize){

        if(points.Count == 0){

            throw new ArgumentException("No points provided to BuildNode!");

        }


        int dimension = depth % 2;  // Limiting this code to 2D
        points.Sort(new PointComparer(dimension));

        int medianIdx = (int) points.Count / 2;
        KDNode node = new KDNode(points[medianIdx], depth);


        /* If the number of points remaining is less than the maximum leaf size, 
        then assign the remaining points to the leaves. */

        if(points.Count <= leafsize){

            node.leaves.AddRange(points.GetRange(0, medianIdx));
            node.leaves.AddRange(points.GetRange(medianIdx + 1, points.Count - medianIdx - 1));

        } else {

            node.left = BuildNode(points.GetRange(0, medianIdx), depth + 1, leafsize);
            node.right = BuildNode(points.GetRange(medianIdx + 1, points.Count - medianIdx - 1), depth + 1, leafsize);

        }


        return node;

    }


    /* FindNode - Finds an existing node in a KDTree. 
        Note this is basically the same as SearchNN, 
        only without the equality of references check. */

    public void Find(in Point query, double? threshold, out Point found){

        if( root is null ){
            throw new ArgumentException("KDTree root node is null!");
        }

        if( threshold is null){
            threshold = DefaultThreshold;
        }


        double bestdist2 = Double.MaxValue;
        Point result = query;


        #if DEBUG

        Console.WriteLine(String.Format("Find: Searching for {0} in KD-Tree. Initial bestdist2 = {1}", query.Print(), bestdist2));

        #endif


        FindNode(root, in query, ref bestdist2, ref result);


        double proximity2 = query.Distance2(result);

        if(proximity2 > Math.Pow((double) threshold, 2.0)){

            throw new ArgumentException(String.Format("Query point ({0}, {1}) is not in KDTree. \nNearest point was ({2}, {3})", query.GetX(), query.GetY(), result.GetX(), result.GetY()));

        }

        found = result;
        
        return;

    }



    private void FindNode(KDNode node, in Point query, ref double bestdist2, ref Point result){

        double distance2 = node.point.Distance2(query);

        if(distance2 < bestdist2){

            bestdist2 = distance2;
            result = node.point;

        }

        if(distance2 == 0){
            return;
        }


        #if DEBUG

        Console.WriteLine(
            String.Format("FindNode: node: {0}:  distance2: {1}  IsLeafNode: {2}  result: {3}  bestdist2: {4}", 
                            node.point.Print(), distance2, node.IsLeafNode(), result.Print(), bestdist2)
        );

        #endif
        
        
        if(node.IsLeafNode()){

            for(int i = 0; i < node.leaves.Count; i++){

                double leafdist2 = query.Distance2(node.leaves[i]);

                #if DEBUG
                    Console.WriteLine(String.Format("FindNode: Searching leaf: {0}  leafdist2: {1}", node.leaves[i].Print(), leafdist2));
                #endif

                if(leafdist2 <= bestdist2){

                    result = node.leaves[i];
                    bestdist2 = leafdist2;

                    if(leafdist2 == 0){
                        return;
                    }

                }

            }

            #if DEBUG

            Console.WriteLine(
                String.Format("FindNode: Finished searching leaves:  result: {0}  bestdist2: {1}", 
                                result.Print(), bestdist2)
            );

            #endif

        } else {

            int dimension = node.GetDimension();
            bool goleft = node.point[dimension] >= query[dimension];
            double linedist2 = Math.Pow(node.point[dimension] - query[dimension], 2.0);

            if(linedist2 == 0){     // point is on the split-line, so check the next dimension

                dimension = (dimension + 1) % 2;

                goleft = node.point[dimension] >= query[dimension];

            }

            #if DEBUG

            Console.WriteLine(
                String.Format("FindNode: Searching next node:  dimension: {0}  goleft: {1}  linedist2: {2}", 
                                dimension, goleft, linedist2)
            );

            #endif


            if(goleft && node.left != null){

                FindNode(node.left, in query, ref bestdist2, ref result);

            } else if (node.right != null){

                FindNode(node.right, in query, ref bestdist2, ref result);

            }

        }


        return;

    }



    public void SearchNN(in Point query, bool onlyundefined, out Point result){

        if( root is null ){
            throw new ArgumentException("KDTree root node is null!");
        }

        double bestdist2 = Double.MaxValue;
        result = new Point(0.0, 0.0);

        SearchNNNode(root, in query, onlyundefined, ref bestdist2, ref result);

        return;

    }


    private void SearchNNNode(KDNode node, in Point query, bool onlyundefined, ref double bestdist2, ref Point result){
        

        double distance2 = node.point.Distance2(query);

        if(!Object.ReferenceEquals(query, node.point) && 
            (!onlyundefined || node.point.IsUndefined()) && 
            (distance2 < bestdist2)){   // What to do about ties?

            bestdist2 = distance2;
            result = node.point;

        }
        
        
        if(node.IsLeafNode()){

            for(int i = 0; i < node.leaves.Count; i++){

                if(Object.ReferenceEquals(query, node.leaves[i]) || 
                    (onlyundefined && !node.leaves[i].IsUndefined())){
                    continue;
                }

                double leafdist2 = node.point.Distance2(node.leaves[i]);

                if(leafdist2 <= bestdist2){     // What to do about ties?

                    result = node.leaves[i];
                    bestdist2 = leafdist2;

                }

            }

        } else {

            int dimension = node.GetDimension();
            bool goleft = node.point[dimension] >= query[dimension];
            double linedist2 = Math.Pow(node.point[dimension] - query[dimension], 2.0);

            if(linedist2 == 0){     // point is on the split-line, so check the next dimension

                dimension = (dimension + 1) % 2;

                goleft = node.point[dimension] >= query[dimension];

            }

            if(linedist2 <= bestdist2){

                if(goleft && node.left != null){

                    SearchNNNode(node.left, in query, onlyundefined, ref bestdist2, ref result);

                } else if (node.right != null){

                    SearchNNNode(node.right, in query, onlyundefined, ref bestdist2, ref result);

                }


            }

        }


        return;

    }


    // Class for comparing points

    private class PointComparer : IComparer<Point> {

        public readonly int dimension;

        public PointComparer(int dimension){
            this.dimension = dimension;
        }

        public int Compare(Point? point1, Point? point2){

            if ((point1 is null) || (point2 is null)){

                throw new ArgumentException("Points cannot be null!");

            }

            return point1[dimension].CompareTo(point2[dimension]);

        }

    }


    // KDNode class

    private class KDNode {

        public Point point;
        public int depth;
        public KDNode? left;
        public KDNode? right;
        public List<Point> leaves;
        public PointComparer comp;

        public KDNode(Point point, int depth){

            this.point = point;
            this.depth = depth;
            this.comp = new PointComparer(depth % 2);
            this.leaves = new List<Point>();

        }

        public int Compare(Point query){

            return comp.Compare(query, point);

        }

        public int GetDimension(){

            return comp.dimension;

        }

        public bool IsLeafNode(){

            return leaves.Count > 0;

        }

    }


}