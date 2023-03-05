

using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Tri;

using RandomPlotGenerator;


class Program{

    /* Program Inputs */

    static string? wkt;
    static int? numplots;


    /* Example Inputs */
    static string ExampleWKT = "POLYGON ((513820.1 7017121, 513833.4 7017112, 513843.6 7017102, 513843.6 7017060, 513841.9 7017065, 513837.8 7017077, 513828 7017101, 513822.9 7017113, 513820.1 7017121))";
    static int ExampleNumPlots = 5;


    /* Program Defaults */
    static int NumCandidates = 1000;
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

                Console.WriteLine(response);

            }

        }


        /* Run some tests instead */

        if(args.Contains("Test")){

            if(testall || args.Contains("Simple")){

                Test_Simple();

            }

            if(testall || args.Contains("Weighted")){

                Test_Weighted();

            }

            if(testall || args.Contains("Grid")){

                Test_Grid();

            }

            if(testall || args.Contains("KDTree")){

                Test_KDTree();

            }

            if(testall || args.Contains("SpatiallyBalanced")){

                Test_SpatiallyBalanced();

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

        var reader = new NetTopologySuite.IO.WKTReader();   // Change to WKBReader for WKB

        Polygon polygon = (Polygon) reader.Read(wkt);


        // Randomly generate points inside the polygon, convert to a 'Point'.

        Simple srs = new Simple(polygon, null);

        List<Coordinate> candidates = srs.Sample(NumCandidates, null);


        // Generate sample using the Local Pivotal Method

        SpatiallyBalanced lpm = new SpatiallyBalanced(null, null);

        List<Coordinate> sample = lpm.Sample(candidates, (int) numplots, null);


        // Return WKT for the sample

        return Print.MultiPointWKT(sample);

    }





    /* Tests */

    static void Test_Simple(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }


        // Print input before any possible errors occur, due to Simple();

        Console.WriteLine(String.Format("Test_Simple: Input WKT: {0}", wkt));


        // Read polygon and create triangulation and print geometric info

        var reader = new NetTopologySuite.IO.WKTReader();   // Change to WKBReader for WKB

        Polygon polygon = (Polygon) reader.Read(wkt);

        Simple srs = new Simple(polygon, null);


        // Print diagnostics

        Console.WriteLine(String.Format("Test_Simple: Polygon: {0}", polygon.ToString()));

        Console.WriteLine(String.Format("Test_Simple: {0}", srs.Print(2)));

        Console.WriteLine("");


        return;

    }



    static void Test_Weighted(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }


        /* Deliberately ignoring the default number of candidates, bit hard to see what's 
        going on when you print 1,000 points! */

        var reader = new NetTopologySuite.IO.WKTReader();   // Change to WKBReader for WKB

        Polygon polygon = (Polygon) reader.Read(wkt);

        Simple srs = new Simple(polygon, null);

        srs.SelectionProbability(out double[] prob);

        Weighted wrs = new Weighted(prob, seed);


        // Check the alias table

        for(int i = 0; i < prob.Length; i++){
            prob[i] = Math.Round(prob[i], 4);
        }

        Console.WriteLine("Test_Weighted: Prob: {0}", Print.Array<double>(prob));

        Console.WriteLine("Test_Weighted: {0}", wrs.Print(4));


        // Randomly pick triangles

        int candidates = 10;

        List<int> index = wrs.Sample(candidates);

        Console.WriteLine(String.Format("Test_Weighted: Selected Triangles: {0}", Print.List<int>(index)));


        // Generate Points from Triangles

        List<Coordinate> sample = srs.Sample(candidates, null);

        Console.WriteLine(String.Format("Test_Weighted: Sample: {0}", Print.MultiPointWKT(sample)));
        
        Console.WriteLine("");


        return;

    }


    static void Test_Grid(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }

        var reader = new NetTopologySuite.IO.WKTReader();   // Change to WKBReader for WKB

        Polygon polygon = (Polygon) reader.Read(wkt);

        Console.WriteLine(String.Format("Test_Grid: polygon: {0}", polygon.ToString()));


        Grid grid = new Grid(polygon, seed);


        List<Coordinate> sample = grid.Sample(gridsizeX, gridsizeY);

        Console.WriteLine(String.Format("Test_Grid: Count: {1}  sample: {0}", Print.MultiPointWKT(sample), sample.Count));

        Console.WriteLine("");

        return;

    }



    static void Test_KDTree(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }

        var reader = new NetTopologySuite.IO.WKTReader();   // Change to WKBReader for WKB

        Polygon polygon = (Polygon) reader.Read(wkt);

        Grid grid = new Grid(polygon, seed);

        List<Coordinate> sample = grid.Sample(gridsizeX, gridsizeY);


        // Print Grid

        Console.WriteLine(String.Format("Test_KDTree: polygon: {0}", polygon.ToString()));
        Console.WriteLine(String.Format("Test_KDTree: grid: {0}", Print.MultiPointWKT(sample)));


        // Construct KDTree

        List<SampleUnit> samplepts = SampleUnit.CreateSampleList(sample, null);

        KDTreeSampleUnit tree = new KDTreeSampleUnit(samplepts, null);


        /* Search through nearest neighbours. */

        SampleUnit query = samplepts[(int) samplepts.Count/2];

        Console.WriteLine(String.Format("Test_KDTree: SearchNN: query: {0}", query.ToString()));

        List<SampleUnit> neighbours = new List<SampleUnit>();

        for (int i = 0; i < 6; i++){

            tree.SearchNN(query, true, out SampleUnit neighbour);

            neighbour.Exclude();

            neighbours.Add(neighbour);

            Console.WriteLine(
                String.Format("Test_KDTree: SearchNN: i: {0}  neighbour: {1}  Distance: {2}", 
                    i, neighbour.ToString(), query.Distance(neighbour))
            );

        }

        Console.WriteLine(String.Format("Test_KDTree: SearchNN All Neighbours: {0}", Print.MultiPointWKT<SampleUnit>(neighbours)));

        Console.WriteLine("");

        return;

    }


    static void Test_SpatiallyBalanced(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input polygon");
        }

        if(numplots is null){
            throw new ArgumentException("Number of plots has not been provided");
        }


        // Read polygon and create triangulation

        var reader = new NetTopologySuite.IO.WKTReader();   // Change to WKBReader for WKB

        Polygon polygon = (Polygon) reader.Read(wkt);

        Console.WriteLine(String.Format("Test_SpatiallyBalanced: polygon: {0}", polygon.ToString()));


        // Randomly generate points inside the polygon, convert to a 'SampleUnit'.

        Simple srs = new Simple(polygon, null);

        List<Coordinate> candidates = srs.Sample(NumCandidates, null);



        // Generate sample using the Local Pivotal Method

        SpatiallyBalanced lpm = new SpatiallyBalanced(seed, null);

        List<Coordinate> sample = lpm.Sample(candidates, (int) numplots, null);


        // Print output

        Console.WriteLine(String.Format("Test_SpatiallyBalanced: numplots: {0}  candidates: {1}  sample: {2}", numplots, candidates.Count, Print.MultiPointWKT(sample)));


        Console.WriteLine("");


        return;

    }

}