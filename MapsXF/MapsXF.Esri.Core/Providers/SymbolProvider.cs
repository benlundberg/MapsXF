using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Drawing;

namespace Esri.Core.Providers
{
    public static class SymbolProvider
    {
        public static Symbol GetSymbol(GeometryType geometryType, Color color, bool fill = false)
        {
            if (geometryType == GeometryType.Point)
            {
                return new SimpleMarkerSymbol
                {
                    Color = color,
                    Style = SimpleMarkerSymbolStyle.Circle,
                };
            }
            else if (geometryType == GeometryType.Polyline)
            {
                return new SimpleLineSymbol
                {
                    Color = color,
                    Width = 3,
                    Style = SimpleLineSymbolStyle.Solid
                };
            }
            else
            {
                return new SimpleFillSymbol
                {
                    Color = color,
                    Style = fill ? SimpleFillSymbolStyle.Solid : SimpleFillSymbolStyle.Null,
                    Outline = new SimpleLineSymbol
                    {
                        Color = color,
                        Width = 2,
                        Style = SimpleLineSymbolStyle.Solid
                    }
                };
            }
        }
    }
}
