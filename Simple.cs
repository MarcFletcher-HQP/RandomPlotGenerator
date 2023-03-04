
#define DEBUG
#undef DEBUG

using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Tri;
using NetTopologySuite.Triangulate.Polygon;



namespace RandomPlotGenerator;


/* Class: Simple

The Simple class returns a simple random sample of the polygon provided, such
that there is a uniform probability of selection across the area of the polygon.

The simple random sample is obtained by the following steps:
    1. Create a triangulation of the polygon.
    2. Determine which triangle each sample will be in using 
        a weighted sample, with probability being proportional
        to the area of the triangle.
    3. Produce a uniform random sample from the selected triangles. 

*/

public class Simple {

    private Random rng;
    private IList<Tri> triangulation;



    /* Constructor for WKT format - mostly used for testing.
    While not as compact, a WKT string is much more explicit. */
    public Simple(in Polygon polygon, int? seed){

        var triangulator = new ConstrainedDelaunayTriangulator(polygon);

        triangulation = triangulator.GetTriangles(); 

        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

    }


    /* Generate a sample of points from the polygon provided */
    public List<Coordinate> Sample(int size, int? seed){

        SelectionProbability(out double[] prob);


        Weighted trisample = new Weighted(in prob, null);

        List<int> index = trisample.Sample(size);


        List<Coordinate> sample = new List<Coordinate>();

        for(int i = 0; i < index.Count; i++){

            Tri trithis = triangulation[index[i]];

            sample.Add(PickPointInTriangle(trithis));

        }


        return sample;

    }


    public void SelectionProbability(out double[] prob){

        double totalarea = 0.0;

        foreach(Tri x in triangulation){
            totalarea += Area(x);
        }


        prob = new double[triangulation.Count];

        for(int i = 0; i < prob.Length; i++){

            prob[i] = ( (double) triangulation.Count ) * Area(triangulation[i]) / totalarea;

        }


        return;

    }


    /* Randomly generated a point inside a triangle
    
    This method forms two vectors from the vertices of the triangle, originating from the first vertex. A random scaling
    is applied to each vector and the results are added together to form the position vector for the generated point. As 
    linear combinations of these vectors span a parallelogram, there is a 50/50 chance that the point generated falls 
    outside of the triangle. As the second half of the parallelogram is just a reflection of the original triangle, we
    simply use the coordinates of the point relative to this "mirrored" triangle.

     */
    private Coordinate PickPointInTriangle(Tri triangle){

        Coordinate origin = triangle.GetCoordinate(0);

        Vector2D v1 = new Vector2D(triangle.GetCoordinate(1), origin);
        Vector2D v2 = new Vector2D(triangle.GetCoordinate(2), origin);

        double scale1 = rng.NextDouble();
        double scale2 = rng.NextDouble();

        Vector2D pointvec = scale1 * v1 + scale2 * v2;


        if(!PointInTriangle(triangle, pointvec.ToCoordinate())){

            scale1 = 1 - scale1;
            scale2 = 1 - scale2;

            pointvec = scale1 * v1 + scale2 * v2;

        }

        return pointvec.ToCoordinate();

    }


    /* Tests whether a point is interior to the triangle provided. 

    This method utilises the fact that the edge of a triangle defines a line which divides the plane in two. A point
    that is exterior to the triangle will be on the opposite side of this line to the remaining vertex of the triangle, 
    for one of the three edges. If the point is on the same side, for all edges, then the point is inside the triangle.

    There are potentially quicker algorithms, but I like the geometric approach.
    */
    private static bool PointInTriangle(Tri triangle, Coordinate point){

        Coordinate origin = triangle.GetCoordinate(0);

        /* NetTopologySuite.Triangulate.Tri uses a clockwise orientation for the vertices of a triangle */

        Coordinate p0 = triangle.GetCoordinate(0);
        Coordinate p1 = triangle.GetCoordinate(1);
        Coordinate p2 = triangle.GetCoordinate(2);


        /* Check that the test point is on the same side of each edge */

        bool pass = Vector2D.SameSide(new Vector2D(point, p0), new Vector2D(p1, p0), new Vector2D(p2, p0)) && 
                Vector2D.SameSide(new Vector2D(point, p1), new Vector2D(p2, p1), new Vector2D(p0, p1)) && 
                Vector2D.SameSide(new Vector2D(point, p2), new Vector2D(p0, p2), new Vector2D(p1, p2));

        return pass;

    }




