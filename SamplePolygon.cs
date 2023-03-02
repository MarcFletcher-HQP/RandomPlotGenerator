
#define DEBUG
#undef DEBUG

using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Tri;
using NetTopologySuite.Triangulate.Polygon;



namespace RandomPlotGenerator;


/* Class: SamplePolygon

SamplePolygon class encapsulates the NetTopologySuite functionality used for:
    1. Creating a polygon from WKT.
    2. Creating a SamplePolygon of the imported polygon.
    3. Providing a number of classes for convenience.
    4. Providing methods for determining polygon attributes, such as area.

The SamplePolygon class is responsible for: 
    1. re-creating the (user) supplied polygon,
    2. creating a SamplePolygon, 
    3. generating a random sample of triangles in which to generate points, 
    4. generating points randomly from within each triangle,
    5. returning the sample as a list.

The output from this class will precede the use of other classes like 
LocalPivotalMethod.

*/

public class SamplePolygon {

    private Random rng;
    private Polygon polygon;
    private IList<Tri>? triangulation;



    /* Constructor for WKT format - mostly used for testing.
    While not as compact, a WKT string is much more explicit. */
    public SamplePolygon(string wkt, int? seed){

        var reader = new NetTopologySuite.IO.WKTReader();

        polygon = (Polygon) reader.Read(wkt);

        var triangulator = new ConstrainedDelaunayTriangulator(polygon);

        triangulation = triangulator.GetTriangles(); 

        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

    }


    /* Constructor for WKB format - likely to be used in the future.
    This method is preferable as WKB is more compact and therefore 
    easier to send via a web-service.
     */
    public SamplePolygon(byte[] wkb, int? seed){

        var reader = new NetTopologySuite.IO.WKBReader();

        polygon = (Polygon) reader.Read(wkb); 

        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

    }


    public void CreateTriangulation(){

        if(triangulation is null){

            var triangulator = new ConstrainedDelaunayTriangulator(polygon);

            triangulation = triangulator.GetTriangles();

        }

        return;

    }




    /* Accessor - for use outside the library */
    public static IEnumerable<Tri> Triangulation(SamplePolygon polygon){

        if(polygon.triangulation is null){

            polygon.CreateTriangulation();

        }

        foreach(Tri triangle in polygon.triangulation!){  // '!' apparently asserts that polygon.triangulation will not be null.

            yield return triangle;

        }

    }


    public List<Coordinate> RandomGrid(double dx, double dy){

        Envelope box = polygon.EnvelopeInternal;

        return this.RandomGrid(box.MinX, box.MinY, box.MaxX, box.MaxY, dx, dy);

    }


    public List<Coordinate> RandomGrid(double xmin, double ymin, double xmax, double ymax, double dx, double dy) {

        // Generate a random starting point and create a grid

        Coordinate start = PointInsideBox(xmin, ymin, xmin + dx, ymin + dy);

        int n = Convert.ToInt32(Math.Truncate((xmax - start.X) / dx)) + 2;
        int m = Convert.ToInt32(Math.Truncate((ymax - start.Y) / dy)) + 2;

        List<Coordinate> grid = new List<Coordinate>();

        for( int i = 0; i < n; i++ ){

            for( int j = 0; j < m; j++ ){

                grid.Add(new Coordinate(start.X + i * dx, start.Y + j * dy));

            }

        }

        return grid;

    }


    private Coordinate PointInsideBox(double xmin, double ymin, double xmax, double ymax){

        double x = rng.NextDouble() * (xmax - xmin) + xmin;
        double y = rng.NextDouble() * (ymax - ymin) + ymin;

        return new Coordinate(x, y);

    }




    /* Generate a sample of points from the polygon provided */
    public List<Coordinate> GenerateRandomPoints(int? numpts){

        if (numpts is null){
            numpts = 1;
        }

        List<Coordinate> sample = new List<Coordinate>();

        PickRandomTriangles(numpts, out int[] triIdx);


        for(int i = 0; i < triIdx.Length; i++){

            Tri trithis = triangulation![triIdx[i]];

            sample.Add(PickPointInTriangle(trithis));

        }

        return sample;

    }



