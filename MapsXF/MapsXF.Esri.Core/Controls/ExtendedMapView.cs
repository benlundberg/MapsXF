using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Xamarin.Forms;
using Esri.Core.Models;
using Esri.Core.Providers;
using Esri.Core.Services;
using MapsXF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Esri.Core.Controls
{
    public class ExtendedMapView : MapView, IDisposable
    {
        public ExtendedMapView()
        {
            InteractionOptions = new MapViewInteractionOptions
            {
                AllowMagnifierToPan = true,
                IsPanEnabled = true,
                IsMagnifierEnabled = true,
                IsEnabled = true,
                IsFlickEnabled = true,
                IsRotateEnabled = true,
                IsZoomEnabled = true,
            };

            this.PropertyChanged += ExtendedMapView_PropertyChanged;
            this.GeoViewTapped += ExtendedMapView_GeoViewTapped;
            this.SpatialReferenceChanged += ExtendedMapView_SpatialReferenceChanged;
            this.GeoViewHolding += ExtendedMapView_GeoViewHolding;
        }

        private void ExtendedMapView_GeoViewHolding(object sender, GeoViewInputEventArgs e)
        {
            MapHoldCommand?.Execute(e.Location);
        }

        private async void ExtendedMapView_SpatialReferenceChanged(object sender, EventArgs e)
        {
            try
            {
                await SetGpsModeAsync(GpsMode.On);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void ExtendedMapView_GeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                var kmlLayer = this.Map.OperationalLayers.FirstOrDefault(x => x.Id == "KML");

                // Perform identify on the KML layer and get the results.
                IdentifyLayerResult identifyResult = await IdentifyLayerAsync(kmlLayer, e.Position, 2, false);

                // Return if there are no results that are KML placemarks.
                if (!identifyResult.GeoElements.OfType<KmlGeoElement>().Any())
                {
                    return;
                }

                // Get the first identified feature that is a KML placemark
                KmlNode firstIdentifiedPlacemark = identifyResult.GeoElements.OfType<KmlGeoElement>().First().KmlNode;

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }

            var graphics = await this.IdentifyGraphicsOverlaysAsync(e.Position, tolerance: 10d, returnPopupsOnly: false);

            if (graphics?.SelectMany(x => x?.Graphics)?.Any() == true)
            {
                this.MapClickCommand?.Execute(graphics?.SelectMany(x => x.Graphics)?.FirstOrDefault());
            }
            else
            {
                this.MapClickCommand?.Execute(null);
            }
        }

        private void ExtendedMapView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Map != null && MapLoadedCommand?.CanExecute(this) == true)
            {
                AddOverlays();

                MapLoadedCommand?.Execute(this);

                this.PropertyChanged -= ExtendedMapView_PropertyChanged;
            }
        }

        private void AddOverlays()
        {
            OverlayProvider.Current.Init();

            this.GraphicsOverlays.Add(OverlayProvider.Current.GeometryOverlay);
        }

        public async Task ShowOfflineMapsAsync(OfflineMapService offlineMapService, IEnumerable<OfflineMapItem> offlineMaps, List<string> deletedMaps = null)
        {
            if (offlineMaps?.Any() == true)
            {
                foreach (var offlineMap in offlineMaps)
                {
                    var layer = Map.Basemap.BaseLayers.FirstOrDefault(x => x.Id == offlineMap.Path);

                    if (layer == null)
                    {
                        if (!offlineMap.IsVisible)
                        {
                            continue;
                        }

                        layer = await offlineMapService.LoadLayerAsync(offlineMap.Path);

                        if (layer != null)
                        {
                            Map.Basemap.BaseLayers.Add(layer);
                        }
                    }
                    else
                    {
                        layer.IsVisible = offlineMap.IsVisible;
                    }
                }
            }

            if (deletedMaps?.Any() == true)
            {
                foreach (var path in deletedMaps)
                {
                    var layer = Map.Basemap.BaseLayers.FirstOrDefault(x => x.Id == path);

                    if (layer == null)
                    {
                        continue;
                    }

                    Map.Basemap.BaseLayers.Remove(layer);
                }
            }
        }

        public async Task SetGpsModeAsync(GpsMode gpsMode)
        {
            if (LocationDisplay == null)
            {
                return;
            }

            switch (gpsMode)
            {
                case GpsMode.Off:
                case GpsMode.On:
                    LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Off;
                    break;
                case GpsMode.ReCenter:
                    LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
                    break;
                case GpsMode.Navigation:
                    LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Navigation;
                    break;
                case GpsMode.Compass:
                    LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.CompassNavigation;
                    break;
                default:
                    LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Off;
                    break;
            }

            if (!LocationDisplay.DataSource.IsStarted)
            {
                await LocationDisplay.DataSource.StartAsync();
            }

            InteractionOptions.IsPanEnabled = gpsMode == GpsMode.On || gpsMode == GpsMode.Off;

            LocationDisplay.IsEnabled = gpsMode != GpsMode.Off;
            LocationDisplay.ShowLocation = gpsMode != GpsMode.Off;
            LocationDisplay.ShowAccuracy = gpsMode != GpsMode.Off;
            LocationDisplay.ShowPingAnimationSymbol = gpsMode != GpsMode.Off;
            LocationDisplay.UseCourseSymbolOnMovement = true;
            LocationDisplay.WanderExtentFactor = 0;
        }

        public void Dispose()
        {
            LocationDisplay?.DataSource?.StopAsync();
            GC.Collect();
        }

        public static readonly BindableProperty MapLoadedCommandProperty =
            BindableProperty.Create(
                propertyName: "MapLoadedCommand",
                returnType: typeof(ICommand),
                declaringType: typeof(ExtendedMapView),
                defaultValue: null);

        public ICommand MapLoadedCommand
        {
            get { return (ICommand)GetValue(MapLoadedCommandProperty); }
            set { SetValue(MapLoadedCommandProperty, value); }
        }

        public static readonly BindableProperty MapHoldCommandProperty =
            BindableProperty.Create(
                propertyName: "MapHoldCommand",
                returnType: typeof(ICommand),
                declaringType: typeof(ExtendedMapView),
                defaultValue: null);

        public ICommand MapHoldCommand
        {
            get { return (ICommand)GetValue(MapHoldCommandProperty); }
            set { SetValue(MapHoldCommandProperty, value); }
        }

        public static readonly BindableProperty MapClickCommandProperty =
            BindableProperty.Create(
                propertyName: "MapClickCommand",
                returnType: typeof(ICommand),
                declaringType: typeof(ExtendedMapView),
                defaultValue: null);

        public ICommand MapClickCommand
        {
            get { return (ICommand)GetValue(MapClickCommandProperty); }
            set { SetValue(MapClickCommandProperty, value); }
        }
    }
}
