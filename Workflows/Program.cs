

#define DEBUG
#undef DEBUG

using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

using RandomPlotGenerator;


class Program{

    /* Program Inputs */

    static string? wkt;
    static int? numplots;


    /* Example Inputs */

    static string ExampleWKT = "POLYGON ((-2981357.75959235 17008002.7078413, -2981360.18802362 17008000.6885057, -2981382.32671075 17007983.897519, -2981434.05812414 17007943.8307399, -2981431.70959373 17007932.1878344, -2981428.40136424 17007902.3712427, -2981425.6918651 17007872.3926812, -2981426.39067164 17007831.1580494, -2981110.83535214 17007887.6359931, -2981080.82804729 17007893.0066019, -2981077.60797266 17007913.8325862, -2981074.06701706 17007949.3850259, -2981070.117229 17007986.6369803, -2981065.6711519 17008024.9149665, -2981060.30263498 17008062.2425068, -2981059.33219773 17008066.162734, -2981194.48707916 17008063.567988, -2981226.41497902 17008068.8072398, -2981344.24989674 17008013.9422043, -2981357.75959235 17008002.7078413))";
    static string ExampleWKB = "0103000000010000000900000066666666705C1F4100000040A8C45A419A999999A55C1F4100000000A6C45A4166666666CE5C1F4100000080A3C45A4166666666CE5C1F410000000099C45A419A999999C75C1F41000000409AC45A4133333333B75C1F41000000409DC45A4100000000905C1F4100000040A3C45A419A9999997B5C1F4100000040A6C45A4166666666705C1F4100000040A8C45A41";
    static int ExampleNumPlots = 1000;


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

            if(testall || args.Contains("Polygon")){

                Test_Polygon();

            }

            if(testall || args.Contains("Rings")){

                Test_Rings();

            }
            

        }


        return;

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

        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

        #if DEBUG

        Console.WriteLine(String.Format("Workflow_Survival: Is MultiPolygon: {0}  Has Interiors: {1}", aoi is MultiPolygon, GeocortexReader.check_for_interiors((MultiPolygon) aoi)));

        #endif

        if(aoi is MultiPolygon && GeocortexReader.check_for_interiors((MultiPolygon) aoi)){

            aoi = GeocortexReader.remove_interiors((MultiPolygon) aoi);

        }


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

        Geometry poly = GeocortexReader.wkt_to_geometry(ExampleWKT);

        Console.WriteLine(String.Format("Test_Geometry:  Input WKT: {0}", ExampleWKT));
        Console.WriteLine(String.Format("Test_Geometry: Output WKT: {0}", poly.ToString()));


        poly = GeocortexReader.wkb_to_geometry(ExampleWKB);

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


        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

        Simple srs = new Simple(aoi, 117);


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

        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

        Simple srs = new Simple(aoi, 117);

        srs.SelectionProbability(out double[] prob);

        Weighted wrs = new Weighted(prob, seed);


        // Check the alias table

        for(int i = 0; i < prob.Length; i++){
            prob[i] = Math.Round(prob[i], 4);
        }

        Console.WriteLine("Test_Weighted: Prob: {0}", Print.Array<double>(prob));

        Console.WriteLine("Test_Weighted: {0}", wrs.Print(4));


        // Randomly pick triangles

        int candidates = 1000;

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

        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

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

        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

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

        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

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



    static void Test_Rings(){

        LineString shell = (LineString) GeocortexReader.wkt_to_geometry("LINESTRING (0 0, 0 4, 4 4, 4 0, 0 0)");
        LineString hole = (LineString) GeocortexReader.wkt_to_geometry("LINESTRING (1 1, 1 3, 3 3, 3 1, 1 1)");

        hole = (LineString) hole.Reverse();

        double shellarea = 0.0;
        double holearea = 0.0;

        for(int i = 0; i < shell.Count - 1; i++){
            
            shellarea += (shell[i].X - shell[i+1].X) * (shell[i].Y + shell[i+1].Y);

        }

        for(int i = 0; i < hole.Count - 1; i++){

            holearea += (hole[i].X - hole[i+1].X) * (hole[i].Y + hole[i+1].Y);

        }

        Console.WriteLine(String.Format("Test_Rings: Shell Area: {0}  Hole Area: {1}", shellarea, holearea));
        
        return;

    }



    static void Test_Polygon(){

        Polygon shell = (Polygon) GeocortexReader.wkt_to_geometry("POLYGON ((0 0, 0 4, 4 4, 4 0, 0 0))");
        Polygon hole = (Polygon) GeocortexReader.wkt_to_geometry("POLYGON ((1 1, 1 3, 3 3, 3 1, 1 1))");

        MultiPolygon multi = new MultiPolygon(new Polygon[] {shell, hole});

        Geometry reconstructed = GeocortexReader.remove_interiors(multi);

        Point test = new Point(new Coordinate(2, 2));

        Console.WriteLine(String.Format("Test_Polygon: MultiPolygon contains holes: {0}", GeocortexReader.check_for_interiors(multi)));
        Console.WriteLine(String.Format("Test_Polygon: POINT(2 2) in MultiPolygon: {0}", multi.Contains(test)));
        Console.WriteLine(String.Format("Test_Polygon: Reconstructed MultiPolygon WKT: {0}", reconstructed.ToString()));
        Console.WriteLine(String.Format("Test_Polygon: POINT(2 2) in reconstructed: {0}", reconstructed.Contains(test)));

        return;

    }

}