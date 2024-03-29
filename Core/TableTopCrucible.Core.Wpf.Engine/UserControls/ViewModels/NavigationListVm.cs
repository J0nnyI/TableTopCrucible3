﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
            PageLocation = page.PageLocation;
            ChangedByUser = changedByUser;
        }

        // used for deselection
        public FlaggedNavigationItem(NavigationPageLocation location, bool changedByUser)
        {
            PageLocation = location;
            ChangedByUser = changedByUser;
        }

        public DateTime TimeStamp { get; private set; } = DateTime.Now;
        public INavigationPage Page { get; }
        public PackIconKind? Icon => Page?.Icon;

        public Name Title => Page?.Title;

        public bool ChangedByUser { get; }

        public NavigationPageLocation? PageLocation { get; }

        public SortingOrder Position => Page?.Position;

        public bool HasContent => Page != null;
        public void OnSelected() => TimeStamp = DateTime.Now;

        public override string ToString()
            => $"{Title}  |  cbu:{ChangedByUser}  |  {PageLocation}";
    }

    [Transient]
    public interface INavigationList
    {
    }

    public class NavigationListVm : ReactiveObject, INavigationList, IActivatableViewModel
    {
        private readonly INavigationService _navigationService;
        private ObservableAsPropertyHelper<bool> _isExpanded;

        public NavigationListVm(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm._navigationService.IsNavigationExpanded)
                    .ToProperty(this, vm => vm.IsExpanded, out _isExpanded),

                // bind listContent
                navigationService
                    .Pages
                    .Connect()
                    .Filter(m => m.PageLocation == NavigationPageLocation.Upper)
                    .Transform(m => new FlaggedNavigationItem(m, true))
                    .Sort(m => m.Position.Value)
                    .Bind(UpperList)
                    .Subscribe(),
                navigationService
                    .Pages
                    .Connect()
                    .Filter(m => m.PageLocation == NavigationPageLocation.Lower)
                    .Transform(m => new FlaggedNavigationItem(m, true))
                    .Sort(m => m.Position.Value)
                    .Bind(LowerList)
                    .Subscribe(),

                // handle selection
                this.WhenAnyValue(vm => vm.UpperSelection)
                    .Do(m => m.OnSelected())
                    .Pairwise(false)
                    .Where(m => m.Current.Value.ChangedByUser).CombineLatest(this.WhenAnyValue(vm => vm.LowerSelection)
                            .Do(m => m.OnSelected())
                            .Pairwise(false)
                            .Where(m => m.Current.Value.ChangedByUser),
                        (upper, lower) => new
                        {
                            upper = new
                            {
                                previous = upper.Previous.Value,
                                current = upper.Current.Value
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
                        var upper = p.upper.current;
                        var lower = p.lower.current;
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
                        {
                            result = upper;
                        }
                        else if (lower.HasContent)
                        {
                            result = lower;
                        }
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
                        {
                            Debugger.Break();
                        }

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
                        var lower =
                            m.PageLocation == NavigationPageLocation.Lower
                                ? m
                                : new FlaggedNavigationItem(NavigationPageLocation.Lower, false);
                        if (upper != UpperSelection)
                            UpperSelection = upper;
                        if (lower != LowerSelection)
                            LowerSelection = lower;
                    }),

                this.WhenAnyValue(vm => vm.UpperSelection).Merge(this.WhenAnyValue(vm => vm.LowerSelection)
                    )
                    .Select(m => m.Page)
                    .WhereNotNull()
                    .BindTo(navigationService, srv => srv.ActiveWorkArea),

                this.WhenAnyValue(vm => vm._navigationService.ActiveWorkArea)
                    .WhereNotNull()
                    .Subscribe(selection =>
                    {
                        if (selection.PageLocation == NavigationPageLocation.Upper)
                        {
                            if (UpperSelection.Page != selection)
                                UpperSelection = UpperList.First(m => m.Page == selection);
                        }
                        else
                        {
                            if (LowerSelection.Page != selection)
                                LowerSelection = LowerList.First(m => m.Page == selection);
                        }
                    })
            });
        }

        public ObservableCollectionExtended<FlaggedNavigationItem> UpperList { get; } = new();
        public ObservableCollectionExtended<FlaggedNavigationItem> LowerList { get; } = new();
        public bool IsExpanded => _isExpanded.Value;

        [Reactive]
        public FlaggedNavigationItem UpperSelection { get; set; }
            = new(NavigationPageLocation.Upper, false);

        [Reactive]
        public FlaggedNavigationItem LowerSelection { get; set; }
            = new(NavigationPageLocation.Lower, false);

        public ViewModelActivator Activator { get; } = new();
    }
}