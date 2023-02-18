
using System;
using System.Collections.Generic;


namespace RandomPlotGenerator;


public class KDTree {

    private KDNode root;
    private int Count;


    // Build (root)

    public void Build(List<Point> points){

        root = BuildNode(points, 0);
        Count = points.Count;

    }


    // BuildNode

    private KDNode BuildNode(List<Point> points, int depth){

        if(points.Count == 0){

            return null;

        }


        int dimension = depth % 2;  // Limiting this code to 2D
        points.Sort(new PointComparer(dimension));


        // find the median node

        int medianIdx = (int) points.Count / 2;
        KDNode node = new KDNode(points[medianIdx], depth);


        // Continue building nodes

        node.left = BuildNode(points.GetRange(0, medianIdx), depth + 1);
        node.right = BuildNode(points.GetRange(medianIdx, points.Count - medianIdx), depth + 1);

        return node;

    }


    // Search

    private List<int> Search(Point query, double radius){

        List<int> result = new List<int>();

        SearchNode(root, query, Math.Pow(radius, 2), result);
        
        return result;

    }


    // SearchNode

    private void SearchNode(KDNode node, Point query, double radius, List<int> result){

        if(node == null){
            return;
        }

        double distance2 = node.point.Distance2(query);

        if(distance2 <= Math.Pow(radius, 2)){

            result.Add(node.index);

        }


        int dimension = node.GetDimension();
        double linedist = node.point[dimension] - query[dimension];

        if(linedist <= radius && node.left != null){

            SearchNode(node.left, query, radius, result);

        } else if (linedist <= -radius && node.right != null){

            SearchNode(node.right, query, radius, result);

        }


        return;

    }




    // Class for comparing points

    private class PointComparer : IComparer<Point> {

        public readonly int dimension;

        public PointComparer(int dimension){
            this.dimension = dimension;
        }

        public int Compare(Point point1, Point point2){

            return point1[dimension].CompareTo(point2[dimension]);

        }

    }


    // KDNode class

    private class KDNode {

        public int index;
        public Point point;
        public int depth;
        public KDNode left;
        public KDNode right;
        public PointComparer comp;

        public KDNode(Point point, int depth){

            this.point = point;
            this.depth = depth;
            this.comp = new PointComparer(depth % 2);

        }

        public int Compare(Point query){

            return comp.Compare(query, point);

        }

        public int GetDimension(){

            return comp.dimension;

        }

    }


    // Indexer

    public int this[int i]{

        

    }


}