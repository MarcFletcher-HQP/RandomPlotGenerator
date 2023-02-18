
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class RandomPointGenerator {

    private readonly Random rand;


    public RandomPointGenerator() {

        rand = new Random()

    }


    public RandomPointGenerator(int seed) {

        rand = new Random(seed);

    }


    public Point PointInsideBox(double xmin, double ymin, double xmax, double ymax){

        double x = rand.NextDouble() * (xmax - xmin) + xmin;
        double y = rand.NextDouble() * (xmax - xmin) + xmin;

        return Point(x, y);

    }


    public List<Point> RandomGrid(double xmin, double ymin, double xmax, double ymax, double dx, double dy) {

        // Generate a random starting point and create a grid

        Point start = PointInsideBox(xmin, ymin, xmin + dx, ymin + dy);

        int n = (int) (xmax - start.x) / dx;
        int m = (int) (ymax - start.y) / dy;

        List<Point> grid = new List<Point>();

        for( int i = 0; i < n; i++ ){

            for( int j = 0; j < m; j++ ){

                grid.Add(Point(xmin + i * dx, ymin + j * dy));

            }

        }

        return grid;

    }

}