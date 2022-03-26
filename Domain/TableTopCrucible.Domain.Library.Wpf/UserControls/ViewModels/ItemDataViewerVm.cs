using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

[Transient]
public interface IItemDataViewer : ITabPage
{
}
public class ItemDataViewerVm : ReactiveObject, IItemDataViewer, IActivatableViewModel
{
    public ITagEditor TagEditor { get; }
    public ViewModelActivator Activator { get; } = new();

    public ItemDataViewerVm(ITagEditor tagEditor, ILibraryService libraryService)
    {
        TagEditor = tagEditor;
        _isSelectable = libraryService
            .SelectedItems
            .CountChanged
            .Select(count => count > 0)
            .ToProperty(this, vm => vm.IsSelectable);

        this.WhenActivated(() =>
        {
            TagEditor.TagManager = new MultiItemTagManager(libraryService.SelectedItems);

            return new IDisposable[]
            {
                TagEditor.TagManager,
            };
        });
    }

    #region ITabPage

    public Name Title => (Name)"Data";
    public PackIconKind SelectedIcon => PackIconKind.Database;
    public PackIconKind UnselectedIcon => PackIconKind.DatabaseOutline;


    private readonly ObservableAsPropertyHelper<bool> _isSelectable;
    public bool IsSelectable => _isSelectable.Value;
    public SortingOrder Position => (SortingOrder)2;

    public void BeforeClose()
    {
    }

    #endregion
}