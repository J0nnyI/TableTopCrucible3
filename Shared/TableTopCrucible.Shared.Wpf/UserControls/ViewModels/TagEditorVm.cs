using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Views.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

[Transient]
public interface ITagEditor
{
    ITagManager TagManager { get; set; }
    bool FluentModeEnabled { get; set; }
    bool TagDeletionEnabled { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class TagEditorVm : ReactiveObject, IActivatableViewModel, ITagEditor
{
    private readonly IStorageController _storageController;
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public ITagManager TagManager { get; set; }

    [Reactive]
    public bool FluentModeEnabled { get; set; } = true;

    [Reactive]
    public bool TagDeletionEnabled { get; set; } = true;

    [Reactive] //if true, the next tag will be opened in edit mode
    internal bool FluentModeActive { get; set; }

    internal ObservableCollection<ITagEditorChip> ChipList { get; } = new();

    public TagEditorVm(IStorageController storageController, ITagView tagView)
    {
        _storageController = storageController;

        ITagEditorChip addChip = null;
        this.WhenActivated(() =>
        {
            // this appends the + button
            
            if(addChip is null){
                addChip = Locator.Current.GetService<ITagEditorChip>()!;
                addChip.Init(null, TagManager, tagView.Data, FluentModeActive
                    ? TagEditorWorkMode.Edit
                    : TagEditorWorkMode.View, TagDeletionEnabled);
                addChip.TagAdded.Take(1).Subscribe(_ => FluentModeActive = FluentModeEnabled);
                ChipList.Add(addChip);
            }

            FluentModeActive = false;

            return new[]
            {
                this.WhenAnyValue(vm => vm.TagManager)
                    .Select(tagManager => tagManager?.Tags ?? Observable.Never(Enumerable.Empty<FractionTag>()))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        _tags =>
                        {
                            var tags = _tags.ToArray();

                            ChipList.RemoveWhere(chip =>
                                chip.DisplayMode != TagEditorDisplayMode.New &&
                                tags.Select(fTag => fTag.Tag)
                                    .ContainsNot(chip.SourceTag));

                            foreach (var fractionTag in tags)
                            {
                                var existingChip =
                                    ChipList.FirstOrDefault(chip => chip.SourceTag == fractionTag.Tag);
                                if (existingChip is not null) // just update the distribution and keep the vm
                                    existingChip.Distribution = fractionTag.Distribution;
                                else // create a new vm
                                {
                                    var chip = Locator.Current.GetService<ITagEditorChip>()!;
                                    chip.Init(fractionTag.Tag, TagManager, tagView.Data, TagEditorWorkMode.View,
                                        TagDeletionEnabled, fractionTag.Distribution);

                                    //find the proper location without changing the original list
                                    var newIndex = ChipList
                                        .Append(chip)
                                        .OrderBy((a, b) 
                                            => a.DisplayMode == TagEditorDisplayMode.New
                                                ? 1
                                                : b.DisplayMode == TagEditorDisplayMode.New
                                                ? -1
                                                : a.SourceTag.CompareTo(b.SourceTag)
                                        )
                                        .IndexOf(chip);

                                    //insert it into the list
                                    ChipList.Insert(newIndex, chip);
                                }
                            }
                        }),
            };
        });
    }
}