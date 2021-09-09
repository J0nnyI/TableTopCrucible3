

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

        [Reactive]
        public INavigationPage SelectedBufferItem { get; set; }

        public NavigationListVm(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.WhenActivated(() => new[]{
                // bind v selection to buffer
                navigationService
                    .Pages
                    .Connect()
                    .Filter(m=>m.PageLocation == NavigationPageLocation.Lower)
                    .Sort(m=>m.Position.Value)
                    .Bind(LowerList)
                    .Subscribe(),
                navigationService
                    .Pages
                    .Connect()
                    .Filter(m=>m.PageLocation == NavigationPageLocation.Upper)
                    .Sort(m=>m.Position.Value)
                    .Bind(UpperList)
                    .Subscribe(),
                // revert on null
                this.WhenAnyValue(vm=>vm.SelectedBufferItem)
                    .DistinctUntilChanged()
                    .Pairwise()
                    .Where(m=>!m.Current.HasValue)
                    .Select(m=>m.Previous.Value)
                    .Where(m=>m != SelectedBufferItem)
                    .BindTo(this, vm=>vm.SelectedBufferItem),
                // bind buffer to live selection
                this.WhenAnyValue(vm=>vm.SelectedBufferItem)
                    .Where(m=>m != null)
                    .BindTo(this, vm=>vm._navigationService.CurrentPage),
                // bind live selection to buffer
                this.WhenAnyValue(vm=>vm._navigationService.CurrentPage)
                .Where(m=>m != null && m != SelectedBufferItem)
                .BindTo(this, vm=>vm.SelectedBufferItem),

                ReactiveCommandHelper.Create(
                    () =>IsExpanded = !IsExpanded,
                    cmd=>ToggleExpansionCommand = cmd),

                this.WhenAnyValue(vm=>vm.SelectedBufferItem)
                    .Where(x=>x!=null)
                    .Subscribe(page =>navigationService.CurrentPage = page),

                this.WhenAnyValue(vm=>vm._navigationService.CurrentPage)
                    .WhereNotNull()
                    .Subscribe(m =>
                    {

                    }),
                this.WhenAnyValue(vm=>vm.SelectedBufferItem)
                    .WhereNotNull()
                    .Subscribe(m =>
                    {

                    }),

            });
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
