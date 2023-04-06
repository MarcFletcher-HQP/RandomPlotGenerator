using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using RandomPlotGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Services;

namespace SpatialFunction
{
    /// <summary>
    /// Summary description for RandomPlotGen1
    /// </summary>
    [WebService(Namespace = "RandomPointGen",
        Description = "This WebService generates random points based on a polygon input and a number of points.")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RandomPlotGen1 : System.Web.Services.WebService
    {
        //public string polygonWKT = "";
        //public string nrPlots = "0";

        [WebMethod]
        public string ReturnPlots(String polygonWKT, String nrPlots)
        {
            int NumCandidates = 1000;

            try
            {

                Geometry aoi = GeocortexReader.wkt_to_geometry(polygonWKT);

                if (aoi is MultiPolygon && GeocortexReader.check_for_interiors((MultiPolygon)aoi))
                {

                    aoi = GeocortexReader.remove_interiors((MultiPolygon)aoi);

                }

                // Randomly generate points inside the aoi, convert to a 'Point'.

                Simple srs = new Simple(aoi, null);

                List<Coordinate> candidates = srs.Sample(NumCandidates, null);


                // Generate sample using the Local Pivotal Method

                SpatiallyBalanced lpm = new SpatiallyBalanced(null, null);

                List<Coordinate> sample = lpm.Sample(candidates, Convert.ToInt32(nrPlots), null);


                // Return WKT for the sample
                return (Print.MultiPointWKT(sample));
            }
            catch (Exception ex)
            {
                return ("An error occurred: " + ex.Message);
            }
        }


     }


}
