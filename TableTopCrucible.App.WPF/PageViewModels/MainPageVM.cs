﻿using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.App.WPF.Views;
using TableTopCrucible.Core.DI;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Core.WPF.Tabs.ViewModels;
using TableTopCrucible.DomainCore.WPF.Toolbar.ViewModels;

namespace TableTopCrucible.App.WPF.ViewModels
{
    [Transient(typeof(MainPageVM))]
    public interface IMainPage { 
    }

    [ViewModel(typeof(MainPageV))]
    internal class MainPageVM : IMainPage
    {
        public MainPageVM(IToolbar toolbar)
        {
            Toolbar = toolbar;
        }

        public IToolbar Toolbar { get; }
    }
}