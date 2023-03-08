

using NetTopologySuite.Geometries;
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

            string buff = "";

            foreach (T pt in points)
            {

                buff += String.Format("({0} {1})", pt.X, pt.Y);
                buff += ", ";

            }

            char[] cleanup = { ',', ' ' };

            buff = buff.TrimEnd(cleanup);

            return String.Format("MULTIPOINT({0})", buff);

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