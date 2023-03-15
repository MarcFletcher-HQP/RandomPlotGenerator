

using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

using RandomPlotGenerator;


class Program{

    /* Program Inputs */

    static string? wkt;
    static int? numplots;


    /* Example Inputs */

    static string ExampleWKT = "MULTIPOLYGON (((17031905 -3122933, 17031899 -3122951, 17031889 -3122968, 17031879 -3122985, 17031874 -3122995, 17031871 -3123003, 17031867 -3123019, 17031867 -3123023, 17031868 -3123040, 17031873 -3123068, 17031874 -3123075, 17031877 -3123087, 17031872 -3123088, 17031872 -3123088, 17031861 -3123090, 17031855 -3123091, 17031844 -3123091, 17031827 -3123084, 17031809 -3123078, 17031802 -3123076, 17031780 -3123070, 17031763 -3123069, 17031749 -3123069, 17031732 -3123073, 17031717 -3123075, 17031708 -3123077, 17031704 -3123078, 17031701 -3123079, 17031690 -3123085, 17031677 -3123092, 17031675 -3123093, 17031656 -3123102, 17031654 -3123103, 17031637 -3123112, 17031607 -3123132, 17031596 -3123142, 17031584 -3123149, 17031576 -3123157, 17031573 -3123161, 17031572 -3123167, 17031576 -3123178, 17031578 -3123182, 17031582 -3123188, 17031589 -3123197, 17031594 -3123201, 17031597 -3123204, 17031599 -3123209, 17031600 -3123213, 17031600 -3123214, 17031602 -3123227, 17031606 -3123237, 17031621 -3123265, 17031629 -3123290, 17031632 -3123297, 17031636 -3123308, 17031639 -3123322, 17031639 -3123322, 17031642 -3123324, 17031644 -3123325, 17031639 -3123324, 17031634 -3123323, 17031593 -3123318, 17031578 -3123318, 17031579 -3123310, 17031580 -3123294, 17031581 -3123274, 17031577 -3123249, 17031570 -3123223, 17031562 -3123210, 17031562 -3123209, 17031553 -3123200, 17031538 -3123191, 17031519 -3123183, 17031353 -3123156, 17031342 -3123135, 17031339 -3123137, 17031327 -3123142, 17031305 -3123153, 17031295 -3123160, 17031283 -3123167, 17031273 -3123172, 17031265 -3123178, 17031257 -3123182, 17031256 -3123182, 17031247 -3123186, 17031242 -3123188, 17031237 -3123191, 17031211 -3123204, 17031201 -3123211, 17031190 -3123220, 17031168 -3123234, 17031148 -3123245, 17031138 -3123252, 17031130 -3123256, 17031122 -3123262, 17031116 -3123268, 17031114 -3123271, 17031110 -3123277, 17031106 -3123286, 17031092 -3123334, 17031075 -3123331, 17031089 -3123266, 17031080 -3123247, 17031066 -3123241, 17031060 -3123240, 17030969 -3123224, 17030955 -3123231, 17030952 -3123233, 17030933 -3123258, 17030918 -3123275, 17030918 -3123275, 17030905 -3123292, 17030904 -3123295, 17030899 -3123300, 17030893 -3123303, 17030885 -3123302, 17030878 -3123300, 17030874 -3123300, 17030875 -3123298, 17030875 -3123289, 17030877 -3123281, 17030897 -3123224, 17030899 -3123220, 17030902 -3123213, 17030905 -3123205, 17030908 -3123194, 17030910 -3123190, 17030913 -3123182, 17030916 -3123174, 17030918 -3123166, 17030920 -3123158, 17030922 -3123151, 17030924 -3123143, 17030925 -3123135, 17030927 -3123127, 17030929 -3123119, 17030930 -3123111, 17030931 -3123103, 17030933 -3123095, 17030935 -3123087, 17030937 -3123079, 17030939 -3123071, 17030942 -3123064, 17030945 -3123056, 17030950 -3123046, 17030952 -3123042, 17030956 -3123035, 17030961 -3123028, 17030965 -3123021, 17030968 -3123015, 17030972 -3123008, 17030977 -3123002, 17030982 -3122996, 17030988 -3122990, 17030995 -3122986, 17031000 -3122981, 17031008 -3122977, 17031015 -3122973, 17031123 -3122919, 17031133 -3122914, 17031138 -3122912, 17031145 -3122909, 17031151 -3122905, 17031159 -3122901, 17031165 -3122896, 17031172 -3122892, 17031178 -3122888, 17031185 -3122883, 17031192 -3122879, 17031199 -3122875, 17031205 -3122870, 17031212 -3122867, 17031218 -3122862, 17031226 -3122857, 17031231 -3122853, 17031238 -3122849, 17031244 -3122844, 17031250 -3122840, 17031258 -3122835, 17031283 -3122818, 17031288 -3122813, 17031294 -3122808, 17031300 -3122804, 17031306 -3122799, 17031311 -3122795, 17031318 -3122791, 17031323 -3122788, 17031336 -3122779, 17031336 -3122779, 17031355 -3122766, 17031375 -3122756, 17031397 -3122754, 17031419 -3122752, 17031458 -3122755, 17031468 -3122755, 17031486 -3122758, 17031510 -3122762, 17031534 -3122768, 17031556 -3122774, 17031575 -3122780, 17031593 -3122786, 17031600 -3122788, 17031627 -3122795, 17031655 -3122802, 17031685 -3122809, 17031713 -3122816, 17031741 -3122822, 17031763 -3122825, 17031784 -3122826, 17031812 -3122827, 17031830 -3122827, 17031838 -3122828, 17031858 -3122830, 17031876 -3122836, 17031889 -3122846, 17031898 -3122860, 17031903 -3122874, 17031905 -3122892, 17031907 -3122913, 17031905 -3122933)), ((17031422 -3123069, 17031425 -3123074, 17031456 -3123066, 17031527 -3123039, 17031531 -3123037, 17031546 -3123033, 17031547 -3123032, 17031589 -3123009, 17031612 -3122997, 17031615 -3122996, 17031648 -3122984, 17031684 -3122971, 17031691 -3122967, 17031720 -3122953, 17031722 -3122951, 17031725 -3122945, 17031720 -3122937, 17031718 -3122934, 17031689 -3122934, 17031687 -3122935, 17031673 -3122937, 17031582 -3122982, 17031576 -3122985, 17031507 -3123020, 17031480 -3123032, 17031478 -3123033, 17031473 -3123036, 17031444 -3123051, 17031439 -3123054, 17031419 -3123061, 17031418 -3123066, 17031422 -3123069)), ((17032852 -3123022, 17032824 -3123040, 17032797 -3123059, 17032771 -3123081, 17032742 -3123105, 17032718 -3123127, 17032716 -3123130, 17032693 -3123158, 17032689 -3123166, 17032570 -3123072, 17032570 -3123072, 17032559 -3123077, 17032550 -3123080, 17032541 -3123085, 17032535 -3123085, 17032513 -3123086, 17032514 -3123085, 17032520 -3123073, 17032522 -3123052, 17032524 -3123040, 17032516 -3123022, 17032512 -3123025, 17032510 -3123026, 17032508 -3123028, 17032503 -3123033, 17032499 -3123038, 17032495 -3123043, 17032491 -3123046, 17032484 -3123059, 17032475 -3123074, 17032474 -3123076, 17032468 -3123083, 17032465 -3123085, 17032461 -3123086, 17032420 -3123106, 17032410 -3123105, 17032407 -3123104, 17032401 -3123101, 17032395 -3123102, 17032389 -3123105, 17032383 -3123107, 17032377 -3123107, 17032359 -3123106, 17032353 -3123107, 17032347 -3123107, 17032341 -3123109, 17032315 -3123130, 17032315 -3123130, 17032280 -3123112, 17032278 -3123099, 17032277 -3123075, 17032287 -3123073, 17032314 -3123066, 17032344 -3123057, 17032378 -3123047, 17032410 -3123036, 17032445 -3123024, 17032472 -3123015, 17032491 -3123006, 17032508 -3122995, 17032524 -3122983, 17032540 -3122969, 17032554 -3122954, 17032568 -3122943, 17032585 -3122932, 17032606 -3122923, 17032622 -3122917, 17032624 -3122916, 17032643 -3122911, 17032659 -3122909, 17032661 -3122909, 17032675 -3122910, 17032689 -3122913, 17032692 -3122913, 17032699 -3122914, 17032737 -3122926, 17032766 -3122935, 17032783 -3122941, 17032854 -3122963, 17032948 -3122993, 17032952 -3122994, 17032935 -3122995, 17032909 -3122999, 17032881 -3123008, 17032852 -3123022)))";
    static string ExampleWKB = "0103000000010000000900000066666666705C1F4100000040A8C45A419A999999A55C1F4100000000A6C45A4166666666CE5C1F4100000080A3C45A4166666666CE5C1F410000000099C45A419A999999C75C1F41000000409AC45A4133333333B75C1F41000000409DC45A4100000000905C1F4100000040A3C45A419A9999997B5C1F4100000040A6C45A4166666666705C1F4100000040A8C45A41";
    static int ExampleNumPlots = 20;


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

        Geometry aoi = GeocortexReader.wkt_to_geometry(wkt);

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