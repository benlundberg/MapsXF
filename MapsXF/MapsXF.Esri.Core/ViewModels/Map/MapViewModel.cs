using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Esri.Core.Controls;
using Esri.Core.Services;
using MapsXF.Core;
using System.Linq;
using Esri.ArcGISRuntime.Geometry;
using System.Collections.Generic;
using Esri.Core.Models;
using Esri.Core.Providers;
using Esri.Core.Extensions;
using Esri.ArcGISRuntime.Ogc;
using Esri.Core.Models.Enums;

namespace MapsXF
{
    public class MapViewModel : BaseViewModel, IDisposable
    {
        public async override void Appearing()
        {
            base.Appearing();

            var res = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.LocationWhenInUse>();

            if (res == Xamarin.Essentials.PermissionStatus.Granted)
            {
                var location = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();

                Map = new Map(BasemapService.GetDefaultBasemap())
                {
                    InitialViewpoint = new Viewpoint(location.Latitude, location.Longitude, 1500000)
                };
            }
            else
            {
                Map = new Map(BasemapService.GetDefaultBasemap());
            }
        }

        private async Task<bool?> ShowPopupAsync(ContentView view, BaseViewModel viewModel)
        {
            view.BindingContext = viewModel;

            PopupView = view;

            await PopupView.SlideUpAsync(Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height / 3, 400, Easing.CubicIn);

            //var res = await viewModel.ShowAsync();

            await PopupView.SlideDownAsync(400, Easing.CubicOut);

            PopupView = null;

            return false;
        }

        private void ChangeBasemap(Basemap basemap)
        {
            if (basemap == null)
            {
                return;
            }

            // Getting the viewpoint
            var currentRegion = mapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

            // Clear all baselayers
            Map.Basemap.BaseLayers.Clear();

            // Adding the layers
            LayerService.MapLayers.ForEach(x =>
            {
                basemap.BaseLayers.Add(x.Layer);
            });

            // Setting the new map with same region
            Map = new Map(basemap)
            {
                InitialViewpoint = currentRegion
            };
        }

        public void ShowDownloadedMaps()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    // Fetch file paths in download folder
                    var filePaths = ComponentContainer.Current.Resolve<ILocalFileSystemService>().GetFiles(Esri.Core.Constants.DownloadFolder);

                    var downloadedMaps = new List<OfflineMapItem>();

                    if (filePaths?.Any() == true)
                    {
                        foreach (var path in filePaths)
                        {
                            var layer = Map.Basemap.BaseLayers.FirstOrDefault(x => x.Id == path);

                            downloadedMaps.Add(new OfflineMapItem
                            {
                                Name = System.IO.Path.GetFileName(path),
                                Path = path,
                                IsVisible = layer?.IsVisible == true,
                                Layer = layer
                            });
                        }
                    }

                    // Create view model
                    var viewModel = new OfflineMapSettingsViewModel(downloadedMaps);

                    // Show popup
                    await ShowPopupAsync(new OfflineMapSettingsView(), viewModel);

