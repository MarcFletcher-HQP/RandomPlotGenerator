
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


/* Class: AOIBox

Class for representing a bounding box. Can be used to perform AOI queries on points or other AOIBoxes.

Not really used at the moment.
 */
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

        this.minpt = new Point(xmin, ymin);
        this.maxpt = new Point(xmax, ymax);

    }


    public AOIBox(Point minpt, Point maxpt){

        if(maxpt.X < minpt.X){

            double swapx = minpt.X;
            minpt[0] = maxpt.X;
            minpt[0] = swapx;

        }

        if(maxpt.Y < minpt.Y){

            double swapy = minpt.Y;
            minpt[1] = maxpt.Y;
            minpt[1] = swapy;

        }

        this.minpt = minpt;
        this.maxpt = maxpt;

    }


    public AOIBox(Point centre, double dx, double dy){

        minpt = new Point(centre.X - dx, centre.Y - dy);
        maxpt = new Point(centre.X + dx, centre.Y + dy);

    }


    public Point Centre(){

        double xmid = (minpt.X + maxpt.X) / 2.0;
        double ymid = (minpt.Y + maxpt.Y) / 2.0;

        return new Point(xmid, ymid);

    }


    public List<AOIBox> Quadrants(){

        Point mid = this.Centre();

        Point minquad1 = new Point(minpt.X, mid.Y);
        Point maxquad1 = new Point(mid.X, maxpt.Y);
        Point minquad2 = new Point(mid.X, minpt.Y);
        Point maxquad2 = new Point(maxpt.X, mid.Y);

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

        return (minpt.X <= query.X) && 
            (minpt.Y <= query.Y) &&
            (maxpt.X >= query.X) &&
            (maxpt.Y >= query.Y);

    }


    public bool Contains(AOIBox other){

        return this.Contains(other.minpt) && 
            this.Contains(other.maxpt);

    }


    public bool Intersects(AOIBox other){

        bool betweenX = ! ( (other.minpt.X >= this.maxpt.X) || (other.maxpt.X <= this.minpt.X) );
        bool betweenY = ! ( (other.minpt.Y >= this.maxpt.Y) || (other.maxpt.Y <= this.minpt.Y) );

        return betweenX && betweenY;

    }


    public bool Intersects(Point query){

        return this.Contains(query);

    }


    public string Print(){

        return String.Format("AOI({0}, {1})", minpt.Print(), maxpt.Print());

    }


}