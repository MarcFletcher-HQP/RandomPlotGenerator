

#define DEBUG
#undef DEBUG


using RandomPlotGenerator;
using System;


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

        } else if (args.Contains("Find")){

            Test_Find();

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


    static void Test_Find(){

        RandomPointGenerator rpg = new RandomPointGenerator(null);

        List<Point> grid = rpg.RandomGrid(AOI[0], AOI[1], AOI[2], AOI[3], GridWidth, GridWidth);

        Random rand = new Random();

        int index = rand.Next(0, grid.Count - 1);


        Point point = new Point(2, 13);
        Point result = point;

        KDTree tree = new KDTree();
        tree.Build(grid, null);

        Console.WriteLine(String.Format("Find: KD-Tree \n{0}", tree.Print(true)));



        tree.Find(point, null, out result);

        Console.WriteLine(String.Format("Find: nearest neighbour of {0} is {1}", point.Print(), result.Print()));




        return;

    }



    static void Test_SearchNN(){

        RandomPointGenerator rpg = new RandomPointGenerator(null);

        List<Point> grid = rpg.RandomGrid(AOI[0], AOI[1], AOI[2], AOI[3], GridWidth, GridWidth);

        Random rand = new Random();


        Point point = new Point(1, 4);

        Point result = new Point(0, 0);

        KDTree tree = new KDTree();
        tree.Build(grid, null);

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




    static void PrintMultiPoint(in List<Point> sample){

        String buff = "MULTIPOINT(";

        for(int i = 0; i < sample.Count; i++){

            buff += ($"({sample[i].GetX()} {sample[i].GetY()})");

            if (i < sample.Count - 1){
                buff += ", ";
            }

        }

        buff += ")";

        Console.WriteLine(buff);

        return;

    }

}
































