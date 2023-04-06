using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpatialFunction
{
    public partial class PlotTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonGenerate_Click(object sender, EventArgs e)
        {
            SpatialFunctionWS.RandomPlotGen1 plotgen = new SpatialFunctionWS.RandomPlotGen1();
            String polygonWKT = TextBoxPolygonWKT.Text;
            String nrPlots = TextBoxNrPlots.Text;

            TextBoxResult.Text = plotgen.ReturnPlots(polygonWKT, nrPlots);


        }
    }
}