    /* Class Vector2D: Extension of the Coordinate class
    
    The Coordinate class provides the notion of a generic point in NetTopologySuite. Code used elsewhere in 
    the 'Simple' class requires the manipulation of Coordinates in a geometric manner, i.e. as position 
    vectors. The Vector2D class provides arithmetic operators for manipulating and combining Coordinates.
     */
    private class Vector2D : Coordinate {

        Coordinate origin;


        public Vector2D(Coordinate xy, Coordinate origin) : base(xy[0] - origin[0], xy[1] - origin[1]){
            this.origin = origin;
        }

        public Vector2D(double x, double y, double x0, double y0) : base(x - x0, y - y0){
            origin = new Coordinate(x0, y0);
        }

        public Coordinate ToCoordinate(){

            double x = this.X + origin[0];
            double y = this.Y + origin[1];

            return new Coordinate(x, y);

        }

        private void SetOrigin(Coordinate origin){
            this.origin = origin;
        }

        public static Vector2D operator +(Vector2D u, Vector2D v){

            if(u.origin != v.origin){
                throw new ArgumentException("Cannot perform arithmetic operations on Vector2D with different origins");
            }

            Vector2D vecsum = new Vector2D(u[0] + v[0], u[1] + v[1], 0.0, 0.0);

            vecsum.SetOrigin(u.origin);

            return vecsum;

        }

        public static Vector2D operator *(Vector2D u, double x){

            return x * u;

        }

        public static Vector2D operator *(double x, Vector2D u){

            Vector2D scalarprod = new Vector2D(x * u[0], x * u[1], 0.0, 0.0);

            scalarprod.SetOrigin(u.origin);

            return scalarprod;

        }

        public static double Determinant(Vector2D u, Vector2D v){

            return u[0] * v[1] - u[1] * v[0];

        }

        public static bool SameSide(Vector2D test, Vector2D line, Vector2D reference){

            int cp1 = Math.Sign(Determinant(test, line));
            int cp2 = Math.Sign(Determinant(reference, line));

            return cp1 == cp2;

        }

        public string Print(){

            return String.Format("Vector2D: (x, y): ({0}, {1})  origin: ({2}, {3})", this.X, this.Y, origin[0], origin[1]);

        }

        public override double this[int i]{
            get{ return (i == 0) ? this.X : this.Y; }

            set{
                if(i == 0){
                    this.X = value;
                } else if (i == 1){
                    this.Y = value;
                } else {
                    throw new ArgumentException("Vector2D can only subset on dimension 0, or 1");
                }
            }
        }

    }


    /* Since the class 'Tri' doesn't have a notion of area */
    static double Area(Tri tri){

        Vector2D v1 = new Vector2D(tri.GetCoordinate(1), tri.GetCoordinate(0));
        Vector2D v2 = new Vector2D(tri.GetCoordinate(2), tri.GetCoordinate(0));

        return Math.Abs(Vector2D.Determinant(v1, v2) / 2);

    }


    public string Print(int? digits){

        if(digits is null){

            digits = 4;

        }

        string polybuff = "";
        char[] trimchars = {',', ' '};

        foreach(Tri tri in triangulation){

            string tribuff = "";

            for(int i = 0; i < 3; i++){

                Coordinate xy = tri.GetCoordinate(2-i);

                tribuff += String.Format("{0} {1}", xy.X, xy.Y);
                tribuff += ", ";

            }

            tribuff += String.Format("{0} {1}", tri.GetCoordinate(2).X, tri.GetCoordinate(2).Y);

            polybuff += String.Format("(({0}))", tribuff);
            polybuff += ", ";

        }

        polybuff = polybuff.TrimEnd(trimchars);


        /* Print the total area */

        double totalarea = 0.0;
        string areabuff = "";

        foreach(Tri tri in triangulation){

            double area = Area(tri);

            areabuff += Math.Round(area, (int) digits);
            areabuff += ", ";

            totalarea += area;

        }

        string strbuff = String.Format("Triangulation");

        strbuff += String.Format("MULTIPOLYGON ({0})", polybuff);

        strbuff += String.Format("Total Area: {0}  Areas: {1}", Math.Round(totalarea, (int) digits), areabuff);

        return strbuff;

    }


}


