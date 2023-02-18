
using System;


namespace RandomPlotGenerator;


public class Point {
    
    public double[] coords;


    public Point(double x, double y){
        
        this.coords = new double[2];
        
        this.coords[0] = x;
        this.coords[1] = y;
    }


    public double GetX(){
        return coords[0];
    }


    public double GetY(){
        return coords[1];
    }


    public double Distance2(double x, double y){
        return Math.Pow(x - this.GetX(), 2) + Math.Pow(y - this.GetY(), 2);
    }


    public double Distance2(Point xy){
        return Math.Pow(xy.GetX() - this.GetX(), 2) + Math.Pow(xy.GetY() - this.GetY(), 2);
    }


    public double this[int i]{
        get { return coords[i]; }
        set { coords[i] = value; }
    }

}
