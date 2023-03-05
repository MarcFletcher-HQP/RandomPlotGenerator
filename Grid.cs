
#define DEBUG
#undef DEBUG

using NetTopologySuite.Geometries;

namespace RandomPlotGenerator;



/* Class: Grid

Sample a polygon by creating a grid of points, with a random start location.

*/

public class Grid {

    private Random rng;
    private Envelope bbox;


    public Grid(in Polygon polygon, int? seed){

        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

        bbox = polygon.EnvelopeInternal;

    }


    public List<Coordinate> Sample(double dx, double dy) {

        double xmin = bbox.MinX;
        double ymin = bbox.MinY;
        double xmax = bbox.MaxX;
        double ymax = bbox.MaxY;


        // Generate a random starting point and create a grid

        double x = rng.NextDouble() * dx + xmin;
        double y = rng.NextDouble() * dy + ymin;

        Coordinate start = new Coordinate(x, y);


        int n = Convert.ToInt32(Math.Truncate((xmax - start.X) / dx)) + 2;
        int m = Convert.ToInt32(Math.Truncate((ymax - start.Y) / dy)) + 2;

        List<Coordinate> grid = new List<Coordinate>();

        for( int i = 0; i < n; i++ ){

            for( int j = 0; j < m; j++ ){

                grid.Add(new Coordinate(start.X + i * dx, start.Y + j * dy));

            }

        }

        return grid;

    }


    public string Print(){

        return String.Format("Grid.Print: Envelope: {0}", bbox.ToString());

    }



}


