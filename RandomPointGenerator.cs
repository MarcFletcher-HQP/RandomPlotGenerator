
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class RandomPointGenerator {

    private readonly Random rand;


    public RandomPointGenerator(int? seed) {

        if (seed is not null){
            rand = new Random((int) seed);
        } else {
            rand = new Random();
        }

    }


    public Point PointInsideBox(double xmin, double ymin, double xmax, double ymax){

        double x = rand.NextDouble() * (xmax - xmin) + xmin;
        double y = rand.NextDouble() * (xmax - xmin) + xmin;

        return new Point(x, y);

    }


    public List<Point> RandomGrid(double xmin, double ymin, double xmax, double ymax, double dx, double dy) {

        // Generate a random starting point and create a grid

        Point start = PointInsideBox(xmin, ymin, xmin + dx, ymin + dy);

        int n = Convert.ToInt32(Math.Truncate((xmax - start.GetX()) / dx));
        int m = Convert.ToInt32(Math.Truncate((ymax - start.GetY()) / dy));

        List<Point> grid = new List<Point>();

        for( int i = 0; i < n; i++ ){

            for( int j = 0; j < m; j++ ){

                grid.Add(new Point(xmin + i * dx, ymin + j * dy));

            }

        }

        return grid;

    }

}