    /* Generate a sample of triangles */
    public void PickRandomTriangles(int? numtris, out int[] sample){

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


    /* Simulate unequal probability sampling using a fair die and a biased coin
    
    To ensure that the probability of sampling a location within the polygon is uniform
    throughout the whole area, it is necessary to bias the selection of triangles by 
    perferring those with a larger area. 
    
    The Alias Method presents an efficient means of randomly sampling discrete units with 
    unequal probability. At its core, the method randomly selects a triangle and then flips
    a biased coin to determine whether the triangle is sampled; with the probability of heads
    being determined by the proportion of the total area occupied by the triangle. The arrays
    created in the CreateAliasTables method provide a way to make this process more efficient.

    */
    public void CreateAliasTables(out double[] prob, out int[] alias){

        if(triangulation is null){

            this.CreateTriangulation();

        }

        int N = triangulation!.Count;


        double totalarea = 0.0;

        foreach(Tri x in triangulation){
            totalarea += TriArea(x);
        }


        alias = new int[triangulation.Count];
        prob = new double[triangulation.Count];

        for(int i = 0; i < prob.Length; i++){
            prob[i] = ( (double) N ) * TriArea(triangulation[i]) / totalarea;
        }


        SortedSet<AliasNode> bst = new SortedSet<AliasNode>(new AliasNodeComparer());

        for(int i = 0; i < N; i++){

            bst.Add(new AliasNode(prob[i], i));

        }


        List<AliasNode> sortednodes = new List<AliasNode>();

        for(int i = 0; i < N; i++){

            AliasNode? minnode = bst.Min;
            AliasNode? maxnode = bst.Max;

            if((minnode is null || (maxnode is null))){
                break;
            }

            bst.Remove(minnode);
            bst.Remove(maxnode);

            minnode.alias = maxnode.index;
            maxnode.prob = maxnode.prob - (1 - minnode.prob);

            sortednodes.Add(minnode);
            bst.Add(maxnode);

        }



        for(int i = 0; i < N; i++){

            int index = sortednodes[i].index;

            prob[index] = sortednodes[i].prob;
            alias[index] = sortednodes[i].alias;

        }

        

        return;

    }



    /* Binary Search Tree representation of the data */
    private class AliasNode {

        public double prob;
        public int alias;
        public int index;

        public AliasNode(double prob, int index){

            this.prob = prob;
            this.alias = index;
            this.index = index;

        }

    }


    /* Method for ordering AliasNodes in a Binary Search Tree */
    private class AliasNodeComparer : IComparer<AliasNode> {

        public int Compare(AliasNode? node1, AliasNode? node2){

            if ((node1 is null) || (node2 is null)){

                throw new ArgumentException("AliasNodes cannot be null!");

            }

            return (node1.prob).CompareTo(node2.prob);

        }

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
    the SamplePolygon class requires the manipulation of Coordinates in a geometric manner, i.e. as position 
    vectors. The Vector2D class provides arithmetic operators for manipulating and combining Coordinates.
     */
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


    /* Static members - Mostly used for print methods (i.e. Debugging) */

    public static string PrintArray(in double[] xin, int digits){

        string buff = "";

        foreach(double x in xin){
            buff += Math.Round(x, digits);
            buff += " ";
        }

        buff.Trim();

        return buff;

    }


    public static string PrintArray(in int[] xin){

        string buff = "";

        foreach(double x in xin){
            buff += x;
            buff += " ";
        }

        buff.Trim();

        return buff;

    }



    public static string SamplePolygonWKT(SamplePolygon x){

        return x.polygon.AsText();

    }


    public static string TriangulationWKT(SamplePolygon polygon){

        if(polygon.triangulation is null){

            polygon.CreateTriangulation();

        }

        string buff = "";
        char[] trimchars = {',', ' '};

        foreach(Tri tri in polygon.triangulation!){

            string tribuff = "";

            for(int i = 0; i < 3; i++){

                Coordinate xy = tri.GetCoordinate(2-i);

                tribuff += String.Format("{0} {1}", xy.X, xy.Y);
                tribuff += ", ";

            }

            tribuff += String.Format("{0} {1}", tri.GetCoordinate(2).X, tri.GetCoordinate(2).Y);

            buff += String.Format("(({0}))", tribuff);
            buff += ", ";

        }

        buff = buff.TrimEnd(trimchars);

        return String.Format("MULTIPOLYGON ({0})", buff);

    }


    public static void TriangulationArea(SamplePolygon polygon, out double[] area){

        if(polygon.triangulation is null){

            polygon.CreateTriangulation();

        }

        area = new double[polygon.triangulation!.Count];

        for(int i = 0; i < polygon.triangulation.Count; i++){

            area[i] = TriArea(polygon.triangulation[i]);

        }

        return;

    }




    /* Since the class 'Tri' doesn't have a notion of area */
    static double TriArea(Tri triangle){

        Vector2D v1 = new Vector2D(triangle.GetCoordinate(1), triangle.GetCoordinate(0));
        Vector2D v2 = new Vector2D(triangle.GetCoordinate(2), triangle.GetCoordinate(0));

        return Math.Abs(Vector2D.Determinant(v1, v2) / 2);

    }


    public static Geometry PolygonBBox(SamplePolygon polygon){

        return polygon.polygon.Envelope;

    }


}


