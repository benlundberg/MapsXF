using SkiaSharp;
using System.Collections.Generic;

namespace MapsXF.Controls.Charts
{
    public class SelectedChartValueItemArgs
    {
        public SKPoint TouchedPoint { get; set; }
        public IList<ChartValueItemParam> ChartValueItems { get; set; }
    }
}
