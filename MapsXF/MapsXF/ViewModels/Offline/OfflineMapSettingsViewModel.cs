using Esri.Core.Models;
using MapsXF.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace MapsXF
{
    public class OfflineMapSettingsViewModel : BaseViewModel
    {
        public OfflineMapSettingsViewModel(IEnumerable<OfflineMapItem> maps)
        {
            DeletedMaps = new List<string>();
            DownloadedMaps = new ObservableCollection<OfflineMapItem>(maps);
        }

        private ICommand doneCommand;
        public ICommand DoneCommand => doneCommand ?? (doneCommand = new Command(() =>
        {
            Hide();
        }));

        private ICommand deleteMapsCommand;
        public ICommand DeleteMapsCommand => deleteMapsCommand ?? (deleteMapsCommand = new Command(async () =>
        {
            var res = await ShowConfirmAsync(Translate("Gen_Sure_Remove_All_Offline_Map"), Translate("Gen_Downloaded_Maps"));

            if (!res)
            {
                return;
            }

            foreach (var item in DownloadedMaps)
            {
                DeletedMaps.Add(item.Path);
                ComponentContainer.Current.Resolve<ILocalFileSystemService>().Delete(item.Path);
            }

            DownloadedMaps.Clear();
        }));

        private ICommand deleteMapCommand;
        public ICommand DeleteMapCommand => deleteMapCommand ?? (deleteMapCommand = new Command(async (param) =>
        {
            if (!(param is OfflineMapItem item))
            {
                return;
            }

            var res = await ShowConfirmAsync(Translate("Gen_Sure_Remove_Offline_Map"), Translate("Gen_Downloaded_Maps"));

            if (!res)
            {
                return;
            }

            DeletedMaps.Add(item.Path);

            ComponentContainer.Current.Resolve<ILocalFileSystemService>().Delete(item.Path);
            DownloadedMaps.Remove(item);
        }));

        public ObservableCollection<OfflineMapItem> DownloadedMaps { get; private set; }
        public List<string> DeletedMaps { get; private set; }
    }
}
