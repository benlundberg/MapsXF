using Esri.ArcGISRuntime.UI;
using System;

namespace Esri.Core.Providers
{
    public class OverlayProvider
    {
        private static readonly Lazy<OverlayProvider> implementation = new Lazy<OverlayProvider>(() => Create(), isThreadSafe: true);

        public static OverlayProvider Current
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

        private static OverlayProvider Create()
        {
            return new OverlayProvider();
        }

        public void Init()
        {
            geometryOverlay = null;
        }

        private GraphicsOverlay geometryOverlay;
        
        /// <summary>
        /// Overlay that holds all drawned geometries
        /// </summary>
        public GraphicsOverlay GeometryOverlay => geometryOverlay ?? (geometryOverlay = new GraphicsOverlay
        {
            Id = "GeometryOverlay",
            RenderingMode = GraphicsRenderingMode.Dynamic
        });
    }
}
