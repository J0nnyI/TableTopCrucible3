﻿

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

using DynamicData;
using DynamicData.Binding;

using MaterialDesignThemes.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Helper;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient(typeof(NavigationListVm))]
    public interface INavigationList
    {

    }
    public class NavigationListVm : ReactiveObject, INavigationList, IActivatableViewModel
    {
        private readonly INavigationService _navigationService;

        public ObservableCollectionExtended<INavigationPage> UpperList { get; } = new();
        public ObservableCollectionExtended<INavigationPage> LowerList { get; } = new();
        [Reactive]
        public bool IsExpanded { get; set; }

        public ICommand ToggleExpansionCommand { get; private set; }

        public NavigationListVm(INavigationService navigationService)
        {
            _navigationService = navigationService;

            this.WhenActivated(() => new[]{
                _navigationService
                    .Pages
                    .Connect()
                    .Filter(m=>m.PageLocation == NavigationPageLocation.Lower)
                    .Sort(m=>m.Position.Value)
                    .Bind(LowerList)
                    .Subscribe(),

                _navigationService
                    .Pages
                    .Connect()
                    .Filter(m=>m.PageLocation == NavigationPageLocation.Upper)
                    .Sort(m=>m.Position.Value)
                    .Bind(UpperList)
                    .Subscribe(),

                ReactiveCommandHelper.Create(
                    () =>IsExpanded = !IsExpanded,
                    cmd=>ToggleExpansionCommand = cmd)
            }, vm => vm.IsExpanded);
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
