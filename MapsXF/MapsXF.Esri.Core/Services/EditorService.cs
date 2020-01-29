using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using Esri.Core.Extensions;
using Esri.Core.Factories;
using Esri.Core.Providers;
using MapsXF.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Esri.Core.Services
{
    public class EditorService : INotifyPropertyChanged
    {
        public async Task<Geometry> CreateAreaToDownloadAsync(string infoMessage)
        {
            if (IsActive)
            {
                return null;
            }

            try
            {
                IsActive = true;

                GeometryInfo = infoMessage;
                
                IsGeometryInfoVisible = true;

                var geometry = await SketchEditor.StartAsync(SketchCreationMode.Polygon, new SketchEditConfiguration()
                {
                    AllowMove = true,
                    ResizeMode = SketchResizeMode.Stretch,
                    AllowRotate = true
                });

                return geometry;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CreateNewGeometry Exception: " + ex.Message);
            }
            finally
            {
                IsActive = false;
                IsGeometryInfoVisible = false;
            }

            return null;
        }

        public async Task CreateNewGeometry(SketchCreationMode creationMode, bool measure = false)
        {
            if (IsActive)
            {
                return;
            }

            try
            {
                IsActive = true;
                SketchEditor.GeometryChanged += SketchEditor_GeometryChanged;

                var geometry = await SketchEditor.StartAsync(creationMode, new SketchEditConfiguration());

                if (geometry == null || measure)
                {
                    return;
                }

                // Creates a sqlite object to save data
                var item = new GeometryItem
                {
                    Color = Color.Red.Name,
                    GeometryType = geometry.GeometryType.ToString(),
                    GeometryJson = geometry.ToJson()
                };

                // Saves data
                await DatabaseRepository.Current.InsertAsync(item);

                // Creates graphic 
                var graphic = GraphicFactory.Current.CreateGraphic(geometry, Color.Red, item.Id);

                // Add graphic to overlay
                OverlayProvider.Current.GeometryOverlay.Graphics.Add(graphic);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CreateNewGeometry Exception: " + ex.Message);
            }
            finally
            {
                IsActive = false;
                IsGeometryInfoVisible = false;
                SketchEditor.GeometryChanged -= SketchEditor_GeometryChanged;
            }
        }

        public async Task EditGeometryAsync(Graphic graphic, SpatialReference mapSpatialReference)
        {
            if (IsActive)
            {
                return;
            }

            try
            {
                IsActive = true;
                SketchEditor.GeometryChanged += SketchEditor_GeometryChanged;

                // Maps geometry spatialreference
                var geometryToEdit = GeometryEngine.Project(graphic.Geometry, mapSpatialReference);

                var geometry = await SketchEditor.StartAsync(geometryToEdit);

                if (geometry == null)
                {
                    return;
                }

                // Updates item in sqlite
                var item = await DatabaseRepository.Current.LoadAsync<GeometryItem>(id: int.Parse(graphic.GetId()));
                
                if (item == null)
                {
                    return;
                }
                
                item.GeometryJson = geometry.ToJson();
                item.GeometryType = geometry.GeometryType.ToString();
                
                await DatabaseRepository.Current.InsertOrReplaceAsync(item);

                graphic.Geometry = geometry;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EditGeometryAsync Exception: " + ex.Message);
            }
            finally
            {
                IsActive = false;
                IsGeometryInfoVisible = false;
                SketchEditor.GeometryChanged -= SketchEditor_GeometryChanged;
            }
        }

        private void SketchEditor_GeometryChanged(object sender, GeometryChangedEventArgs e)
        {
            if (e.NewGeometry.GeometryType == GeometryType.Polygon)
            {
                GeometryInfo = $"{e.NewGeometry.GetArea()} ha";
                IsGeometryInfoVisible = true;
            }
            else if (e.NewGeometry.GeometryType == GeometryType.Polyline)
            {
                GeometryInfo = $"{e.NewGeometry.GetMeters()} m";
                IsGeometryInfoVisible = true;
            }
        }

        public SketchEditor SketchEditor { get; private set; } = new SketchEditor();
        public bool IsActive { get; private set; }
        public bool IsGeometryInfoVisible { get; set; }
        public string GeometryInfo { get; set; }
    
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
