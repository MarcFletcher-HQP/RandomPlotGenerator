

using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Tri;

using RandomPlotGenerator;

using Point = RandomPlotGenerator.Point;

class Program{

    /* Program Inputs */

    static string? wkt;
    static int? numplots;


    /* Example Inputs */
    static string ExampleWKT = "POLYGON ((513820.1 7017121, 513833.4 7017112, 513843.6 7017102, 513843.6 7017060, 513841.9 7017065, 513837.8 7017077, 513828 7017101, 513822.9 7017113, 513820.1 7017121))";
    static int ExampleNumPlots = 5;


    /* Program Defaults */
    static int NumCandidates = 1000;
    static int digits = 4;
    static int seed = 117;
    static double gridsizeX = 5.0;
    static double gridsizeY = 15.0;



    static void Main(string[] args){

        if(args.Contains("--wkt")){

            int pos = Array.FindIndex(args, s => s.Equals("--wkt"));

            wkt = args[pos + 1];

        } else {

            wkt = ExampleWKT;

        }

        if(args.Contains("--numplots")){

            int pos = Array.FindIndex(args, s => s.Equals("--numplots"));

            numplots = Convert.ToInt32(args[pos + 1]);

        } else {

            numplots = ExampleNumPlots;

        }


        bool testall = false;

        if(args.Contains("All")){

            testall = true;

        }


        /* Now dispatch on workflow */

        if(args.Contains("Workflow")){

            if(testall || args.Contains("Survival")){

                string response = Workflow_Survival();

            }

        }


        /* Run some tests instead */

        if(args.Contains("Test")){

            if(testall || args.Contains("SamplePolygon")){

                Test_SamplePolygon();

            }

            if(testall || args.Contains("RandomPoints")){

                Test_RandomPoints();

            }

            if(testall || args.Contains("RandomGrid")){

                Test_RandomGrid();

            }

            if(testall || args.Contains("KDTree")){

                Test_KDTree();

            }

            if(testall || args.Contains("LPM")){

                Test_LPM();

            }
            

        }


        return;

    }




    /* Workflows */

    static string Workflow_Survival(){

        // Check arguments

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }

        if(numplots is null){
            throw new ArgumentException("Number of plots was not provided");
        }


        // Read polygon and create triangulation

        SamplePolygon polygon = new SamplePolygon(wkt, null);


        // Randomly generate points inside the polygon, convert to a 'Point'.

        List<Coordinate> candidates = polygon.GenerateRandomPoints(NumCandidates);


        // Generate sample using the Local Pivotal Method

        LocalPivotalMethod lpm = new LocalPivotalMethod(null, null);

        List<Point> sample = lpm.SamplePoints(candidates, (int) numplots, null);


        // Return WKT for the sample

