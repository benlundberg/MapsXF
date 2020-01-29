using System.Collections.Generic;
using System.Globalization;

namespace MapsXF.Core
{
    public interface ILocalizeService
    {
        string GetCurrentCountry();
        CultureInfo GetCurrentCultureInfo();
        void SetLocale();
        IComparer<string> CreateStringComparer(CultureInfo cultureInfo = null);
    }
}
