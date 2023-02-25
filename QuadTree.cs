
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class QuadTree {

    private readonly double MinArea;
    private readonly QuadTreeNode root;




    public QuadTree(List<Point> points, double minarea){

        this.MinArea = minarea;


        if(points.Count == 0){
            throw new ArgumentException("No points provided to QuadTree!");
        }

        double xmin = points[0].GetX(), ymin = points[0].GetY(); 
        double xmax = points[0].GetX(), ymax = points[0].GetY();


        // Sadly, need to search all points for the bounding box

        for( int i = 0; i < points.Count; i++ ){

            double x = points[i].GetX();
            double y = points[i].GetY();

            xmin = (xmin > x) ? x : xmin;
            ymin = (ymin > y) ? y : ymin;
            xmax = (xmax < x) ? x : xmax;
            xmin = (ymax < y) ? y : ymax;

        }


        root = new QuadTreeNode(new AOIBox(xmin, ymin, xmax, ymax));


        for( int i = 0; i < points.Count; i++ ){

            root.Insert(points[i], i, MinArea);

        }

    }






    public class QuadTreeNode {

        public readonly AOIBox aoi;
        private QuadTreeNode? quad0;
        private QuadTreeNode? quad1;
        private QuadTreeNode? quad2;
        private QuadTreeNode? quad3;
        private List<int> indices;



        public QuadTreeNode(AOIBox aoi){

            this.aoi = aoi;
            this.indices = new List<int>();

        }


        /* public void GetPoints(out List<int> points){          // There's potentially a lot of copying going on here!


            if(IsLeafNode()){

                points.AddRange(indices);
                return;

            } 


            if (quad0 is not null){

                quad0.GetPoints(out points);

            }

            if (quad1 is not null){

                quad1.GetPoints(out points);

            }

            if (quad2 is not null){

                quad2.GetPoints(out points);

            }

            if (quad3 is not null){

                quad3.GetPoints(out points);

            }

            return;

        } */



        public bool IsLeafNode(){
            return indices.Count > 0;
        }


        public bool Contains(Point query){
            return this.aoi.Contains(query);
        }
        



        public void ChildContaining(Point query, out QuadTreeNode? node){

            if(!this.Contains(query)){
                throw new ArgumentException(String.Format("{0} not in {1}", query.Print(), this.aoi.Print()));
            }

            if((quad0 is not null && quad0.Contains(query))){

                node = quad0;
                return;

            } else if ((quad1 is not null) && quad1.Contains(query)){

                node = quad1;
                return;

            }  else if ((quad2 is not null) && quad2.Contains(query)){

                node = quad2;
                return;

            }  else if ((quad3 is not null) && quad3.Contains(query)){

                node = quad3;
                return;

            } else {

                node = null;
                return;

            }

        }


        public void Insert(in Point query, int index, double minarea){

            if(!this.Contains(query)){
                throw new ArgumentException(String.Format("{0} not in {1}", query.Print(), aoi.Print()));
            }

            QuadTreeNode? node;
            ChildContaining(query, out node);

            if( (node is null) && (this.aoi.Area() > minarea) ){

                List<AOIBox> quadList = this.aoi.Quadrants();

                quad0 = new QuadTreeNode(quadList[0]);
                quad1 = new QuadTreeNode(quadList[1]);
                quad2 = new QuadTreeNode(quadList[2]);
                quad3 = new QuadTreeNode(quadList[3]);

                ChildContaining(query, out node);

            } else if ( node is null ){

                indices.Add(index);
                return;

            }


            if (node is null){

                throw new ArgumentException(String.Format("Could not insert {0} into node: {1}", query.Print(), aoi.Print()));

            }

            node.Insert(in query, index, minarea);

            return;

        }



    }


}