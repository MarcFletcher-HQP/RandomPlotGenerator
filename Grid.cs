
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
    public enum Type {
        Rectangular,
        Hexagonal
    };


    public Grid(in Geometry aoi, int? seed){

        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }

        bbox = aoi.EnvelopeInternal;

    }


    public List<Coordinate> Sample(double dx, double dy, Geometry? aoi, Type type) {

        if(aoi is not null && aoi is not Polygon && aoi is not MultiPolygon){

            throw new ArgumentException("Input geometry must be either a Polygon, or a MultiPolygon!");

        }

        if(dy > 0 & (type == Type.Hexagonal)){

            Console.WriteLine(String.Format("Grid::Sample: Argument 'dy' ignored when 'type' is 'Hexagonal'."));

        }


        double xmin = bbox.MinX;
        double ymin = bbox.MinY;
        double xmax = bbox.MaxX;
        double ymax = bbox.MaxY;


        if(type == Type.Hexagonal){
            dy = Math.Sqrt(3) / 2 * dx;
        }


        // Generate a random starting point and create a grid

        double x = rng.NextDouble() * dx + xmin;
        double y = rng.NextDouble() * dy + ymin;

        Coordinate start = new Coordinate(x, y);

        List<Coordinate> grid = new List<Coordinate>();


        if(type == Type.Rectangular){

            int n = Convert.ToInt32(Math.Truncate((xmax - start.X) / dx)) + 2;
            int m = Convert.ToInt32(Math.Truncate((ymax - start.Y) / dy)) + 2;

            for( int i = 0; i < n; i++ ){

                for( int j = 0; j < m; j++ ){

                    grid.Add(new Coordinate(start.X + i * dx, start.Y + j * dy));

                }

            }

        } else if (type == Type.Hexagonal){

            int n = Convert.ToInt32(Math.Truncate((xmax - start.X) / dx)) + 2;
            int m = Convert.ToInt32(Math.Truncate((ymax - start.Y) / (2*dy))) + 2;

            for( int i = 0; i < n; i++ ){

                for( int j = 0; j < m; j++ ){

                    grid.Add(new Coordinate(start.X + i * dx, start.Y + j * 2 * dy));
                    grid.Add(new Coordinate(start.X + dx/2 + i * dx, start.Y + dy + j * 2 * dy));

                }

            }

        } else {

            throw new ArgumentException("How did you provide an invalid Type?");
        }


        if(aoi is not null){

            grid_intersecting_aoi(grid, aoi);

        }

        return grid;

    }



    private static void grid_intersecting_aoi(List<Coordinate> grid, Geometry aoi){

        List<int> index = new List<int>();

        for(int i = grid.Count - 1; i >= 0; i--){

            Point point = new Point(grid[i]);

            if(!point.Intersects(aoi)){

                grid.RemoveAt(i);

            }

        }


        return;

    }



    public string Print(){

        return String.Format("Grid.Print: Envelope: {0}", bbox.ToString());

    }



}


