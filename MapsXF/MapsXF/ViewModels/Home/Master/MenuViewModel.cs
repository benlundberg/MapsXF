using System;
using Xamarin.Forms;

namespace MapsXF
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public Page Page { get; set; }
        public Action Action { get; set; }
    }
}