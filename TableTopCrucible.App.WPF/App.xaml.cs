﻿using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using TableTopCrucible.App.WPF.ViewModels;
using TableTopCrucible.Core.WPF.Tabs.ViewModels;

namespace TableTopCrucible.App.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }
        protected override void OnStartup(StartupEventArgs e)
        {


            this.Resources.MergedDictionaries.Add(Core.WPF.Helper.Factory.GetTemplateDictionary());

            var provider = Core.DI.Factory.GenerateServiceProvider();

            new MainWindow()
            {
                Content = provider.GetRequiredService<IMainPage>()
            }.Show();



        }
    }
}
