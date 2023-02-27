
#define DEBUG
#undef DEBUG


using System;
using System.Collections.Generic;


namespace RandomPlotGenerator;




public class KDTree {

    private KDNode? root;
    private int Count;
    private readonly int DefaultLeafSize = 40;
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


        FindNode(root, in query, ref bestdist2, ref result);


        double proximity2 = query.Distance2(result);

        if(proximity2 > Math.Pow((double) threshold, 2.0)){

            throw new ArgumentException(
                String.Format("Query point ({0}, {1}) is not in KDTree. \nNearest point was ({2}, {3})", 
                    query.GetX(), query.GetY(), result.GetX(), result.GetY())
            );

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
        
        
        if(node.IsLeafNode()){

            for(int i = 0; i < node.leaves.Count; i++){

                double leafdist2 = query.Distance2(node.leaves[i]);

                if(leafdist2 <= bestdist2){

                    result = node.leaves[i];
                    bestdist2 = leafdist2;

                    if(leafdist2 == 0){
                        return;
                    }

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
            (distance2 <= bestdist2)){

                if(distance2 < bestdist2){

                    bestdist2 = distance2;
                    result = node.point;

                } else if ((distance2 == bestdist2) && BreakTie()){

                    bestdist2 = distance2;
                    result = node.point;

                }

        }
        
        
        if(node.IsLeafNode()){

            for(int i = 0; i < node.leaves.Count; i++){

                if(Object.ReferenceEquals(query, node.leaves[i]) || 
                    (onlyundefined && !node.leaves[i].IsUndefined())){
                    continue;
                }

                double leafdist2 = query.Distance2(node.leaves[i]);

                if(leafdist2 < bestdist2){

                    bestdist2 = leafdist2;
                    result = node.leaves[i];

                } else if ((leafdist2 == bestdist2) && BreakTie()){

                    bestdist2 = leafdist2;
                    result = node.leaves[i];

                }

            }

        } else {

            int dimension = node.GetDimension();
            double linedist2 = Math.Pow(node.point[dimension] - query[dimension], 2.0);


            /* Unlike in FindNode, need to search both branches of the tree when the split point is in range */

            if(linedist2 <= bestdist2){ 

                if(node.left is not null){

                    SearchNNNode(node.left, in query, onlyundefined, ref bestdist2, ref result);

                } 
                
                if (node.right is not null){

                    SearchNNNode(node.right, in query, onlyundefined, ref bestdist2, ref result);

                }


            }

        }


        return;

    }


    public bool BreakTie(){

        Random rand = new Random();

        return rand.NextDouble() > 0.5;

    }


    public string Print(bool? nodesonly){

        if(nodesonly is null){
            nodesonly = false;
        }

        string rootprnt = "";

        if(root is not null){
            root.Print(ref rootprnt, (bool) nodesonly);
        }

        return rootprnt;

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

            int order = point1[dimension].CompareTo(point2[dimension]);

            if(order == 0){

                int newdim = (dimension + 1) % 2;

                order = point1[newdim].CompareTo(point2[newdim]);

            }

            return order;

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


        public void Print(ref string buff, bool nodesonly){

            buff += String.Format("Depth: {0}  Dimension: {1}  {2}: {3}  {4}\n", depth, GetDimension(), point.Print(), point.prob, point.status);

            if(IsLeafNode() && !nodesonly){
                for(int i = 0; i < leaves.Count; i++){
                    buff += String.Format("Depth: {0}  {1}: {2}  {3}\n", depth, leaves[i].Print(), leaves[i].prob, leaves[i].status);
                }
            }

            if(left is not null){
                left.Print(ref buff, nodesonly);
            }

            if(right is not null){
                right.Print(ref buff, nodesonly);
            }

            return;

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