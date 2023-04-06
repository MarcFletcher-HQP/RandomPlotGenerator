

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;

namespace RandomPlotGenerator
{

    interface Point2D
    {

    }


    public class Print
    {

        static public string MultiPointWKT<T>(List<T> points) where T : Coordinate
        {

            Point[] pointarr = new Point[points.Count];

            for (int i = 0; i < points.Count; i++)
            {

                pointarr[i] = new Point((Coordinate)points[i]);

            }

            MultiPoint multipoint = new MultiPoint(pointarr);

            return multipoint.ToString();

        }


        static public string MultiPointWKB<T>(List<T> points) where T : Coordinate
        {

            Point[] pointarr = new Point[points.Count];

            for (int i = 0; i < points.Count; i++)
            {

                pointarr[i] = new Point((Coordinate)points[i]);

            }

            MultiPoint multipoint = new MultiPoint(pointarr);

            return WKBWriter.ToHex(multipoint.ToBinary());

        }


        static public string List<T>(List<T> array)
        {

            string buff = "";

            foreach (T x in array)
            {

                buff += x;
                buff += ", ";

            }

            char[] cleanup = { ',', ' ' };

            buff = buff.TrimEnd(cleanup);

            return buff;

        }


        static public string Array<T>(T[] array)
        {

            string buff = "";

            foreach (T x in array)
            {

                buff += x;
                buff += ", ";

            }

            char[] cleanup = { ',', ' ' };

            buff = buff.TrimEnd(cleanup);

            return buff;

        }

    }
}