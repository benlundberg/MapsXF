using Esri.ArcGISRuntime.UI;
using System.Linq;

namespace Esri.Core.Extensions
{
    public static class GraphicsOverlayExtensions
    {
        public static Graphic GetGraphicById(this GraphicsOverlay graphicsOverlay, string id)
        {
            return graphicsOverlay.Graphics.FirstOrDefault(x => x.GetId() == id);
        }
    }
}
