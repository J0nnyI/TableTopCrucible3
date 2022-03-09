using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
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
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public ITagManager TagManager { get; set; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool FluentModeEnabled { get; set; } = true;

    [Reactive]
    public bool TagDeletionEnabled { get; set; } = true;

    [Reactive] //if true, the next tag will be opened in edit mode
    internal bool FluentModeActive { get; set; }

    private SourceList<ITagEditorChip> _chipSourceList = new();
    private ReadOnlyObservableCollection<ITagEditorChip> _chipList;
    internal ReadOnlyObservableCollection<ITagEditorChip> ChipList => _chipList;
    private CancellationTokenSource chipUpdateCancellationToken = new();

    public TagEditorVm(IStorageController storageController, ITagView tagView, INotificationService notificationService)
    {
        ITagEditorChip addChip = null;
        this.WhenActivated(() =>
        {
            // this appends the + button

            if (addChip is null)
            {
                addChip = Locator.Current.GetService<ITagEditorChip>()!;
                addChip.Init(null, TagManager, tagView.Data, FluentModeActive
                    ? TagEditorWorkMode.Edit
                    : TagEditorWorkMode.View, TagDeletionEnabled);
                addChip.TagAdded.Take(1).Subscribe(_ => FluentModeActive = FluentModeEnabled);
                _chipSourceList.Add(addChip);
            }

            FluentModeActive = false;

            return new[]
            {
                this.WhenAnyValue(vm => vm.TagManager)
                    .Do(_ => IsBusy = true)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .Select(tagManager => tagManager?.Tags ?? Observable.Never(Enumerable.Empty<FractionTag>()))
                    .Switch()
                    .Retry(3)
                    .Subscribe(tags =>
                    {
                        chipUpdateCancellationToken.Cancel();
                        chipUpdateCancellationToken = new();

                        try
                        {
                            var chipsToRemove = new List<ITagEditorChip>();
                            var chipsToAdd = new List<ITagEditorChip>();
                            var unprocessedTags = tags.ToList();
                            foreach (var chip in _chipSourceList.Items)
                            {
                                if (chip.DisplayMode == TagEditorDisplayMode.New)
                                    continue;
                                var fTag = unprocessedTags
                                    .RemoveWhere(fTag => fTag.Tag == chip.SourceTag)
                                    .FirstOrDefault();
                                if (fTag is null)
                                    chipsToRemove.Add(chip);
                                else
                                    chip.Distribution = fTag.Distribution;
                                chipUpdateCancellationToken.Token.ThrowIfCancellationRequested();
                            }

                            chipsToAdd.AddRange(unprocessedTags.Select(fractionTag =>
                            {
                                var chip = Locator.Current.GetService<ITagEditorChip>()!;
                                chip.Init(fractionTag.Tag, TagManager, tagView.Data, TagEditorWorkMode.View,
                                    TagDeletionEnabled, fractionTag.Distribution);
                                chipUpdateCancellationToken.Token.ThrowIfCancellationRequested();
                                return chip;
                            }));

                            _chipSourceList.Edit(updater =>
                            {
                                updater.RemoveMany(chipsToRemove);
                                updater.AddRange(chipsToAdd);
                            });
                        }
                        catch (OperationCanceledException)
                        {
                            notificationService.AddInfo("Selection Changed before Tags could be updated", null);
                        }
                        catch (Exception e)
                        {
                            notificationService.AddError(e, "Tags could not be updated");
                            Debugger.Break();
                        }
                    }),
                this._chipSourceList
                .Connect()
                .Sort()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _chipList)
                .Subscribe(state=> IsBusy = false),
                // .ObserveOn(RxApp.MainThreadScheduler)
                // .SubscribeOn(RxApp.MainThreadScheduler)
                // .Subscribe(
                //     changes =>
                //     {
                //         changes.chipsToAdd
                //             .ToList()
                //             .ForEach(x => ChipList.Insert(x.index, x.chip));
                //         ChipList.RemoveMany(changes.chipsToRemove);
                //     },
                //     e =>
                //     {
                //         Debugger.Break();
                //         notificationService.AddError(e, "Tags could not be added");
                //     }),
            };
        });
    }
}