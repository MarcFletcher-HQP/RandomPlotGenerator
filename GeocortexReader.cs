

using NetTopologySuite.IO;
using NetTopologySuite.Geometries;


namespace RandomPlotGenerator;


/* ESRI API for Geocortex assumes data is only going to be used for rendering 
    maps, as such the orientation of each ring in a polygon is ignored and all
    WKT/WKB constructions will result in stacked polygons, rather than holes. 
*/

public class GeocortexReader{


    public static Geometry wkt_to_geometry(string wkt){

        if(wkt is null){
            throw new ArgumentException("No WKT provided for input AOI");
        }

        var reader = new WKTReader();

        return reader.Read(wkt);

    }


    public static Geometry wkb_to_geometry(string wkb){

        if(wkb is null){
            throw new ArgumentException("No WKB provided for input");
        }

        var reader = new WKBReader();

        byte[] hex = WKBReader.HexToBytes(wkb);

        return reader.Read(hex);;

    }



    /* Use the Shoelace Formula to determine whether two rings have the same orientation */
    public static bool same_orientation(LineString ring1, LineString ring2){

        double area1 = 0.0;
        double area2 = 0.0;

        for(int i = 0; i < ring1.Count - 1; i++){
            
            area1 += (ring1[i].X - ring1[i+1].X) * (ring1[i].Y + ring1[i+1].Y);

        }

        for(int i = 0; i < ring2.Count - 1; i++){

            area2 += (ring2[i].X - ring2[i+1].X) * (ring2[i].Y + ring2[i+1].Y);

        }

        return Math.Sign(area1) == Math.Sign(area2);

    }



    /* At least one bounding-box has to be contained within another
        for it to be worth checking any further. */
    public static bool check_for_interiors(MultiPolygon multi){

        for(int i = 0; i < multi.Count; i++){

            Geometry box = multi[i].Envelope;

            for(int j = i+1; j < multi.Count; j++){

                if(box.Contains(multi[j].Envelope)){
                    return true;
                }

            }

        }

        return false;

    }



    public static MultiPolygon remove_interiors(MultiPolygon multi){

        List<Polygon> polygons = new List<Polygon>();

        for(int i = 0; i < multi.Count; i++){

            List<LinearRing> holes = new List<LinearRing>();

            bool AddPolygon = true;

            for(int j = 1; j < multi.Count; j++){

                if(i == j){
                    continue;
                }

                if (multi[i].Within(multi[j])){

                    AddPolygon = false;
                    holes.Clear();
                    break;

                }

                if(multi[i].Contains(multi[j])){

                    LinearRing boundary = (LinearRing) ((Polygon) multi[j]).ExteriorRing;

                    boundary = (LinearRing) boundary.Reverse(); // Polygon constructor doesn't check the orientation of interior rings.

                    holes.Add(boundary);

                }

            }

            if(AddPolygon && holes.Count > 0){

                Polygon exterior = (Polygon) multi[i];

                LinearRing shell = (LinearRing) exterior.ExteriorRing;

                Polygon swisscheese = new Polygon(shell, holes.ToArray());

                polygons.Add(swisscheese);

            } else if (AddPolygon){

                polygons.Add((Polygon) multi[i]);

            }

        }

        return new MultiPolygon((Polygon[]) polygons.ToArray());

    }


}
