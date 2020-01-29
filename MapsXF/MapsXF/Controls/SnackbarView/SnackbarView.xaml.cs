using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsXF.Controls
{
    public enum SnackbarDuration
    {
        SHORT,
        LONG
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnackbarView : ContentView
    {
        public SnackbarView()
        {
            InitializeComponent();
            TranslationY = 50;
        }

        public async Task ShowLoadingAsync(string message)
        {
            this.Text = message;
            this.IsLoading = true;

            await OpenAsync();
        }

        public async Task HideLoadingAsync()
        {
            await CloseAsync();
        }

        public async Task ShowAsync(string message, SnackbarDuration duration, string buttonText, ICommand command)
        {
            this.Text = message;
            this.ButtonText = buttonText;
            this.Command = command;

            await OpenAsync();

            if (duration == SnackbarDuration.SHORT)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            await CloseAsync();
        }

        private async Task CloseAsync()
        {
            await this.TranslateTo(0, 50, 400);
            IsVisible = false;
        }

        private async Task OpenAsync()
        {
            IsVisible = true;
            await this.TranslateTo(0, 0, 400);
        }

        public bool IsLoading { get; set; }
        public string Text { get; set; }
        public string ButtonText { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public Color ButtonTextColor { get; set; } = Color.Orange;
        public ICommand Command { get; set; }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CloseAsync();
        }
    }
}