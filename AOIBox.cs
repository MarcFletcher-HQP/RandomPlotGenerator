
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class AOIBox{

    /* private readonly double xmin, ymin, xmax, ymax; */
    public readonly Point minpt, maxpt;


    public AOIBox(double xmin, double ymin, double xmax, double ymax){

        if(xmax < xmin){

            double swapx = xmin;
            xmin = xmax;
            xmax = swapx;

        }

        if(ymax < ymin){

            double swapy = ymin;
            ymin = ymax;
            ymax = swapy;

        }

        /* this.xmin = xmin;
        this.ymin = ymin;
        this.xmax = xmax;
        this.ymax = ymax; */

        this.minpt = new Point(xmin, ymin);
        this.maxpt = new Point(xmax, ymax);

    }


    public AOIBox(Point minpt, Point maxpt){

        if(maxpt.GetX() < minpt.GetX()){

            double swapx = minpt.GetX();
            minpt.SetX(maxpt.GetX());
            minpt.SetX(swapx);

        }

        if(maxpt.GetY() < minpt.GetY()){

            double swapy = minpt.GetY();
            minpt.SetY(maxpt.GetY());
            minpt.SetY(swapy);

        }

        this.minpt = minpt;
        this.maxpt = maxpt;

    }


    public AOIBox(Point centre, double dx, double dy){

        minpt = new Point(centre.GetX() - dx, centre.GetY() - dy);
        maxpt = new Point(centre.GetX() + dx, centre.GetY() + dy);

    }


    public Point Centre(){

        double xmid = (minpt.GetX() + maxpt.GetX()) / 2.0;
        double ymid = (minpt.GetY() + maxpt.GetY()) / 2.0;

        return new Point(xmid, ymid);

    }


    public List<AOIBox> Quadrants(){

        Point mid = this.Centre();

        Point minquad1 = new Point(minpt.GetX(), mid.GetY());
        Point maxquad1 = new Point(mid.GetX(), maxpt.GetY());
        Point minquad2 = new Point(mid.GetX(), minpt.GetY());
        Point maxquad2 = new Point(maxpt.GetX(), mid.GetY());

        List<AOIBox> quads = new List<AOIBox>(4);

        quads[0] = new AOIBox(minpt, mid);
        quads[1] = new AOIBox(minquad1, maxquad1);
        quads[2] = new AOIBox(minquad2, maxquad2);
        quads[3] = new AOIBox(mid, maxpt);

        return quads;

    }


    public double Size(int dimension){

        if (dimension > 1){
            throw new ArgumentException("Dimension cannot exceed 1 (2D)");
        }

        return maxpt[dimension] - minpt[dimension];

    }


    public double Area(){

        return Size(0) * Size(1);

    }


    public bool Contains(Point query){

        return (minpt.GetX() <= query.GetX()) && 
            (minpt.GetY() <= query.GetY()) &&
            (maxpt.GetX() >= query.GetX()) &&
            (maxpt.GetY() >= query.GetY());

    }


    public bool Contains(AOIBox other){

        return this.Contains(other.minpt) && 
            this.Contains(other.maxpt);

    }


    public bool Intersects(AOIBox other){

        bool betweenX = ! ( (other.minpt.GetX() >= this.maxpt.GetX()) || (other.maxpt.GetX() <= this.minpt.GetX()) );
        bool betweenY = ! ( (other.minpt.GetY() >= this.maxpt.GetY()) || (other.maxpt.GetY() <= this.minpt.GetY()) );

        return betweenX && betweenY;

    }


    public bool Intersects(Point query){

        return this.Contains(query);

    }


    public string Print(){

        return String.Format("AOI({0}, {1})", minpt.Print(), maxpt.Print());

    }


}