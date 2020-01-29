using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Security;
using Esri.Core.Models;
using MapsXF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Esri.Core.Services
{
    public class LayerConstansts
    {
        public const string SlopeLayerId = "1";
        public const string ForestDataId = "2";
        public const string MarktackeDataId = "3";
        public const string OsiId = "4";
    }

    public class LayerService : INotifyPropertyChanged
    {
        public LayerService(Map map)
        {
            this.map = map;
        }

        public async Task<bool> InitLayersAsync()
        {
            try
            {
                MapLayers = new List<LayerItem>
                {
                    new LayerItem
                    {
                        Layer = await GetSlopeLayerAsync(),
                        FontFamily = App.Current.FontAwesomeSolid(),
                        Text = "\uf6fc",
                        LayerVisibilityChanged = LayerVisibilityChanged
                    },
                    new LayerItem
                    {
                        Layer = await GetForestDataLayerAsync(),
                        FontFamily = App.Current.FontAwesomeSolid(),
                        Text = "\uf1bb",
                        LayerVisibilityChanged = LayerVisibilityChanged
                    },
                    new LayerItem
                    {
                        Layer = await GetMarktackeLayerAsync(),
                        FontFamily = App.Current.FontAwesomeSolid(),
                        Text = "\uf1ce",
                        LayerVisibilityChanged = LayerVisibilityChanged
                    },
                    new LayerItem
                    {
                        Layer = await GetOsiLayerAsync(),
                        FontFamily = App.Current.FontAwesomeBrands(),
                        Text = "\uf41a",
                        LayerVisibilityChanged = LayerVisibilityChanged
                    }
                };

                // Adding layer to map
                foreach (var layer in MapLayers)
                {
                    this.map.Basemap.BaseLayers.Add(layer.Layer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in InitLayers {ex.Message}");
            }

            return false;
        }

        private void LayerVisibilityChanged()
        {
            AnyLayerIsVisible = MapLayers?.Any(x => x?.IsVisible == true) == true;
        }

        public void ChangeLayerOpacity(double opacity)
        {
            if (!MapLayers.Any(x => x.IsVisible))
            {
                return;
            }

            MapLayers.Where(x => x.IsVisible)?.ForEach(x => x.Layer.Opacity = opacity);
        }

        private async Task<Layer> GetSlopeLayerAsync()
        {
            try
            {
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CredentialService.CreateCredential);

                var uri = new Uri("https://geodata.skogsstyrelsen.se/arcgis/rest/services/Publikt/Lutning/ImageServer");

                ImageServiceRaster imageServiceRaster = new ImageServiceRaster(uri);

                await imageServiceRaster.LoadAsync();

                RasterLayer rasterLayer = new RasterLayer(imageServiceRaster)
                {
                    Id = LayerConstansts.SlopeLayerId,
                    IsVisible = false
                };

                return rasterLayer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private async Task<Layer> GetMarktackeLayerAsync()
        {
            try
            {
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CredentialService.CreateCredential);

                var uri = new Uri("https://geodata.skogsstyrelsen.se/arcgis/rest/services/Publikt/Marktackedata_2_0/ImageServer");

                ImageServiceRaster imageServiceRaster = new ImageServiceRaster(uri);

                await imageServiceRaster.LoadAsync();

                RasterLayer rasterLayer = new RasterLayer(imageServiceRaster)
                {
                    Id = LayerConstansts.MarktackeDataId,
                    IsVisible = false
                };

                return rasterLayer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }
        
        private async Task<Layer> GetOsiLayerAsync()
        {
            try
            {
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CredentialService.CreateCredential);

                var uri = new Uri("https://geodata.skogsstyrelsen.se/arcgis/rest/services/Publikt/OSIraster/ImageServer");

                ImageServiceRaster imageServiceRaster = new ImageServiceRaster(uri);

                await imageServiceRaster.LoadAsync();

                RasterLayer rasterLayer = new RasterLayer(imageServiceRaster)
                {
                    Id = LayerConstansts.OsiId,
                    IsVisible = false
                };

                return rasterLayer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private async Task<Layer> GetForestDataLayerAsync()
        {
            try
            {
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CredentialService.CreateCredential);

                //var uri = new Uri("https://geodata.holmen.com/arcgis/rest/services/Baskartor/Fj%C3%A4llkartan/MapServer");

                //var layer = new ArcGISMapImageLayer(uri)
                //{
                //    Id = LayerConstansts.ForestDataId
                //};

                //await layer.LoadAsync();

                //return layer;

                var uri = new Uri("https://geodata.skogsstyrelsen.se/arcgis/rest/services/Publikt/SkogligaGrunddata_1_2/ImageServer");

                ImageServiceRaster imageServiceRaster = new ImageServiceRaster(uri);

                await imageServiceRaster.LoadAsync();

                RasterLayer rasterLayer = new RasterLayer(imageServiceRaster)
                {
                    Id = LayerConstansts.ForestDataId,
                    IsVisible = false
                };

                return rasterLayer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private ICommand layerOpacityChangedCommand;
        public ICommand LayerOpacityChangedCommand => layerOpacityChangedCommand ?? (layerOpacityChangedCommand = new Command((param) =>
        {
            ChangeLayerOpacity(LayerOpacity);
        }));

        public List<LayerItem> MapLayers { get; private set; }

        public bool AnyLayerIsVisible { get; private set; }
        public double LayerOpacity { get; set; } = 1d;

        private readonly Map map;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
