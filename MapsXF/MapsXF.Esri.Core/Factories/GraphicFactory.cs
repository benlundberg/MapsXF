using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using Esri.Core.Extensions;
using Esri.Core.Providers;
using System;
using System.Drawing;

namespace Esri.Core.Factories
{
    public class GraphicFactory
    {
        private static readonly Lazy<GraphicFactory> implementation = new Lazy<GraphicFactory>(() => Create(), isThreadSafe: true);

        public static GraphicFactory Current
        {
            get
            {
                var ret = implementation.Value;

                if (ret == null)
                {
                    throw new NotImplementedException();
                }

                return ret;
            }
        }

        private static GraphicFactory Create()
        {
            return new GraphicFactory();
        }

        public Graphic CreateGraphic(Geometry geometry, Color color, object id)
        {
            var graphic = new Graphic
            {
                Geometry = geometry,
                Symbol = SymbolProvider.GetSymbol(geometry.GeometryType, color)
            };

            graphic.SetId(id?.ToString());

            return graphic;
        }
    }
}
