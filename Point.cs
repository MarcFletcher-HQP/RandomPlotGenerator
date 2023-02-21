
using System;


namespace RandomPlotGenerator;


public class Point {
    
    public double[] coords;
    public PointStatus status;
    public double prob;


    public enum PointStatus {
        Undefined,
        Excluded,
        Selected
    }


    public Point(double x, double y){
        
        this.coords = new double[2];        
        this.coords[0] = x;
        this.coords[1] = y;

        this.prob = 0.0;
        this.status = PointStatus.Undefined;

    }


    public void SetInitialProb(double prob){
        
        if(prob < 0.0 || prob > 1.0){
            throw new ArgumentException("Probability can not be less than 0.0 or greater than 1.0.");
        }
        
        this.prob = prob;
    }


    public double GetX(){
        return coords[0];
    }


    public double GetY(){
        return coords[1];
    }


    public bool IsSelected(){
        return status == PointStatus.Selected;
    }


    public bool IsExcluded(){
        return status == PointStatus.Excluded;
    }


    public bool IsUndefined(){
        return status == PointStatus.Undefined;
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


    public void Select(){
        prob = 1.0;
        status = PointStatus.Selected;
    }


    public void Exclude(){
        prob = 0.0;
        status = PointStatus.Excluded;
    }

}

