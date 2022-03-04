using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Domain.Library.Wpf.Services;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

[Transient]
public interface IItemViewerHeader
{
}

public class ItemViewerHeaderVm : ReactiveObject, IItemViewerHeader, IActivatableViewModel
{
    public IIconTabStrip TabStrip { get; }
    public ViewModelActivator Activator { get; } = new();

    public IObservable<int> SelectionCountChanges { get; }
    public IObservable<bool> ShowItemCountChanges { get; }
    public IObservable<bool> EditableChanges { get; }

    private readonly BehaviorSubject<bool> _editMode = new(false);
    public IObservable<bool> EditModeChanges => _editMode;
    public ReactiveCommand<Unit, Unit> EditNameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RevertNameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> SaveNameCommand { get; private set; }

    [Reactive]
    public string EditedName { get; set; } = string.Empty;
    public IObservable<string> TitleTooltipChanges { get; }

    public ItemViewerHeaderVm(ILibraryService libraryService, IIconTabStrip tabStrip)
    {
        TabStrip = tabStrip;
        TabStrip.Init(libraryService);
        SelectionCountChanges = libraryService
            .SelectedItems
            .CountChanged;
        ShowItemCountChanges = libraryService
            .SelectedItems
            .CountChanged
            .Select(count => count > 1);

        EditableChanges = libraryService
            .SingleSelectedItemChanges
            .Select(item => item is not null)
            .StartWith(false);

        TitleTooltipChanges = libraryService
            .SelectedItems
            .Connect()
            .ToCollection()
            .Select(items =>
                string.Join(
                    Environment.NewLine,
                    items.Take(10).Select(item => item.Name.Value)
                ) + (
                    items.Count > 10
                    ? Environment.NewLine + "..."
                    : string.Empty));
        this.WhenActivated(() => new[]
        {
            libraryService
                .SelectedItems
                .Connect()
                .ToCollection()
                .Select(items => string.Join(", ",
                                     items.Take(10).Select(item => item.Name.Value))
                                 + (items.Count > 10
                                     ? "..."
                                     : string.Empty))
                .BindTo(this, vm => vm.EditedName),
            this.EditNameCommand = ReactiveCommand.Create(
                () => _editMode.OnNext(true)),
            this.SaveNameCommand = ReactiveCommand.Create(
                () =>
                {
                    _editMode.OnNext(false);
                    libraryService.SingleSelectedItem.Name = EditedName;
                }),
            this.RevertNameCommand = ReactiveCommand.Create(
                () =>
                {
                    _editMode.OnNext(false);
                    EditedName = libraryService.SingleSelectedItem.Name.Value;
                }),
        });
    }
}