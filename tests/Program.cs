

#define DEBUG
#undef DEBUG


using RandomPlotGenerator;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

using Point = RandomPlotGenerator.Point;


class Program {

    static int Repeats = 100;
    static int SampleSize = 5;
    static double GridWidth = 1;
    static double[] AOI = {0, 0, 9, 14};


    static void Main(string[] args){

        if (args.Contains("LPM")){

            Test_LPM();

        } else if (args.Contains("SearchNN")){

            Test_SearchNN();

        } else if (args.Contains("RandomTri")){

            Test_RandomTri();

        }

        return;

    }



    static void Test_LPM(){

        RandomPointGenerator rpg = new RandomPointGenerator(null);

        LocalPivotalMethod lpm = new LocalPivotalMethod(null, null);


        for (int i = 0; i < Repeats; i++){

            List<Point> grid = rpg.RandomGrid(AOI[0], AOI[1], AOI[2], AOI[3], GridWidth, GridWidth);

            List<Point> sample = lpm.SamplePoints(grid, SampleSize, null);

            PrintMultiPoint(in sample);

        }

        return;

    }




    static void Test_SearchNN(){

        RandomPointGenerator rpg = new RandomPointGenerator(null);

        List<Point> grid = rpg.RandomGrid(AOI[0], AOI[1], AOI[2], AOI[3], GridWidth, GridWidth);

        Random rand = new Random();


        Point point = new Point(1, 4);

        Point result = new Point(0, 0);

        KDTree tree = new KDTree(grid, null);

        tree.Find(point, null, out point);
        tree.SearchNN(point, true, out result);

        Console.WriteLine(String.Format("SearchNN: nearest neighbour of {0} is {1}", point.Print(), result.Print()));


        Console.WriteLine(String.Format("SearchNN: Marking point as Excluded and trying again..."));

        result.Exclude();

        tree.SearchNN(point, true, out result);

        Console.WriteLine(String.Format("SearchNN: nearest neighbour of {0} is {1}", point.Print(), result.Print()));


        result.Exclude();

        tree.SearchNN(point, true, out result);

        Console.WriteLine(String.Format("SearchNN: nearest neighbour of {0} is {1}", point.Print(), result.Print()));


        result.Exclude();

        tree.SearchNN(point, true, out result);

        Console.WriteLine(String.Format("SearchNN: nearest neighbour of {0} is {1}", point.Print(), result.Print()));


        // should have run out of nearest neighbours by here
        result.Exclude();

        tree.SearchNN(point, true, out result);

        Console.WriteLine(String.Format("SearchNN: nearest neighbour of {0} is {1}", point.Print(), result.Print()));

    }



    static void Test_RandomTri(){

        // Polygon WKT
        string wkt = "POLYGON ((513820.1 7017121, 513833.4 7017112, 513843.6 7017102, 513843.6 7017060, 513841.9 7017065, 513837.8 7017077, 513828 7017101, 513822.9 7017113, 513820.1 7017121))";


        // Create triangulation

        Triangulation tri = new Triangulation(wkt, null);


        // Generate the sample

        List<Coordinate> sample = tri.GenerateRandomPoints(100);
        List<Point> samplePt = new List<Point>();

        foreach(Coordinate coord in sample){
            samplePt.Add(new Point(coord[0], coord[1]));
        }


        // Print

        PrintMultiPoint(in samplePt);


        return;
    }




    private static void PrintMultiPoint(in List<Point> sample){

        string buff = "";

        for(int i = 0; i < sample.Count; i++){

            buff += String.Format("({0} {1})", sample[i].X, sample[i].Y);

            if (i < sample.Count - 1){
                buff += ", ";
            }

        }

        Console.WriteLine(String.Format("MULTIPOINT({0})", buff));

        return;

    }


}
































