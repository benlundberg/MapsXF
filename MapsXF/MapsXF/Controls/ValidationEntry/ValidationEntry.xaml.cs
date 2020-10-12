using MapsXF.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsXF.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValidationEntry : ContentView
    {
        public ValidationEntry()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: "Text",
            returnType: typeof(ValidatableObject<string>),
            declaringType: typeof(FrameEntry),
            defaultValue: default(ValidatableObject<string>));

        public ValidatableObject<string> Text
        {
            get { return (ValidatableObject<string>)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            if (HasValidation)
            {
                Text?.Validate();
            }
        }

        public Keyboard Keyboard { get; set; }
        public string Placeholder { get; set; }
        public Color PlaceholderColor { get; set; } = Color.Gray;
        public Color TextColor { get; set; } = Color.Black;
        public bool IsPassword { get; set; }
        public string IconFontFamily { get; set; }
        public string IconTextSource { get; set; }
        public Color IconColor { get; set; } = Color.Black;
        public bool HasIcon => !string.IsNullOrEmpty(IconTextSource);
        public bool HasValidation { get; set; }
    }
}