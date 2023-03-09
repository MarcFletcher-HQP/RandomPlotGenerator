

using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

using RandomPlotGenerator;


class Program{

    /* Program Inputs */

    static string? wkt;
    static int? numplots;


    /* Example Inputs */
    static string ExampleWKT = "POLYGON ((513820.1 7017121, 513833.4 7017112, 513843.6 7017102, 513843.6 7017060, 513841.9 7017065, 513837.8 7017077, 513828 7017101, 513822.9 7017113, 513820.1 7017121))";
    static string ExampleWKB = "0103000000010000000900000066666666705C1F4100000040A8C45A419A999999A55C1F4100000000A6C45A4166666666CE5C1F4100000080A3C45A4166666666CE5C1F410000000099C45A419A999999C75C1F41000000409AC45A4133333333B75C1F41000000409DC45A4100000000905C1F4100000040A3C45A419A9999997B5C1F4100000040A6C45A4166666666705C1F4100000040A8C45A41";
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

                string? response = Workflow_Survival();

                Console.WriteLine(response);

            }

        }


        /* Run some tests instead */

        if(args.Contains("Test")){

            if(testall || args.Contains("Geometry")){

                Test_Geometry();

            }

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



    /* Import Geometry */

    static Geometry wkt_to_geometry(string wkt){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input AOI");
        }


        // Read aoi and create triangulation

        var reader = new WKTReader();

        Geometry geom = reader.Read(wkt);

        if(geom is not Polygon && geom is not MultiPolygon){

            throw new ArgumentException("Input geometry must be either a Polygon, or a MultiPolygon!");

        }

        return geom;

    }



    static Geometry wkb_to_geometry(string wkb){

        if(wkb is null){
            throw new ArgumentException("No WKB provided for input");
        }


        // Read aoi and create triangulation

        var reader = new WKBReader();

        byte[] hex = WKBReader.HexToBytes(wkb);

        Geometry geom = reader.Read(hex);

        if(geom is not Polygon && geom is not MultiPolygon){

            throw new ArgumentException("Input geometry must be either a Polygon, or a MultiPolygon!");

        }

        return geom;

    }




    /* Workflows */

    static string? Workflow_Survival(){

        // Check arguments

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input AOI");
        }

        if(numplots is null){
            throw new ArgumentException("Number of plots was not provided");
        }

        Geometry aoi = wkt_to_geometry(wkt);


        // Randomly generate points inside the aoi, convert to a 'Point'.

        Simple srs = new Simple(aoi, null);

        List<Coordinate> candidates = srs.Sample(NumCandidates, null);


        // Generate sample using the Local Pivotal Method

        SpatiallyBalanced lpm = new SpatiallyBalanced(null, null);

        List<Coordinate> sample = lpm.Sample(candidates, (int) numplots, null);


        // Return WKT for the sample

        return Print.MultiPointWKT(sample);

    }





    /* Tests */

    static void Test_Geometry(){

        Geometry poly = wkt_to_geometry(ExampleWKT);

        Console.WriteLine(String.Format("Test_Geometry:  Input WKT: {0}", ExampleWKT));
        Console.WriteLine(String.Format("Test_Geometry: Output WKT: {0}", poly.ToString()));


        poly = wkb_to_geometry(ExampleWKB);

        Console.WriteLine(String.Format("Test_Geometry:  Input WKB: {0}", ExampleWKB));
        Console.WriteLine(String.Format("Test_Geometry: Output WKB: {0}", WKBWriter.ToHex(poly.ToBinary())));

        Console.WriteLine("");

        return;

    }




    static void Test_Simple(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input AOI");
        }


        // Print input before any possible errors occur, due to Simple();

        Console.WriteLine(String.Format("Test_Simple: Input WKT: {0}", wkt));


        Geometry aoi = wkt_to_geometry(wkt);

        Simple srs = new Simple(aoi, null);


        // Print diagnostics

        Console.WriteLine(String.Format("Test_Simple: Polygon: {0}", aoi.ToString()));

        Console.WriteLine(String.Format("Test_Simple: {0}", srs.Print(2)));

        Console.WriteLine("");


        return;

    }



    static void Test_Weighted(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input AOI");
        }


        /* Deliberately ignoring the default number of candidates, bit hard to see what's 
        going on when you print 1,000 points! */

        Geometry aoi = wkt_to_geometry(wkt);

        Simple srs = new Simple(aoi, null);

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
            throw new ArgumentException("No WKT provided for input AOI");
        }

        Geometry aoi = wkt_to_geometry(wkt);

        Console.WriteLine(String.Format("Test_Grid: aoi: {0}", aoi.ToString()));


        Grid grid = new Grid(aoi, seed);


        List<Coordinate> sample = grid.Sample(gridsizeX, gridsizeY, aoi);

        Console.WriteLine(String.Format("Test_Grid: Count: {1}  sample: {0}", Print.MultiPointWKT(sample), sample.Count));

        Console.WriteLine("");

        return;

    }



    static void Test_KDTree(){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input AOI");
        }

        Geometry aoi = wkt_to_geometry(wkt);

        Grid grid = new Grid(aoi, seed);

        List<Coordinate> sample = grid.Sample(gridsizeX, gridsizeY, null);


        // Print Grid

        Console.WriteLine(String.Format("Test_KDTree: aoi: {0}", aoi.ToString()));
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
            throw new ArgumentException("No WKT provided for input AOI");
        }

        if(numplots is null){
            throw new ArgumentException("Number of plots has not been provided");
        }


        // Read AOI and create triangulation

        Geometry aoi = wkt_to_geometry(wkt);

        Console.WriteLine(String.Format("Test_SpatiallyBalanced: aoi: {0}", aoi.ToString()));


        // Randomly generate points inside the aoi, convert to a 'SampleUnit'.

        Simple srs = new Simple(aoi, null);

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