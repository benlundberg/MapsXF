using System.Windows.Input;
using Xamarin.Forms;

namespace MapsXF
{
    public class PointViewModel : BaseViewModel
    {
        private ICommand navigateCommand;
        public ICommand NavigateCommand => navigateCommand ?? (navigateCommand = new Command(() =>
        {
            Hide(true);
        }));

        private ICommand closeCommand;
        public ICommand CloseCommand => closeCommand ?? (closeCommand = new Command(() =>
        {
            Hide();
        }));
    }
}
