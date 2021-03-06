﻿using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsXF.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchEntry : ContentView
    {
        public SearchEntry()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: "Text",
            returnType: typeof(string),
            declaringType: typeof(SearchEntry),
            defaultValue: default(string));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create(
            propertyName: "SearchCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(SearchEntry),
            defaultValue: default(ICommand));

        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create(
            propertyName: "TextChangedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(SearchEntry),
            defaultValue: default(ICommand));

        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        private ICommand clearCommand;
        public ICommand ClearCommand => clearCommand ?? (clearCommand = new Command(() =>
        {
            Text = string.Empty;
            IsClearVisible = false;
        }));

        private ICommand internalSearchCommand;
        public ICommand InternalSearchCommand => internalSearchCommand ?? (internalSearchCommand = new Command(() =>
        {
            SearchCommand?.Execute(Text);
        }));

        private void BorderlessEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsClearVisible = !string.IsNullOrEmpty(Text);
            TextChangedCommand?.Execute(Text);
        }

        public string Placeholder { get; set; }
        public bool IsClearVisible { get; set; }
        public NamedSize FontSize { get; set; } = NamedSize.Small;
        public double InternalFontSize => Device.GetNamedSize(FontSize, typeof(Entry));
        public Color TextColor { get; set; } = Color.Black;
        public Color PlaceholderColor { get; set; } = Color.Gray;
        public Color SearchIconColor { get; set; } = Color.Gray;
        public Color ClearIconColor { get; set; } = Color.Gray;
        public Color EntryBackground { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.Gray;
        public Thickness EntryPadding { get; set; } = Device.RuntimePlatform == Device.iOS ? new Thickness(16, 10) : new Thickness(16, 2);
    }
}