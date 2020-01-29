﻿using MapsXF.Core;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace MapsXF
{
    public class TranslateService : ITranslateService
    {
        public TranslateService(ILocalizeService localizeHelper)
        {
            ci = localizeHelper.GetCurrentCultureInfo();
        }

        public string Translate(string key)
        {
            ResourceManager rm = new ResourceManager("MapsXF.Resources.Strings", typeof(TranslateService).GetTypeInfo().Assembly);

            string result = rm.GetString(key, ci);

            if (result == null)
            {
                Debug.WriteLine("Translation Error: Could not find translation for key '" + key + "'");
                result = key;
            }

            return result;
        }

        private readonly CultureInfo ci;
    }
}
