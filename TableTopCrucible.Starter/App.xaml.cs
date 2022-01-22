﻿using System.Configuration;
using System.Windows;
using Microsoft.EntityFrameworkCore.Storage;
using Splat;
using TableTopCrucible.Core.Wpf.Engine;
using TableTopCrucible.Domain.Library.Services;

namespace TableTopCrucible.Starter
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            EngineStarter.InitializeEngine();
            Locator.Current.GetService<IFileWatcherService>().StartSynchronization();
        }
    }
}