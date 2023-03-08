using NetTopologySuite.Geometries;
using RandomPlotGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpatialFunction
{
    /// <summary>
    /// Summary description for RandomPlotGen
    /// </summary>
    public class RandomPlotGen : IHttpHandler
    {
        static string wkt;
        static int? numplots;
        static string ExampleWKT = "POLYGON ((513820.1 7017121, 513833.4 7017112, 513843.6 7017102, 513843.6 7017060, 513841.9 7017065, 513837.8 7017077, 513828 7017101, 513822.9 7017113, 513820.1 7017121))";
        static int ExampleNumPlots = 10;
        static int NumCandidates = 1000;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["poly"]))
                wkt = HttpContext.Current.Request.QueryString["poly"];
            else
                wkt = "";

            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["nrplots"]))
            {
                if (int.TryParse(HttpContext.Current.Request.QueryString["nrplots"], out _))
                    numplots = Convert.ToInt32(HttpContext.Current.Request.QueryString["nrplots"]);
                else
                    numplots= 0;
            }
            else
                numplots = 0;

            try
            {
                if (wkt == "" || numplots == 0)
                {
                    context.Response.Write(
                        "Usage: " + Environment.NewLine +
                        "RandomPlotGen.ashx?" +
                        "poly=" + ExampleWKT +
                        "&nrplots=" + ExampleNumPlots
                        );
                }
                else
                {
                    // Change to WKBReader for WKB
                    var reader = new NetTopologySuite.IO.WKTReader();
                    //wkt = ExampleWKT;
                    Polygon polygon = (Polygon)reader.Read(wkt);


                    // Randomly generate points inside the polygon, convert to a 'Point'.
                    Simple srs = new Simple(polygon, null);
                    List<Coordinate> candidates = srs.Sample(NumCandidates, null);


                    // Generate sample using the Local Pivotal Method
                    SpatiallyBalanced lpm = new SpatiallyBalanced(null, null);
                    //numplots = ExampleNumPlots;
                    List<Coordinate> sample = lpm.Sample(
                        candidates, 
                        (int)numplots, 
                        NumCandidates*10);


                    // Return WKT for the sample
                    context.Response.Write(Print.MultiPointWKT(sample));
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("An error occurred: " + ex.Message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}