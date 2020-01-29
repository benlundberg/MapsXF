using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Esri.Core.Services
{
    public class BasemapService
    {
        public Basemap GetDefaultBasemap()
        {
            return Basemap.CreateImagery();
        }

        public Basemap GetBasemap(BasemapType basemapType)
        {
            Basemap basemap = null;

            switch (basemapType)
            {
                case BasemapType.Imagery:
                    basemap = Basemap.CreateImagery();
                    break;
                case BasemapType.ImageryWithLabels:
                    basemap = Basemap.CreateImageryWithLabels();
                    break;
                case BasemapType.Streets:
                    basemap = Basemap.CreateStreets();
                    break;
                case BasemapType.Topographic:
                    basemap = Basemap.CreateTopographic();
                    break;
                case BasemapType.TerrainWithLabels:
                    basemap = Basemap.CreateTerrainWithLabels();
                    break;
                case BasemapType.LightGrayCanvas:
                    basemap = Basemap.CreateLightGrayCanvas();
                    break;
                case BasemapType.NationalGeographic:
                    basemap = Basemap.CreateNationalGeographic();
                    break;
                case BasemapType.Oceans:
                    basemap = Basemap.CreateOceans();
                    break;
                case BasemapType.OpenStreetMap:
                    basemap = Basemap.CreateOpenStreetMap();
                    break;
                case BasemapType.ImageryWithLabelsVector:
                    basemap = Basemap.CreateImageryWithLabelsVector();
                    break;
                case BasemapType.StreetsVector:
                    basemap = Basemap.CreateStreetsVector();
                    break;
                case BasemapType.TopographicVector:
                    basemap = Basemap.CreateTopographicVector();
                    break;
                case BasemapType.TerrainWithLabelsVector:
                    basemap = Basemap.CreateTerrainWithLabelsVector();
                    break;
                case BasemapType.LightGrayCanvasVector:
                    basemap = Basemap.CreateLightGrayCanvasVector();
                    break;
                case BasemapType.NavigationVector:
                    basemap = Basemap.CreateNavigationVector();
                    break;
                case BasemapType.StreetsNightVector:
                    basemap = Basemap.CreateStreetsVector();
                    break;
                case BasemapType.StreetsWithReliefVector:
                    basemap = Basemap.CreateStreetsWithReliefVector();
                    break;
                case BasemapType.DarkGrayCanvasVector:
                    basemap = Basemap.CreateDarkGrayCanvasVector();
                    break;
                default:
                    break;
            }

            return basemap;
        }

        public async Task<Basemap> GetTopoBasemapAsync()
        {
            var wmtsUri = new Uri(ServiceConfig.SwedishTopographic);

            WmtsService myWmtsService = new WmtsService(wmtsUri);

            // Load the WMTS service.
            await myWmtsService.LoadAsync();

            // Get the service information (i.e. metadata) about the WMTS service.
            WmtsServiceInfo myWmtsServiceInfo = myWmtsService.ServiceInfo;

            // Obtain the read only list of WMTS layer info objects.
            IReadOnlyList<WmtsLayerInfo> myWmtsLayerInfos = myWmtsServiceInfo.LayerInfos;

            // Create a WMTS layer using the first item in the read only list of WMTS layer info objects.
            var myWmtsLayer = new WmtsLayer(myWmtsLayerInfos.LastOrDefault());

            return new Basemap(myWmtsLayer);
        }
    }
}
