﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

// <controls:FloatingMenuControl>
//    <controls:FloatingMenuControl.BaseItem>
//        <controls:FloatingMenuItem
//            IconSource = "{StaticResource AddIcon}"
//            FontFamily="{StaticResource FontAwesomeRegular}" />
//    </controls:FloatingMenuControl.BaseItem>
//    <controls:FloatingMenuControl.MenuItems>
//        <controls:FloatingMenuItem
//            IconSource = "{StaticResource RemoveIcon}"
//            Text="Remove"
//            FontFamily="{StaticResource FontAwesomeRegular}" />
//        <controls:FloatingMenuItem
//            IconSource = "{StaticResource SearchIcon}"
//            Text="Search"
//            FontFamily="{StaticResource FontAwesomeRegular}" />
//        <controls:FloatingMenuItem
//            IconSource = "{StaticResource SendIcon}"
//            Text="Send"
//            FontFamily="{StaticResource FontAwesomeRegular}" />
//    </controls:FloatingMenuControl.MenuItems>
//</controls:FloatingMenuControl>
namespace MapsXF.Controls
{
    public class FloatingMenuControl : StackLayout
    {
        public FloatingMenuControl()
        {
            MenuItems = new List<FloatingMenuItem>();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            foreach (var item in MenuItems)
            {
                Grid grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition() { Width = GridLength.Auto },
                        new ColumnDefinition() { Width = GridLength.Auto }
                    },
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End
                };

                ImageButton but = new ImageButton
                {
                    BackgroundColor = item.BackgroundColor,
                    CornerRadius = FloatingButtonRadius,
                    HeightRequest = FloatingButtonHeight,
                    WidthRequest = FloatingButtonWidth,
                    Padding = new Thickness(FloatingButtonPadding),
                    Command = item.Command,
                    CommandParameter = item.CommandParameter,
                    Source = new FontImageSource
                    {
                        Color = item.TextColor,
                        Glyph = item.IconSource,
                        FontFamily = item.FontFamily,
                    }
                };

                but.Clicked += BaseButton_Clicked;

                StackLayout stack = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = item.HintBackgroundColor,
                    Padding = new Thickness(2),
                    Children =
                    {
                        new Label
                        {
                            TextColor = item.HintTextColor,
                            Text = item.Text
                        }
                    }
                };

                Grid.SetColumn(stack, 0);
                Grid.SetColumn(but, 1);

                grid.Children.Add(stack);
                grid.Children.Add(but);

                grid.IsVisible = false;

                Children.Add(grid);
            }

            ImageButton baseButton = new ImageButton
            {
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = BaseItem.BackgroundColor,
                CornerRadius = FloatingButtonRadius,
                HeightRequest = FloatingButtonHeight,
                WidthRequest = FloatingButtonWidth,
                Padding = new Thickness(FloatingButtonPadding),
                Source = new FontImageSource
                {
                    Color = BaseItem.TextColor,
                    Glyph = BaseItem.IconSource,
                    FontFamily = BaseItem.FontFamily,
                }
            };

            baseButton.Clicked += BaseButton_Clicked;

            Children.Add(baseButton);
        }

        private async void BaseButton_Clicked(object sender, EventArgs e)
        {
            if (Children.All(x => x.IsVisible))
            {
                await HideAsync();
            }
            else
            {
                await ShowAsync();
            }
        }

        private async Task ShowAsync()
        {
            var button = Children.LastOrDefault();

            await button.RotateTo(360, 350);

            foreach (var child in Children.Where(x => x != button))
            {
                child.IsVisible = !child.IsVisible;
            }
        }

        private async Task HideAsync()
        {
            var button = Children.LastOrDefault();

            await button.RotateTo(-360, 350);

            foreach (var child in Children.Where(x => x != button))
            {
                child.IsVisible = !child.IsVisible;
            }
        }

        public List<FloatingMenuItem> MenuItems { get; set; }
        public FloatingMenuItem BaseItem { get; set; }
        public int FloatingButtonWidth { get; set; } = 58;
        public int FloatingButtonHeight { get; set; } = 58;
        public int FloatingButtonRadius { get; set; } = 28;
        public int FloatingButtonPadding { get; set; } = 16;
    }

    public class FloatingMenuItem : View
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: "Text",
            returnType: typeof(string),
            declaringType: typeof(FloatingMenuItem),
            defaultValue: string.Empty);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: "Command",
            returnType: typeof(ICommand),
            declaringType: typeof(FloatingMenuItem),
            defaultValue: null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string IconSource { get; set; }
        public string FontFamily { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public Color HintTextColor { get; set; } = Color.Black;
        public Color HintBackgroundColor { get; set; } = Color.White;
        public object CommandParameter { get; set; }
    }
}
