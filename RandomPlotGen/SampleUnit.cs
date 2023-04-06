

using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator
{

    /* Class: SampleUnit

    SampleUnit extends NetTopologySuite's Coordinate class and is used to represent sampling units 
    within the polygon; that will either be accepted or rejected by the chosen sampling method 
    (currently just the Local Pivotal Method).

    The Local Pivotal Method has been implemented in this library by storing the candidate 
    sampling units in a KD-tree. The SampleUnit class has likewise been developed to meet this need.
     */
    public class SampleUnit : Coordinate
    {

        /* public Coordinate point; */
        public SampleUnitStatus status;
        public double prob;


        public enum SampleUnitStatus
        {
            Undefined,
            Excluded,
            Selected
        }


        /* Constructor for when you don't know the initial probability */
        public SampleUnit(Coordinate point) : base(point)
        {

            /* this.point = point;     // As far as I know, this just copies the reference. */
            this.prob = 0.0;
            this.status = SampleUnitStatus.Undefined;

        }


        /* Constructor for when you do */
        public SampleUnit(Coordinate point, double? prob) : base(point)
        {

            if (prob is null)
            {
                prob = 0.0;
            }

            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Probability can not be less than 0.0 or greater than 1.0.");
            }

            /* this.point = point; */
            this.prob = (double)prob;
            this.status = SampleUnitStatus.Undefined;

        }


        /* Sometimes we don't know what the initial probability should be, we don't have all of the points yet. */
        public void SetInitialProb(double prob)
        {

            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Probability can not be less than 0.0 or greater than 1.0.");
            }

            this.prob = prob;

        }


        public bool IsSelected()
        {

            return status == SampleUnitStatus.Selected;

        }

        public bool IsExcluded()
        {

            return status == SampleUnitStatus.Excluded;

        }

        public bool IsUndefined()
        {

            return status == SampleUnitStatus.Undefined;

        }


        /* public double this[int i]{
            get{ return (i == 0) ? this.point.X : this.point.Y; }
            // No 'set' method - can't think of a good reason why this class should change the coordinates.
        } */


        /* KD-Tree construction and querying rarely requires the actual distance
        Using the squared-distance saves us calculating a sqrt. */
        public double Distance2(double x, double y)
        {

            return Math.Pow(x - this[0], 2) + Math.Pow(y - this[1], 2);

        }

        public double Distance2(Coordinate xy)
        {

            return Math.Pow(xy[0] - this[0], 2) + Math.Pow(xy[1] - this[1], 2);

        }

        public double Distance2(SampleUnit xy)
        {

            return Math.Pow(xy[0] - this[0], 2) + Math.Pow(xy[1] - this[1], 2);

        }

        public void Select()
        {

            prob = 1.0;
            status = SampleUnitStatus.Selected;

        }

        public void Exclude()
        {

            prob = 0.0;
            status = SampleUnitStatus.Excluded;

        }


        /* WKT for a single point */
        public string Print()
        {

            return String.Format("POINT({0} {1}):  prob: {2}  status: {3}", this[0], this[1], this.prob, this.status);

        }


        /* WKT for a single point */
        public override string ToString()
        {

            return String.Format("POINT({0} {1})", this[0], this[1]);

        }


        public static List<SampleUnit> CreateSampleList(List<Coordinate> points, double? prob)
        {

            List<SampleUnit> sample = new List<SampleUnit>();

            foreach (Coordinate pt in points)
            {

                sample.Add(new SampleUnit(pt, prob));

            }

            return sample;

        }


        public static List<Coordinate> CreateCoordinateList(List<SampleUnit> sample)
        {

            List<Coordinate> points = new List<Coordinate>();

            foreach (SampleUnit pt in sample)
            {

                points.Add(pt);

            }

            return points;

        }

    }
}