using MapsXF.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Esri.Core.Helpers
{
    public static class ColorHelper
    {
        public static IList<string> GetGeometryColorNames()
        {
            return new List<string>
            {
                Color.Red.Name,
                Color.Blue.Name,
                Color.Green.Name,
                Color.Orange.Name,
                Color.White.Name,
                Color.Purple.Name
            };
        }

        public static Color TryFromName(string name)
        {
            try
            {
                var colorFromName = Color.FromName(name);

                if (colorFromName.IsNamedColor)
                { 
                    return colorFromName; 
                }
            }
            catch (Exception ex)
            {
                ex.Print();
            }

            return Color.Red;
        }
    }
}
