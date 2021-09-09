

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Permissions;
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
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    public class FlaggedNavigationItem
    {
        public FlaggedNavigationItem(INavigationPage page, bool changedByUser)
        {
            Page = page;
            this.PageLocation = page.PageLocation;
            this.ChangedByUser = changedByUser;
        }
        // used for deselection
        public FlaggedNavigationItem(NavigationPageLocation location, bool changedByUser)
        {
            this.PageLocation = location;
            this.ChangedByUser = changedByUser;
        }

        public DateTime TimeStamp { get; private set; } = DateTime.Now;
        public void OnSelected() => TimeStamp = DateTime.Now;
        public INavigationPage Page { get; }
        public PackIconKind? Icon => Page?.Icon;

        public Name Title => Page?.Title;

        public bool ChangedByUser { get; }

        public NavigationPageLocation? PageLocation { get; }

        public SortingOrder Position => Page?.Position;

        public bool HasContent => Page != null;

        public override string ToString()
            => $"{Title}  |  cbu:{ChangedByUser}  |  {PageLocation}";
    }

    [Transient(typeof(NavigationListVm))]
    public interface INavigationList
    {

    }
    public class NavigationListVm : ReactiveObject, INavigationList, IActivatableViewModel
    {
        private readonly INavigationService _navigationService;

        public ObservableCollectionExtended<FlaggedNavigationItem> UpperList { get; } = new();
        public ObservableCollectionExtended<FlaggedNavigationItem> LowerList { get; } = new();
        [Reactive]
        public bool IsExpanded { get; set; }

        public ICommand ToggleExpansionCommand { get; private set; }

        [Reactive]
        public FlaggedNavigationItem UpperSelection { get; set; }
            = new(NavigationPageLocation.Upper, false);

        [Reactive]
        public FlaggedNavigationItem LowerSelection { get; set; }
            = new(NavigationPageLocation.Lower, false);

        public NavigationListVm(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.WhenActivated(() => new[]{
                // bind listContent
                navigationService
                    .Pages
                    .Connect()
                    .Filter(m=>m.PageLocation == NavigationPageLocation.Upper)
                    .Transform(m=>new FlaggedNavigationItem(m, true))
                    .Sort(m=>m.Position.Value)
                    .Bind(UpperList)
                    .Subscribe(),
                navigationService
                    .Pages
                    .Connect()
                    .Filter(m=>m.PageLocation == NavigationPageLocation.Lower)
                    .Transform(m=>new FlaggedNavigationItem(m, true))
                    .Sort(m=>m.Position.Value)
                    .Bind(LowerList)
                    .Subscribe(),

                // handle selection
                Observable.CombineLatest(
                    this.WhenAnyValue(vm=>vm.UpperSelection)
                        .Do(m=>m.OnSelected())
                        .Pairwise(false)
                        .Where(m=>m.Current.Value.ChangedByUser),
                    this.WhenAnyValue(vm=>vm.LowerSelection)
                        .Do(m=>m.OnSelected())
                        .Pairwise(false)
                        .Where(m=>m.Current.Value.ChangedByUser),
                    (upper, lower)=>new
                    {
                        upper=new
                        {
                            previous = upper.Previous.Value,
                            current = upper.Current.Value,
                        },
                        lower = new
                        {
                            previous = lower.Previous.Value,
                            current = lower.Current.Value
                        }
                    }
                )
                    .Select(p =>
                    {
                        FlaggedNavigationItem upper = p.upper.current;
                        FlaggedNavigationItem lower = p.lower.current;
                        FlaggedNavigationItem result = null;
                        // upper <=> lower
                        if (upper.HasContent && lower.HasContent)
                        {
                            result = upper.TimeStamp > lower.TimeStamp
                                ? upper
                                : lower;
                        }
                        // same => same
                        else if (upper.HasContent)
                            result = upper;
                        else if (lower.HasContent)
                            result = lower;
                        // deselect
                        else if (!lower.HasContent && !upper.HasContent)
                        {
                            var previousUpper = p.upper.previous;
                            var previousLower = p.lower.previous;

                            result = previousUpper.TimeStamp > previousLower.TimeStamp
                                ? previousUpper
                                : previousLower;
                        }
                        else
                            Debugger.Break();
                        return result;
                    })
                    .Catch((Exception e) =>
                    {
                        Debugger.Break();
                        return Observable.Never<FlaggedNavigationItem>();
                    })
                    .Subscribe(m =>
                    {
                        var upper = 
                            m.PageLocation == NavigationPageLocation.Upper
                            ? m
                            : new FlaggedNavigationItem(NavigationPageLocation.Upper, false);
                        var lower= 
                            m.PageLocation == NavigationPageLocation.Lower
                            ? m
                            : new FlaggedNavigationItem(NavigationPageLocation.Lower, false);
                        if(upper != UpperSelection)
                            UpperSelection = upper;
                        if(lower != LowerSelection)
                            LowerSelection = lower;
                    }),

                Observable.Merge(
                        this.WhenAnyValue(vm=>vm.UpperSelection),
                        this.WhenAnyValue(vm => vm.LowerSelection)
                    )
                    .Select(m=>m.Page)
                    .WhereNotNull()
                    .BindTo(navigationService, srv=>srv.CurrentPage),

                this.WhenAnyValue(vm=>vm._navigationService.CurrentPage)
                    .Do(_=>{})
                    .WhereNotNull()
                    .Subscribe(selection =>
                    {
                        if (selection.PageLocation == NavigationPageLocation.Upper)
                        {
                            if (this.UpperSelection.Page != selection)
                                this.UpperSelection = this.UpperList.First(m => m.Page == selection);
                        }
                        else
                        {
                            if (this.LowerSelection.Page != selection)
                                this.LowerSelection = this.LowerList.First(m => m.Page == selection);
                        }
                    }),
                    
                
                // commands
                ReactiveCommandHelper.Create(
                    () =>IsExpanded = !IsExpanded,
                    cmd=>ToggleExpansionCommand = cmd),



            });
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
