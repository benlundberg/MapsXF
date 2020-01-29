using Esri.ArcGISRuntime.Geometry;
using System;
using System.Diagnostics;

namespace Esri.Core.Extensions
{
    public static class GeometryExtensions
    {
        public static decimal GetArea(this Geometry geometry)
        {
            try
            {
                var projectArea = GeometryEngine.Area(GeometryEngine.Simplify(geometry as Polygon));

                double projectAreaHa = AreaUnits.Hectares.FromSquareMeters(projectArea);
                decimal insideItemArea = Convert.ToDecimal(Math.Round(projectAreaHa, 2, MidpointRounding.AwayFromZero));

                return insideItemArea;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return 0;
        }

        public static int GetMeters(this Geometry geometry)
        {
            try
            {
                var projectDistance = GeometryEngine.Length(GeometryEngine.Simplify(geometry as Polyline));

                double projectDistanceMeter = LinearUnits.Meters.FromMeters(projectDistance);
                decimal insideItemArea = Convert.ToDecimal(Math.Round(projectDistanceMeter, 2, MidpointRounding.AwayFromZero));

                return Convert.ToInt32(Math.Truncate(insideItemArea));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return 0;
        }
    }
}
