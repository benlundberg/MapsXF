﻿using MapsXF.Core;

namespace MapsXF.iOS
{
    public class Bootstrapper_iOS
    {
        public static void Initialize()
        {
            // Register common types
            Bootstrapper.RegisterTypes();

            // Register device specific types
            RegisterTypes();
        }

        private static void RegisterTypes()
        {
            // Services
            ComponentContainer.Current.Register<ILocalFileSystemService, LocalFileSystemService_iOS>(singelton: true);
            ComponentContainer.Current.Register<IOpenFileService, OpenFileService_iOS>(singelton: true);
            ComponentContainer.Current.Register<IDialogService, DialogService_iOS>(singelton: true);
            ComponentContainer.Current.Register<INotificationService, NotificationService_iOS>(singelton: true);
        }
    }
}
