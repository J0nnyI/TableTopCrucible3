using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

using DynamicData;
using DynamicData.Binding;

using MoreLinq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Infrastructure.Views.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;


[Transient]
public interface ITagEditor
{
    ITagCollection SelectedTags { get; set; }
    public bool FluentModeEnabled { get; set; }
}

public class TagEditorVm : ReactiveObject, IActivatableViewModel, ITagEditor
{
    private readonly IStorageController _storageController;
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public ITagCollection SelectedTags { get; set; }

    [Reactive]
    public bool FluentModeEnabled { get; set; } = true;

    [Reactive]
    public bool TagDeletionEnabled { get; set; } = true;

    [Reactive] //if true, the next tag will be opened in edit mode
    public bool FluentModeActive { get; set; } = false;

    public ObservableCollectionExtended<ITagEditorChip> TagList { get; } = new();

    public TagEditorVm(IStorageController storageController, ITagView tagView)
    {
        _storageController = storageController;

        this.WhenActivated(() =>
        {
            // this appends the + button
            var addChip = Locator.Current.GetService<ITagEditorChip>()!;
            addChip.Init(null, SelectedTags, tagView.Data, !FluentModeActive, FluentModeActive, TagDeletionEnabled);
            addChip.TagAdded.Take(1).Subscribe(_ => FluentModeActive = FluentModeEnabled);
            TagList.Add(addChip);
            FluentModeActive = false;
            var appendList = new SourceList<ITagEditorChip>();
            appendList.Add(addChip);
            return new[]
            {
                this.WhenAnyValue(vm => vm.SelectedTags)
                    .Select(tags => tags?.Connect() ?? Observable.Never(ChangeSet<Tag>.Empty))
                    .Switch()
                    .Transform(tag =>
                    {
                        // this converts existing tags to a vm
                        var chip = Locator.Current.GetService<ITagEditorChip>()!;
                        chip.Init(tag, SelectedTags, tagView.Data, false, false, TagDeletionEnabled);
                        return chip;
                    })
                    .Or(appendList.Connect())
                    .DisposeMany()
                    .StartWithEmpty()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(TagList)
                    .Subscribe(_ =>
                    {
                        // this appends the + button
                        //var chip = Locator.Current.GetService<ITagEditorChip>()!;
                        //chip.Init(null, SelectedTags, tagView.Data, !FluentModeActive,FluentModeActive, TagDeletionEnabled);
                        //chip.TagAdded.Take(1).Subscribe(_=>FluentModeActive = FluentModeEnabled);
                        //TagList.Add(chip);
                        //FluentModeActive = false;
                    }),

                new ActOnLifecycle(null, () => TagList.Clear()),
                appendList
            };
        });
    }

}