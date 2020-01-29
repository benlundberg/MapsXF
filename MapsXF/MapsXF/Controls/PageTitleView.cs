﻿using Xamarin.Forms;

namespace MapsXF.Controls
{
    public class PageTitleView : Label
    {
        public PageTitleView()
        {
            MaxLines = 1;
            TextColor = Application.Current.ToolbarTextColor();
            VerticalOptions = LayoutOptions.CenterAndExpand;

            if (Device.RuntimePlatform == Device.iOS)
            {
                Margin = new Thickness(8, 0, 0, 0);
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            }
            else
            {
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label));
            }
        }
    }
}