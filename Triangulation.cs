
#define DEBUG
#undef DEBUG

using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Tri;
using NetTopologySuite.Triangulate.Polygon;



namespace RandomPlotGenerator;



public class Triangulation {

    private Random rng;
    private Polygon polygon;    // Not sure of the purpose
    private IList<Tri> triangles;
    private double[] area;



    public Triangulation(string wkt, int? seed){

        var reader = new NetTopologySuite.IO.WKTReader();

        polygon = (Polygon) reader.Read(wkt);

        
        var triangulator = new ConstrainedDelaunayTriangulator(polygon);

        triangles = triangulator.GetTriangles(); 

        
        area = new double [(triangles.Count)];

        for(int i = 0; i < area.Length; i++){

            area[i] = triangles[i].Area;

        }


        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

    }


    public List<Coordinate> GenerateRandomPoints(int? numpts){

        if (numpts is null){
            numpts = 1;
        }

        List<Coordinate> sample = new List<Coordinate>();

        PickRandomTriangles(numpts, out int[] triIdx);


        for(int i = 0; i < triIdx.Length; i++){

            Tri trithis = triangles[triIdx[i]];

            sample.Add(PickPointInTriangle(trithis));

        }

        return sample;

    }




    private void PickRandomTriangles(int? numtris, out int[] sample){

        if(numtris is null){
            numtris = 1;
        }


        sample = new int[ (int) numtris ];

        CreateAliasTables(out double[] prob, out int[] alias);


        // Using the "Alias Method" for generating an unequal probability sample.

        for(int i = 0; i < sample.Length; i++){

            int roll = rng.Next(0, prob.Length);
            
            double flip = rng.NextDouble();

            if(flip <= prob[roll]){

                sample[i] = roll;

            } else {

                sample[i] = alias[roll];

            }

        }

        return;

    }



    private void CreateAliasTables(out double[] prob, out int[] alias){

        int N = area.Length;


        double totalarea = 0.0;

        foreach(double x in area){
            totalarea += x;
        }


        prob = new double[area.Length];

        Array.Copy(area, prob, area.Length);

        for(int i = 0; i < prob.Length; i++){
            prob[i] = ( (double) N ) * prob[i] / totalarea;
        }


        int[] index = Enumerable.Range(0, prob.Length).ToArray();

        Array.Sort(prob, index);


        alias = new int[index.Length];
        Array.Copy(index, alias, index.Length); 


        for(int i = 0; i < N; i++){

            // Assumes arrays have been sorted 'i' will be the new smallest, 'N-1' will be the new biggest.

            alias[i] = index[N-1];

            prob[N-1] = prob[N-1] - (1 - prob[i]);

            double[] cpyprob = new double[prob.Length];     // oof, really?
            Array.Copy(prob, cpyprob, prob.Length);         // this sucks!

            Array.Sort(prob, index, i + 1, N - 1 - i);      // Order of the first 'i' elements doesn't change.
            Array.Sort(cpyprob, alias, i + 1, N - 1 - i);   // find a better way!

        }

        prob[N-1] = 1;  // Just in case it didn't work out this way



        // Return probability array to it's original order

        
        int[] cpyindex = new int[index.Length];     // oof, really?
        Array.Copy(index, cpyindex, index.Length);         // this sucks!

        Array.Sort(index, prob);
        Array.Sort(cpyindex, alias);   // find a better way!

        return;

    }



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




    private class Vector2D : Coordinate {

        Coordinate origin;

        public Vector2D(){
            origin = new Coordinate();
        }

        public Vector2D(Coordinate xy){
            origin = new Coordinate();
        }

        public Vector2D(Coordinate xy, Coordinate origin) : base(xy[0] - origin[0], xy[1] - origin[1]){
            this.origin = origin;
        }

        public Vector2D(double x, double y, Coordinate origin) : base(x - origin[0], y - origin[1]){
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


        public static Vector2D operator -(Vector2D u){

            Vector2D mirrorvec = new Vector2D(-u[0], -u[1], 0.0, 0.0);

            mirrorvec.SetOrigin(u.origin);

            return mirrorvec;

        }


        public static Vector2D operator -(Vector2D u, Vector2D v){

            if(u.origin != v.origin){
                throw new ArgumentException("Cannot perform arithmetic operations on Vector2D with different origins");
            }

            Vector2D vecdiff = new Vector2D(u[0] - v[0], u[1] - v[1], 0.0, 0.0);

            vecdiff.SetOrigin(u.origin);

            return vecdiff;

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


        public static double InnerProduct(Vector2D u, Vector2D v){

            if(u.origin != v.origin){
                throw new ArgumentException("Cannot perform arithmetic operations on Vector2D with different origins");
            }

            return u[0] * v[0] + u[1] * v[1];

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

            string buff = String.Format("Vector2D: (x, y): ({0}, {1})  origin: ({2}, {3})", this.X, this.Y, origin[0], origin[1]);

            return buff;

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


    static string PrintArray(in double[] xin, int digits){

        string buff = "";

        foreach(double x in xin){
            buff += Math.Round(x, digits);
            buff += " ";
        }

        buff.Trim();

        return buff;

    }


    static string PrintArray(in int[] xin){

        string buff = "";

        foreach(double x in xin){
            buff += x;
            buff += " ";
        }

        buff.Trim();

        return buff;

    }


}


