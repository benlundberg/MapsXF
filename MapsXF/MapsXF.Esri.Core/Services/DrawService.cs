using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using Esri.Core.Factories;
using Esri.Core.Helpers;
using Esri.Core.Providers;
using MapsXF.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Esri.Core.Services
{
    public static class DrawService
    {
        public static async Task DrawGeometriesAsync()
        {
            // Load items from sqlite
            var items = await DatabaseRepository.Current.LoadAllAsync<GeometryItem>();

            if (items?.Any() != true)
            {
                return;
            }

            foreach (var item in items)
            {
                // Get geometry from json
                Geometry geometry = Geometry.FromJson(item.GeometryJson);

                if (geometry == null)
                {
                    continue;
                }

                // Create graphic
                Graphic graphic = GraphicFactory.Current.CreateGraphic(geometry, ColorHelper.TryFromName(item.Color), item.Id);

                if (graphic == null)
                {
                    continue;
                }

                OverlayProvider.Current.GeometryOverlay.Graphics.Add(graphic);
            }
        }
    }
}
