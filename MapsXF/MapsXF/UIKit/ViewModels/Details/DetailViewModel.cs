using System.Collections.ObjectModel;

namespace MapsXF
{
    public class DetailViewModel : BaseViewModel
    {
        public DetailViewModel()
        {

        }

        public ObservableCollection<object> Items => new ObservableCollection<object>()
        {
            new object (),
            new object (),
            new object (),
            new object (),
            new object (),
            new object (),
        };
    }
}
