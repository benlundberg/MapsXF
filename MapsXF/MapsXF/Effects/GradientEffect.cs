using Xamarin.Forms;

namespace MapsXF
{
    public enum GradientDirection
    {
        ToRight,
        ToLeft,
        ToTop,
        ToBottom,
        ToTopLeft,
        ToTopRight,
        ToBottomLeft,
        ToBottomRight
    }

    public class GradientEffect : RoutingEffect
    {


        public GradientEffect() : base("MapsXF.GradientEffect")
        {
        }

        public Color StartColor { get; set; } = Application.Current.PrimaryColor();
        public Color EndColor { get; set; } = Application.Current.DarkPrimaryColor();
        public GradientDirection Direction { get; set; } = GradientDirection.ToTop;
    }
}

