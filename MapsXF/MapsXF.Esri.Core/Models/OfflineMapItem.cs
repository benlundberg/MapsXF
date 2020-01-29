using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;

namespace Esri.Core.Models
{
    public class OfflineMapItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsVisible { get; set; }
        public Layer Layer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
