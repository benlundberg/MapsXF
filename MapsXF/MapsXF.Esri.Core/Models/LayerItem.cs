using Esri.ArcGISRuntime.Mapping;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Esri.Core.Models
{
    public class LayerItem : INotifyPropertyChanged
    {
        private ICommand showCommand;
        public ICommand ShowCommand => showCommand ?? (showCommand = new Command(() =>
        {
            IsVisible = !IsVisible;

            LayerVisibilityChanged?.Invoke();
        }));

        public string FontFamily { get; set; }
        public string Text { get; set; }
        public bool IsVisible { get => Layer.IsVisible; set => Layer.IsVisible = value; }
        public Layer Layer { get; set; }
        public Action LayerVisibilityChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