        return Point.MultiPointToWKT(in sample);

    }





    /* Tests */

    static void Test_SamplePolygon(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }


        // Print input before any possible errors occur, due to SamplePolygon();

        Console.WriteLine(String.Format("Test_SamplePolygon: Input WKT: {0}", wkt));


        // Read polygon and create triangulation and print geometric info

        SamplePolygon polygon = new SamplePolygon(wkt, null);

        SamplePolygon.TriangulationArea(polygon, out double[] area);

        Console.WriteLine(String.Format("Test_SamplePolygon: Polygon: {0}", SamplePolygon.SamplePolygonWKT(polygon)));
        Console.WriteLine(String.Format("Test_SamplePolygon: Triangulation: {0}", SamplePolygon.TriangulationWKT(polygon)));
        Console.WriteLine(String.Format("Test_SamplePolygon: Triangle Areas: {0}", SamplePolygon.PrintArray(area, 2)));
        Console.WriteLine("");


        return;

    }



    static void Test_RandomPoints(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }


        /* Deliberately ignoring the default number of candidates, bit hard to see what's 
        going on when you print 1,000 points! */

        int candidates = 10;


        SamplePolygon polygon = new SamplePolygon(wkt, seed);


        // Check the alias table

        polygon.CreateAliasTables(out double[] prob, out int[] alias);

        Console.WriteLine(String.Format("Test_GenerateRandomPoints: Prob: {0}", SamplePolygon.PrintArray(prob, digits)));
        Console.WriteLine(String.Format("Test_GenerateRandomPoints: Alias: {0}", SamplePolygon.PrintArray(alias)));


        // Randomly pick triangles

        polygon.PickRandomTriangles(candidates, out int[] index);

        Console.WriteLine(String.Format("Test_GenerateRandomPoints: Index: {0}", SamplePolygon.PrintArray(index)));


        // Generate Points from Triangles

        List<Coordinate> sample = polygon.GenerateRandomPoints(candidates);

        List<Point> samplepts = Point.CoordinateToPoint(sample);

        Console.WriteLine(String.Format("Test_GenerateRandomPoints: Sample: {0}", Point.MultiPointToWKT(samplepts)));
        

        Console.WriteLine("");


        return;

    }


    static void Test_RandomGrid(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }

        SamplePolygon polygon = new SamplePolygon(wkt, seed);

        Console.WriteLine(String.Format("Test_RandomGrid: polygon: {0}", SamplePolygon.SamplePolygonWKT(polygon)));



        List<Coordinate> grid = polygon.RandomGrid(gridsizeX, gridsizeY);

        Console.WriteLine(String.Format("Test_RandomGrid: grid: {0}", Point.MultiPointToWKT(grid)));

        Console.WriteLine("");

        return;

    }



    static void Test_KDTree(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }

        SamplePolygon polygon = new SamplePolygon(wkt, seed);

        List<Point> grid = Point.CoordinateToPoint( polygon.RandomGrid(gridsizeX, gridsizeY) );


        // Print Grid

        Console.WriteLine(String.Format("Test_KDTree: polygon: {0}", SamplePolygon.SamplePolygonWKT(polygon)));
        Console.WriteLine(String.Format("Test_KDTree: grid: {0}", Point.MultiPointToWKT(grid)));


        // Construct KDTree

        KDTree tree = new KDTree(grid, null);


        /* Search through nearest neighbours. */

        Point query = grid[(int) grid.Count/2];

        Console.WriteLine(String.Format("Test_KDTree: SearchNN: query: {0}", query.ToWKT()));

        List<Point> neighbours = new List<Point>();

        for (int i = 0; i < 6; i++){

            tree.SearchNN(query, true, out Point neighbour);

            neighbour.Exclude();

            neighbours.Add(neighbour);

            Console.WriteLine(
                String.Format("Test_KDTree: SearchNN: i: {0}  neighbour: {1}  Distance: {2}", 
                    i, neighbour.ToWKT(), query.Distance(neighbour))
            );

        }

        Console.WriteLine(String.Format("Test_KDTree: SearchNN All Neighbours: {0}", Point.MultiPointToWKT(neighbours)));

        Console.WriteLine("");

        return;

    }


    static void Test_LPM(){

        if(wkt is null){

            throw new ArgumentException("No WKT provided for input polygon");

        }

        if(numplots is null){

            throw new ArgumentException("Number of plots has not been provided");

        }


        // Read polygon and create triangulation

        SamplePolygon polygon = new SamplePolygon(wkt, null);

        Console.WriteLine(String.Format("Test_LPM: polygon: {0}", SamplePolygon.SamplePolygonWKT(polygon)));


        // Randomly generate points inside the polygon, convert to a 'Point'.

        List<Coordinate> candidates = polygon.GenerateRandomPoints(NumCandidates);


        // Generate sample using the Local Pivotal Method

        LocalPivotalMethod lpm = new LocalPivotalMethod(seed, null);

        List<Point> sample = lpm.SamplePoints(candidates, (int) numplots, null);


        // Print output

        Console.WriteLine(String.Format("Test_LPM: numplots: {0}  candidates: {1}  sample: {2}", numplots, candidates.Count, Point.MultiPointToWKT(sample)));


        Console.WriteLine("");


        return;

    }
    



}