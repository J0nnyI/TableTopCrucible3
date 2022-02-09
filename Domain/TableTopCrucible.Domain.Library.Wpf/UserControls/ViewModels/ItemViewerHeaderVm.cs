using System;
using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemViewerHeader
    {
    }

    public class ItemViewerHeaderVm : ReactiveObject, IItemViewerHeader, IActivatableViewModel
    {
        public IIconTabStrip TabStrip { get; }
        public ViewModelActivator Activator { get; } = new();

        public IObservable<string> TitleChanges { get; }
        public IObservable<int> SelectionCountChanges { get; }
        public IObservable<bool> ShowItemCount { get; }

        public ItemViewerHeaderVm(ILibraryService libraryService, IIconTabStrip tabStrip)
        {
            TabStrip = tabStrip;
            TabStrip.Init(libraryService);
            TitleChanges = libraryService
                .SelectedItems
                .Connect()
                .ToCollection()
                .Select(items => string.Join(", ", items.Select(item => item.Name.Value)));
            SelectionCountChanges = libraryService
                .SelectedItems
                .CountChanged;
            ShowItemCount = libraryService
                .SelectedItems
                .CountChanged
                .Select(count => count > 1);
        }
    }
}