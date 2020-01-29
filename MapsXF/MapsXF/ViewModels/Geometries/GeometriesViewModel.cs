using Esri.Core.Extensions;
using Esri.Core.Providers;
using MapsXF.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MapsXF
{
    public class GeometriesViewModel : BaseViewModel
    {
        public GeometriesViewModel(IEnumerable<GeometryItem> geometries)
        {
            Geometries = new ObservableCollection<GeometryItem>(geometries);
        }

        private ICommand doneCommand;
        public ICommand DoneCommand => doneCommand ?? (doneCommand = new Command(() =>
        {
            Hide();
        }));

        private ICommand deleteMapsCommand;
        public ICommand DeleteMapsCommand => deleteMapsCommand ?? (deleteMapsCommand = new Command(async () =>
        {
            var res = await ShowConfirmAsync(Translate("Gen_Sure_Remove_All_Geometries"), Translate("Gen_Geometries"));

            if (!res)
            {
                return;
            }

            foreach (var item in Geometries)
            {
                await RemoveGeometryAsync(item);
            }

            Geometries.Clear();
        }));

        private ICommand deleteMapCommand;
        public ICommand DeleteMapCommand => deleteMapCommand ?? (deleteMapCommand = new Command(async (param) =>
        {
            if (!(param is GeometryItem item))
            {
                return;
            }

            var res = await ShowConfirmAsync(Translate("Gen_Sure_Remove_Geometry"), Translate("Gen_Geometries"));

            if (!res)
            {
                return;
            }

            await RemoveGeometryAsync(item);
            
            Geometries.Remove(item);
        }));

        private async Task RemoveGeometryAsync(GeometryItem item)
        {
            // Get graphic to remove
            var graphicToRemove = OverlayProvider.Current.GeometryOverlay.GetGraphicById(item.Id.ToString());

            // Remove graphic from layer
            OverlayProvider.Current.GeometryOverlay.Graphics.Remove(graphicToRemove);

            // Remove item from sqlite
            await DatabaseRepository.Current.DeleteAsync(entity: item);
        }

        private GeometryItem selectedGeometry;
        public GeometryItem SelectedGeometry
        {
            get { return selectedGeometry; }
            set 
            { 
                selectedGeometry = value;

                if (selectedGeometry != null)
                {
                    Hide();
                }
            }
        }

        public ObservableCollection<GeometryItem> Geometries { get; private set; }
    }
}
