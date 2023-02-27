
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Tri;
using NetTopologySuite.Triangulate.Polygon;



namespace RandomPlotGenerator;



public class Triangulation {

    private Random rng;
    private Polygon polygon;    // Not sure of the purpose
    private IList<Tri> triangles;



    public Triangulation(string wkt, int? seed){

        var reader = new NetTopologySuite.IO.WKTReader();

        polygon = (Polygon) reader.Read(wkt);

        var triangulator = new ConstrainedDelaunayTriangulator(polygon);

        triangles = triangulator.GetTriangles(); 

        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

    }



    public Tri PickRandomTriangle(){

        int index = rng.Next(0, triangles.Count);

        return triangles[index];

    }



    public List<Point> GenerateRandomPoints(int? numpts){

        if (numpts is null){
            numpts = 1;
        }

        List<Point> sample = new List<Point>();


        for(int i = 0; i < numpts; i++){

            Tri trithis = PickRandomTriangle();

            

        }



    }







}


