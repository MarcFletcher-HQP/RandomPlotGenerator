

using RandomPlotGenerator;
using System;


class Program {

    static int Repeats = 1;
    static int SampleSize = 10;
    static int NumPoints = 1000;
    static double GridWidth = 0.1;
    static double[] AOI = {0, 0, 10, 15};


    static void Main(string[] args){

        if (args.Contains("LPM")){

            Test_LPM();

        }

        return;

    }



    static void Test_LPM(){

        RandomPointGenerator rpg = new RandomPointGenerator(null);

        LocalPivotalMethod lpm = new LocalPivotalMethod(null, null);


        for (int i = 0; i < Repeats; i++){

            List<Point> grid = rpg.RandomGrid(AOI[0], AOI[1], AOI[2], AOI[3], GridWidth, GridWidth);

            List<Point> sample = lpm.SamplePoints(grid, SampleSize);

            PrintMultiPoint(in sample);

        }

        return;

    }




    static void PrintMultiPoint(in List<Point> sample){

        String buff = "MULTIPOINT(";

        for(int i = 0; i < sample.Count; i++){

            buff += ($"({sample[i].GetX()}, {sample[i].GetY()})");

            if (i < sample.Count - 1){
                buff += ", ";
            }

        }

        buff += ")";

        Console.WriteLine(buff);

        return;

    }

}
