                    // Show layers
                    await mapView.ShowOfflineMapsAsync(OfflineMapService, viewModel.DownloadedMaps, viewModel.DeletedMaps);
                }
                catch (Exception ex)
                {
                    ex.Print();
                }
            });
        }

        public void ShowGeometries()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var geometries = await DatabaseRepository.Current.LoadAllAsync<GeometryItem>();

                    // Create view model
                    var viewModel = new GeometriesViewModel(geometries);

                    // Show popup
                    var res = await ShowPopupAsync(new GeometriesView(), viewModel);

                    if (viewModel.SelectedGeometry != null)
                    {
                        var graphic = OverlayProvider.Current.GeometryOverlay.GetGraphicById(viewModel.SelectedGeometry.Id.ToString());

                        await mapView.SetViewpointGeometryAsync(graphic.Geometry, padding: 50);
                    }
                }
                catch (Exception ex)
                {
                    ex.Print();
                }
            });
        }

        public void Dispose()
        {
            mapView?.Dispose();
            GC.Collect();
        }

        private ICommand downloadOfflineMapCommand;
        public ICommand DownloadOfflineMapCommand => downloadOfflineMapCommand ?? (downloadOfflineMapCommand = new Command(async () =>
        {
            try
            {
                var geometry = await Editor.CreateAreaToDownloadAsync("Gen_Draw_Area_To_Download");

                if (geometry == null)
                {
                    return;
                }

                var shouldDownload = await ShowConfirmAsync("Download_Region", "Gen_Download_Offline_Map");

                if (!shouldDownload)
                {
                    return;
                }

                var permission = false;// await PermissionService.Current.CheckPermissionAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                if (!permission)
                {
                    return;
                }

                //string name = await Application.Current.MainPage.DisplayPromptAsync(
                //    Translate("Gen_Download_Offline_Map"),
                //    Translate("Create_Map_Name"),
                //    Translate("Gen_Ok"),
                //    Translate("Gen_Cancel"),
                //    Translate("Create_Map_Name"),
                //    12,
                //    Keyboard.Default);

                //if (string.IsNullOrEmpty(name))
                //{
                //    ShowAlert(Translate("Missing_Map_Name"), Translate("Gen_Download_Offline_Map"));
                //    return;
                //}

                ComponentContainer.Current.Resolve<ILocalFileSystemService>().CreateFolder(Esri.Core.Constants.DownloadFolder);

                //await ShowLoadingSnackbarAsync(Translate("Gen_Download_Offline_Map"));

                //await OfflineMapService.DownloadMapAsync(
                //    area: geometry,
                //    downloadFolderPath: ComponentContainer.Current.Resolve<ILocalFileSystemService>().GetLocalPath(Esri.Core.Constants.DownloadFolder, name));
            }
            catch (Exception ex)
            {
                ex.Print();
                //ShowAlert(ex.Message, Translate("Gen_Download_Map_Failed"));
            }
            finally
            {
                //await HideLoadingSnackbarAsync();
            }
        }));

        private ICommand measureCommand;
        public ICommand MeasureCommand => measureCommand ?? (measureCommand = new Command(async () =>
        {
            try
            {
                if (Editor.IsActive)
                {
                    return;
                }

                //// Selecting measure type 
                //var measureType = await ShowActionSheetAsync(
                //    Translate("Select_Geometry"),
                //    Translate("Gen_Cancel"),
                //    null,
                //    new string[] { Translate("Measure_Area"), Translate("Measure_Lenght") });

                //if (string.IsNullOrEmpty(measureType) || measureType?.Equals(Translate("Gen_Cancel")) == true)
                //{
                //    return;
                //}

                //await Editor.CreateNewGeometry(measureType.Equals(Translate("Measure_Area")) ? SketchCreationMode.Polygon : SketchCreationMode.Polyline, measure: true);
            }
            catch (Exception ex)
            {
                ex.Print();
            }
        }));

        private ICommand mapClickedCommand;
        public ICommand MapClickedCommand => mapClickedCommand ?? (mapClickedCommand = new Command(async (param) =>
        {
            if (!(param is Graphic graphic) || Editor.IsActive)
            {
                return;
            }

            var viewModel = new GeometryViewModel(graphic);

            graphic.IsSelected = true;

            var res = await ShowPopupAsync(new GeometryView(), viewModel);

            graphic.IsSelected = false;

            if (res == true)
            {
                await Editor.EditGeometryAsync(graphic, Map.SpatialReference);
            }
        }));

        private ICommand mapHoldCommand;
        public ICommand MapHoldCommand => mapHoldCommand ?? (mapHoldCommand = new Command(async (param) =>
        {
            if (!(param is MapPoint location) || Editor.IsActive)
            {
                return;
            }

            Geometry myGeometry = GeometryEngine.Project(location, SpatialReferences.Wgs84);

            MapPoint projectedLocation = (MapPoint)myGeometry;

            string mapLocationDescription = $"Lat: {projectedLocation.Y:F3} Long: {projectedLocation.X:F3}";

            CalloutDefinition myCalloutDefinition = new CalloutDefinition("Location:", mapLocationDescription);

            mapView.ShowCalloutAt(location, myCalloutDefinition);

            var viewModel = new PointViewModel();

            var res = await ShowPopupAsync(new PointView(), viewModel);

            mapView.DismissCallout();

            if (res == true)
            {
                await Xamarin.Essentials.Map.OpenAsync(new Xamarin.Essentials.Location(projectedLocation.Y, projectedLocation.X));
            }
        }));

        private ICommand changeBasemapCommand;
        public ICommand ChangeBasemapCommand => changeBasemapCommand ?? (changeBasemapCommand = new Command(async () =>
        {
            var basemapTypes = Enum.GetNames(typeof(BasemapType));

            var res = await ShowActionSheetAsync("Select_Basemap", "Gen_Cancel", null, basemapTypes);

            if (string.IsNullOrEmpty(res) || res?.Equals("Gen_Cancel") == true)
            {
                return;
            }

            if (Enum.TryParse(res, out BasemapType basemapType))
            {
                ChangeBasemap(BasemapService.GetBasemap(basemapType));
            }
        }));

        private ICommand swedenBasemapCommand;
        public ICommand SwedenBasemapCommand => swedenBasemapCommand ?? (swedenBasemapCommand = new Command(async () =>
        {
            ChangeBasemap(await BasemapService.GetTopoBasemapAsync());
        }));

        private ICommand locationOptionsCommand;
        public ICommand LocationOptionsCommand => locationOptionsCommand ?? (locationOptionsCommand = new Command(async () =>
        {
            //var permission = await PermissionService.Current.CheckPermissionAsync(Plugin.Permissions.Abstractions.Permission.Location);

            //if (!permission)
            //{
            //    return;
            //}

            //var res = await ShowActionSheetAsync(Translate("Select_Gps_Settings"), Translate("Gen_Cancel"), null, new string[]
            //{
            //    Translate("Gen_Off"),
            //    Translate("Gen_On"),
            //    Translate("Gps_ReCenter"),
            //    Translate("Gps_Navigation"),
            //    Translate("Gps_Compass")
            //});

            //if (res == null || res?.Equals(Translate("Gen_Cancel")) == true)
            //{
            //    return;
            //}

            //GpsMode gpsMode = GpsMode.On;

            //if (res == Translate("Gen_Off"))
            //{
            //    gpsMode = GpsMode.Off;
            //}
            //else if (res == Translate("Gen_On"))
            //{
            //    gpsMode = GpsMode.On;
            //}
            //else if (res == Translate("Gps_ReCenter"))
            //{
            //    gpsMode = GpsMode.ReCenter;
            //}
            //else if (res == Translate("Gps_Navigation"))
            //{
            //    gpsMode = GpsMode.Navigation;
            //}
            //else if (res == Translate("Gps_Compass"))
            //{
            //    gpsMode = GpsMode.Compass;
            //}

            //FollowPosition = gpsMode != GpsMode.Off || gpsMode != GpsMode.On;

            //this.mapView.InteractionOptions.IsPanEnabled = !FollowPosition;

            //await this.mapView.SetGpsModeAsync(gpsMode);
        }));

        private ICommand positionClickedCommand;
        public ICommand PositionClickedCommand => positionClickedCommand ?? (positionClickedCommand = new Command(async (param) =>
        {
            //var res = await PermissionService.Current.CheckPermissionAsync(Plugin.Permissions.Abstractions.Permission.Location);

            //if (!res)
            //{
            //    return;
            //}

            //FollowPosition = !FollowPosition;

            //this.mapView.InteractionOptions.IsPanEnabled = !FollowPosition;

            //GpsMode gpsMode = FollowPosition ? GpsMode.Navigation : GpsMode.On;

            //await mapView.SetGpsModeAsync(gpsMode);
        }));

        private ICommand drawCommand;
        public ICommand DrawCommand => drawCommand ?? (drawCommand = new Command(async () =>
        {
            if (Editor.IsActive)
            {
                return;
            }

            //// Selecting geometry 
            //var selectedSketchMode = await ShowActionSheetAsync(
            //    Translate("Select_Geometry"),
            //    Translate("Gen_Cancel"),
            //    null,
            //    Enum.GetNames(typeof(SketchCreationMode)));

            //if (!Enum.TryParse(selectedSketchMode, out SketchCreationMode creationMode))
            //{
            //    return;
            //}

            //await Editor.CreateNewGeometry(creationMode);
        }));

        private ICommand mapLoadedCommand;
        public ICommand MapLoadedCommand => mapLoadedCommand ?? (mapLoadedCommand = new Command(async (param) =>
        {
            if (!(param is ExtendedMapView mapView))
            {
                return;
            }

            this.mapView = mapView;

            LayerService = new LayerService(Map);

            try
            {
                //await ShowLoadingSnackbarAsync(Translate("Gen_Loading_Layers"));

                await DrawService.DrawGeometriesAsync();

                await LayerService.InitLayersAsync();

                await LoadKmlLayerAsync();
            }
            catch (Exception ex)
            {
                ex.Print();
            }
            finally
            {
                //await HideLoadingSnackbarAsync();
            }
        }));

        private async Task LoadKmlLayerAsync()
        {
            Uri kmlUrl = new Uri("http://clarityapplication.com/dev/Gottne_4_3_20200123.kml");

            // Create the KML dataset and layer.
            KmlDataset dataset = new KmlDataset(kmlUrl);
            
            KmlLayer layer = new KmlLayer(dataset)
            {
                Id = "KML"
            };

            await layer.LoadAsync();

            mapView.Map.OperationalLayers.Add(layer);

            await mapView.SetViewpointGeometryAsync(layer.FullExtent);
        }

        private ICommand compassClickCommand;
        public ICommand CompassClickCommand => compassClickCommand ?? (compassClickCommand = new Command(async () =>
        {
            await this.mapView.SetViewpointRotationAsync(0);
        }));

        public Map Map { get; private set; }
        public ContentView PopupView { get; set; }
        public EditorService Editor { get; private set; } = new EditorService();
        public BasemapService BasemapService { get; private set; } = new BasemapService();
        public OfflineMapService OfflineMapService { get; private set; } = new OfflineMapService();
        public LayerService LayerService { get; private set; }
        public bool FollowPosition { get; set; }

        private ExtendedMapView mapView;
    }
}
