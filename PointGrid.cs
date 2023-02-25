

using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;



public class PointGrid {

    private List<Point> grid;
    private readonly int rows;
    private readonly int cols;
    private readonly double dx;
    private readonly double dy;
    private readonly QuadTree index;



    public PointGrid(double xstart, double ystart, double dx, double dy, int nrow, int ncol) {

        this.dx = dx;
        this.dy = dy;
        this.rows = nrow;
        this.cols = ncol;

        this.grid = new List<Point>();

        for( int i = 0; i < nrow; i++ ){

            for( int j = 0; j < ncol; j++ ){

                grid.Add(new Point(xstart + j * dx, ystart + i * dy));

            }

        }

        this.index = new QuadTree(grid, 4 * dx * dy);

        return;

    }



}