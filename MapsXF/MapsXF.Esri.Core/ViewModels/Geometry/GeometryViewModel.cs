using Esri.ArcGISRuntime.UI;
using Esri.Core.Extensions;
using Esri.Core.Helpers;
using Esri.Core.Models;
using Esri.Core.Providers;
using MapsXF.Core;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MapsXF
{
    public class GeometryViewModel : BaseViewModel
    {
        public GeometryViewModel(Graphic graphic)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                graphic.IsSelected = true;

                // Load geometry item from sqlite
                geometryItem = await DatabaseRepository.Current.LoadAsync<GeometryItem>(graphic.GetId());

                Name = geometryItem.Name;
                About = geometryItem.About;
                GeometryType = geometryItem.GeometryType;
                SelectedColor = geometryItem.Color;
            });
        }

        private async Task UpdateGeometryItem()
        {
            geometryItem.Name = Name;
            geometryItem.About = About;
            geometryItem.Color = string.IsNullOrEmpty(SelectedColor) ? System.Drawing.Color.Red.Name : SelectedColor;

            // Update item in sqlite
            await DatabaseRepository.Current.InsertAsync(geometryItem);

            var graphic = OverlayProvider.Current.GeometryOverlay.GetGraphicById(geometryItem.Id.ToString());

            if (graphic != null)
            {
                graphic.Symbol = SymbolProvider.GetSymbol(graphic.Geometry.GeometryType, ColorHelper.TryFromName(geometryItem.Color));
            }
        }

        private ICommand closeGeometryCommand;
        public ICommand CloseGeometryCommand => closeGeometryCommand ?? (closeGeometryCommand = new Command(() =>
        {
        }));

        private ICommand deleteGeometryCommand;
        public ICommand DeleteGeometryCommand => deleteGeometryCommand ?? (deleteGeometryCommand = new Command(async () =>
        {
            // Get graphic to remove
            var graphicToRemove = OverlayProvider.Current.GeometryOverlay.GetGraphicById(geometryItem.Id.ToString());

            // Remove graphic from layer
            OverlayProvider.Current.GeometryOverlay.Graphics.Remove(graphicToRemove);

            // Remove item from sqlite
            await DatabaseRepository.Current.DeleteAsync(entity: geometryItem);

        }));

        private ICommand saveGeometryCommand;
        public ICommand SaveGeometryCommand => saveGeometryCommand ?? (saveGeometryCommand = new Command(async () =>
        {
            await UpdateGeometryItem();

        }));

        private ICommand editGeometryCommand;
        public ICommand EditGeometryCommand => editGeometryCommand ?? (editGeometryCommand = new Command(async () =>
        {
            await UpdateGeometryItem();

        }));

        public string Name { get; set; }
        public string About { get; set; }
        public string GeometryType { get; set; }
        public string SelectedColor { get; set; }
        public ObservableCollection<string> ColorValues { get; set; } = new ObservableCollection<string>
        {
            System.Drawing.Color.Red.Name,
            System.Drawing.Color.Blue.Name,
            System.Drawing.Color.Green.Name,
            System.Drawing.Color.Orange.Name,
            System.Drawing.Color.White.Name,
            System.Drawing.Color.Purple.Name
        };

        private GeometryItem geometryItem;
    }
}