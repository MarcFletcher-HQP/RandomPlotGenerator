
using System;
using NetTopologySuite.Geometries;


namespace RandomPlotGenerator;


/* Class: Point

Point extends NetTopologySuite's Coordinate class and is used to represent sampling units 
within the polygon; that will either be accepted or rejected by the chosen sampling method 
(currently just the Local Pivotal Method).

The Local Pivotal Method has been implemented in this library by storing the candidate 
sampling units in a KD-tree. The Point class has likewise been developed to meet this need.
 */
public class Point : Coordinate {
    
    public PointStatus status;
    public double prob;


    public enum PointStatus {
        Undefined,
        Excluded,
        Selected
    }


    /* Constructor for when you don't know the initial probability */
    public Point(double x, double y) : base(x, y){
        
        this.prob = 0.0;
        this.status = PointStatus.Undefined;

    }


    /* Constructor for when you do */
    public Point(double x, double y, double prob) : base(x, y){
        
        if(prob < 0.0 || prob > 1.0){
            throw new ArgumentException("Probability can not be less than 0.0 or greater than 1.0.");
        }

        this.prob = prob;
        this.status = PointStatus.Undefined;

    }
    


    /* Sometimes we don't know what the initial probability should be, we don't have all of the points yet. */
    public void SetInitialProb(double prob){
        
        if(prob < 0.0 || prob > 1.0){
            throw new ArgumentException("Probability can not be less than 0.0 or greater than 1.0.");
        }
        
        this.prob = prob;

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


    /* KD-Tree construction and querying rarely requires the actual distance
    Using the squared-distance saves us calculating a sqrt. */
    public double Distance2(double x, double y){
        
        return Math.Pow(x - this.X, 2) + Math.Pow(y - this.Y, 2);

    }

    public double Distance2(Point xy){
        
        return Math.Pow(xy.X - this.X, 2) + Math.Pow(xy.Y - this.Y, 2);

    }

    public void Select(){
        
        prob = 1.0;
        status = PointStatus.Selected;

    }

    public void Exclude(){
        
        prob = 0.0;
        status = PointStatus.Excluded;

    }


    /* WKT for a single point */
    public string Print(){

        return String.Format("POINT({0} {1})", this.X, this.Y);

    }


    /* WKT for a single point */
    public string ToWKT(){

        return String.Format("POINT({0} {1})", this.X, this.Y);

    }


    /* WKT for a single point */
    static string PointToWKT(Point point){

        return point.ToWKT();

    }


    /* Output will need to be written to WKT (for now). Returning as a multipoint. */
    static string MultiPointToWKT(List<Point> points){

        string buff = "";

        foreach(Point pt in points){

            buff += String.Format("({0} {1})", pt.X, pt.Y);
            buff += ", ";

        }

        char[] cleanup = {',', ' '};

        buff.TrimEnd(cleanup);

        return String.Format("MULTIPOINT({0})", buff);

    }


}

