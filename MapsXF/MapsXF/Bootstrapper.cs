﻿using MapsXF.Core;
using System;
using System.Collections.Generic;

namespace MapsXF
{
    public class Bootstrapper
    {
        public static void RegisterTypes()
        {
            // Services
            ComponentContainer.Current.Register<ITranslateService, TranslateService>();
            ComponentContainer.Current.Register<ILoggerService, LoggerService>(singelton: true);
        }

        public static void RegisterViews()
        {
            ViewContainer.Current.Register<HomeMasterViewModel, HomeMasterPage>();
            ViewContainer.Current.Register<MasterViewModel, MasterPage>();
            ViewContainer.Current.Register<HomeTabbedViewModel, HomeTabbedPage>();
            ViewContainer.Current.Register<LoggerViewModel, LoggerPage>();
            ViewContainer.Current.Register<MapViewModel, MapPage>();
        }

        public static void CreateTables()
        {
            DatabaseRepository.Current.CreateTablesAsync(new List<Type>()
            {
                typeof(GeometryItem)
            }).ConfigureAwait(false);
        }
    }
}
