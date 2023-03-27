﻿

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

    static string ExampleWKT = "MULTIPOLYGON (((-3114702.32330586 17030769.0108994, -3114717.54898509 17030774.2524889, -3114733.05461509 17030778.9212284, -3114748.58186283 17030785.5835886, -3114763.24135443 17030791.4012565, -3114780.47646509 17030797.1900926, -3114792.52966791 17030800.1885944, -3114805.45065685 17030804.0317885, -3114819.83938638 17030811.2765723, -3114830.50779996 17030818.5628783, -3114842.89941664 17030826.3996591, -3114852.11247658 17030831.423619, -3114863.89804012 17030836.1341033, -3114869.69605654 17030837.0979074, -3114871.92631321 17030837.4686014, -3114884.17974572 17030832.4898371, -3114891.56512315 17030827.2805302, -3114897.51947649 17030822.0872533, -3114903.72330404 17030813.4732398, -3114910.7332354 17030800.0081455, -3114913.48142111 17030789.4389165, -3114913.06301261 17030777.1961102, -3114910.37363639 17030766.6876616, -3114912.22020233 17030752.1409871, -3114915.29127838 17030744.9860382, -3114925.13543262 17030728.9258639, -3114934.74248275 17030717.4255587, -3114945.26651911 17030711.3265864, -3114961.53267977 17030706.8724709, -3114974.6723311 17030704.4471532, -3114987.22739341 17030700.888937, -3114997.19446169 17030696.2204202, -3115009.45099988 17030691.5261886, -3115021.3998894 17030684.8416758, -3115026.49574793 17030679.6578611, -3115037.78313359 17030664.7206778, -3115044.54690391 17030654.9609641, -3115053.6031442 17030645.4604022, -3115072.92817907 17030632.7119828, -3115090.31450251 17030625.9666895, -3115110.83040175 17030617.4773538, -3115123.08092059 17030612.2135004, -3115178.56050129 17030581.4012672, -3115191.91574425 17030572.4222371, -3115201.27050271 17030564.0574679, -3115209.48676272 17030556.2752336, -3115216.55515754 17030548.2209345, -3115224.74982508 17030538.4449682, -3115233.50151401 17030527.2386577, -3115242.52403777 17030514.6052313, -3115251.84187017 17030502.8228424, -3115261.1535904 17030490.470943, -3115270.74226505 17030477.261327, -3115274.39178929 17030470.6693207, -3115281.12470697 17030458.0613865, -3115286.44828674 17030447.4629917, -3115292.63355602 17030437.1398901, -3115298.24632563 17030426.8230223, -3115303.84686079 17030415.3670222, -3115308.30563904 17030404.2086904, -3115311.90285486 17030392.7750655, -3115315.47534845 17030379.0630646, -3115318.49082294 17030366.7815191, -3115319.53081841 17030357.0854802, -3115319.69364158 17030345.6903718, -3115321.01670473 17030335.7063493, -3115320.94777959 17030333.3950227, -3115320.56719289 17030320.6152112, -3115319.98432588 17030316.3494483, -3115318.39118116 17030304.688843, -3115315.0611734 17030287.9210107, -3115312.56513525 17030268.8651177, -3115311.20448873 17030248.9420458, -3115310.10842979 17030227.0221248, -3115311.03404893 17030206.7885829, -3115313.13838739 17030189.6749923, -3115317.82154987 17030172.8173252, -3115324.79410318 17030155.9340546, -3115331.20052432 17030139.6266397, -3115339.02228812 17030121.8793073, -3115346.31812644 17030108.4103168, -3115355.31562469 17030093.4980692, -3115362.64530981 17030083.1618318, -3115369.12878976 17030073.9744116, -3115378.69244009 17030058.4859742, -3115386.88070901 17030048.1401634, -3115389.40210794 17030044.6839158, -3115394.21340732 17030038.0886813, -3115402.68787614 17030027.7395309, -3115414.0303792 17030017.927831, -3115432.45962493 17030001.7702521, -3115464.51023508 17029964.8480261, -3115469.91741935 17029960.2464123, -3115476.19067473 17029956.4917172, -3115481.03159698 17029952.7491559, -3115491.56166061 17029943.83291, -3115497.26191569 17029940.0831129, -3115506.39099434 17029935.1653518, -3115516.38165522 17029930.5251101, -3115533.51629631 17029923.5466026, -3115548.3614074 17029916.8722198, -3115558.92948081 17029912.7965906, -3115569.47521102 17029905.8735202, -3115574.85109408 17029897.2854441, -3115575.08159793 17029890.1644476, -3115572.41144959 17029878.5116346, -3115567.48096658 17029870.8644309, -3115559.68117724 17029862.6719843, -3115553.05201525 17029857.6020494, -3115547.5821161 17029854.2309613, -3115538.37247196 17029848.8979784, -3115529.74471059 17029844.6992299, -3115523.99062501 17029841.6152347, -3115517.37933568 17029838.8232306, -3115505.24971871 17029828.0704356, -3115487.36803944 17029838.3566906, -3115485.58145856 17029839.1862434, -3115483.18365959 17029839.7574237, -3115392.58042226 17029847.4599532, -3115389.56131082 17029847.1760885, -3115298.04879047 17029820.9259507, -3115277.54959995 17029811.8820213, -3115205.0050409 17029779.6658267, -3115188.3521473 17029772.8137781, -3115106.1636651 17029738.9964753, -3115002.18505315 17029713.6041655, -3114939.94231916 17029711.8135915, -3114898.88145651 17029710.632269, -3114795.66689353 17029724.1751757, -3114711.5239128 17029737.7210879, -3114711.78461839 17029739.8459543, -3114712.43738126 17029747.2440251, -3114713.08714755 17029754.3572292, -3114715.50654086 17029766.292571, -3114718.77510104 17029777.363851, -3114721.18200943 17029788.1599492, -3114727.08185214 17029804.3285486, -3114732.92601013 17029815.3708855, -3114738.83197587 17029832.1091068, -3114747.31666332 17029849.1031402, -3114752.59771396 17029861.0063108, -3114759.30953065 17029872.8934513, -3114769.4988124 17029888.7290941, -3114779.37108301 17029901.7200787, -3114819.69779519 17029978.1696295, -3114828.26768591 17029976.6493393, -3114833.68648758 17029974.879582, -3114844.81027671 17029971.3369505, -3114857.33725454 17029965.2150464, -3114862.50085287 17029966.2962927, -3114878.87467251 17029971.8090565, -3114892.16869928 17029983.6225035, -3114900.57294398 17029993.2122323, -3114911.29748121 17030005.6242442, -3114920.57251688 17030016.343532, -3114927.53962933 17030025.3797803, -3114933.37450594 17030035.5680741, -3114937.20599008 17030045.7788544, -3114937.86475672 17030053.7465469, -3114937.97288635 17030063.7142054, -3114938.11497818 17030076.8146171, -3114938.81694679 17030088.7694398, -3114938.41439488 17030104.4393279, -3114935.69155108 17030117.2870443, -3114929.82071356 17030130.170049, -3114926.14668103 17030134.4836793, -3114915.08138737 17030143.4372172, -3114907.11164936 17030147.5141823, -3114896.00004735 17030152.1959461, -3114887.73789669 17030155.7064062, -3114876.91873319 17030160.9545636, -3114865.51485688 17030165.0699339, -3114857.81884069 17030168.004427, -3114850.39677205 17030169.7965595, -3114843.54394482 17030171.2974802, -3114837.26947296 17030172.1182388, -3114836.92123811 17030172.1638797, -3114837.05483843 17030175.9688913, -3114835.41767273 17030198.2616212, -3114833.7796332 17030214.0296931, -3114831.59495649 17030229.797765, -3114826.13045686 17030245.0225978, -3114818.47853775 17030252.6355151, -3114808.64070106 17030265.6859443, -3114799.89564401 17030275.4738219, -3114795.52392912 17030286.8924186, -3114792.83669796 17030293.2682425, -3114728.82838186 17030281.3347931, -3114727.23967409 17030293.9351575, -3114722.81192001 17030307.9410419, -3114717.2394595 17030321.9596167, -3114709.11613206 17030338.2853993, -3114701.52807944 17030351.1873282, -3114690.81069405 17030365.8331884, -3114680.62858618 17030377.0551949, -3114667.89874544 17030390.8690532, -3114651.72244746 17030403.5821843, -3114640.89709749 17030408.2604972, -3114629.77334456 17030411.8027947, -3114618.30149584 17030413.4226047, -3114616.7666144 17030505.2179926, -3114609.21432094 17030511.6581591, -3114592.5819843 17030526.4633174, -3114579.88955564 17030545.6220697, -3114570.69864875 17030560.4264488, -3114561.94549884 17030576.1015689, -3114555.81823472 17030586.987168, -3114553.63110399 17030603.0969907, -3114551.88122484 17030614.8528855, -3114551.88247342 17030628.3502625, -3114554.94772743 17030640.9765644, -3114559.32598511 17030651.4256797, -3114563.96519671 17030661.7369815, -3114565.30704238 17030662.9237586, -3114565.3592329 17030662.9527017, -3114577.3217297 17030669.5809981, -3114586.27616006 17030677.1709836, -3114592.09491347 17030685.9353898, -3114597.05826719 17030694.9941247, -3114600.56303125 17030701.5057583, -3114613.23237978 17030716.4518471, -3114624.35512079 17030731.4308864, -3114636.80199317 17030744.3935959, -3114644.31322299 17030750.860479, -3114654.978507 17030757.8619184, -3114665.06220776 17030764.0154372, -3114675.97060734 17030767.0265181, -3114690.2885345 17030767.7211517, -3114702.32330586 17030769.0108994)) , ((-3114835.18268603 17030397.944854, -3114856.68698215 17030409.0291583, -3114874.39230674 17030380.5063218, -3114906.50969577 17030399.2703351, -3114890.81196406 17030429.2153895, -3114902.2789155 17030434.0442064, -3114923.98284177 17030399.2460675, -3114956.95963848 17030418.0089677, -3114800.20991609 17030687.5341603, -3114744.86255367 17030653.4126213, -3114759.13160597 17030625.1799947, -3114715.83292238 17030600.7314515, -3114835.18268603 17030397.944854)))";
    static string ExampleWKB = "0103000000010000000900000066666666705C1F4100000040A8C45A419A999999A55C1F4100000000A6C45A4166666666CE5C1F4100000080A3C45A4166666666CE5C1F410000000099C45A419A999999C75C1F41000000409AC45A4133333333B75C1F41000000409DC45A4100000000905C1F4100000040A3C45A419A9999997B5C1F4100000040A6C45A4166666666705C1F4100000040A8C45A41";
    static int ExampleNumPlots = 80;


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