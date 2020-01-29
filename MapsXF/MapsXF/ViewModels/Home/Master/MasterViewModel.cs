using System.Collections.Generic;
using System.Linq;
using MapsXF.Core;
using Xamarin.Forms;

namespace MapsXF
{
    public class MasterViewModel : BaseViewModel
    {
        public MasterViewModel(MasterDetailPage masterDetailPage)
        {
            Title = Device.RuntimePlatform == Device.iOS ? "☰" : AppConfig.AppName;

            this.masterDetailPage = masterDetailPage;

            if (Device.RuntimePlatform == Device.UWP)
            {
                masterDetailPage.MasterBehavior = MasterBehavior.Popover;
            }

            var mapViewModel = new MapViewModel();

            MasterItems = new List<MenuViewModel>()
            {
                new MenuViewModel
                {
                    Action = () => masterDetailPage.Detail = new NavigationPage(ViewContainer.Current.CreatePage(mapViewModel)),
                    Title = Translate("Gen_Map")
                },
                new MenuViewModel
                {
                    Action = mapViewModel.ShowDownloadedMaps,
                    Title = Translate("Gen_Downloaded_Maps")
                },
                new MenuViewModel
                {
                    Action = mapViewModel.ShowGeometries,
                    Title = Translate("Gen_Geometries")
                }
            };

            // TODO: If test you can add this log view
            if (true)
            {
                MasterItems.Add(new MenuViewModel()
                {
                    Title = Translate("Gen_Log"),
                    Page = new NavigationPage(ViewContainer.Current.CreatePage<LoggerViewModel>())
                });
            }

            if (MasterItems.FirstOrDefault()?.Page == null)
            {
                MasterItems.FirstOrDefault().Action();
            }
            else
            {
                ItemSelected(MasterItems.FirstOrDefault()?.Page);
            }
        }

        private void ItemSelected(Page page)
        {
            if (page == null)
            {
                return;
            }

            masterDetailPage.Detail = page;
        }

        private MenuViewModel selectedMasterItem;
        public MenuViewModel SelectedMasterItem
        {
            get { return selectedMasterItem; }
            set
            {
                selectedMasterItem = value;

                if (selectedMasterItem != null)
                {
                    masterDetailPage.IsPresented = false;

                    if (selectedMasterItem.Page == null)
                    {
                        selectedMasterItem.Action?.Invoke();
                    }
                    else
                    {
                        ItemSelected(selectedMasterItem.Page);
                    }

                    SelectedMasterItem = null;
                }
            }
        }

        public List<MenuViewModel> MasterItems { get; private set; }

        public string Title { get; set; }

        private MasterDetailPage masterDetailPage;
    }
}
