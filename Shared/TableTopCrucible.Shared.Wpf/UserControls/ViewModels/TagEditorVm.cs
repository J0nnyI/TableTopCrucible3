using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Data;

using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Views.Services;
using TableTopCrucible.Shared.Wpf.Models.TagEditor;

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
    public bool IsBusy { get; set; } = false;//does not work

    [Reactive]
    public bool FluentModeEnabled { get; set; } = true;

    [Reactive]
    public bool TagDeletionEnabled { get; set; } = true;

    [Reactive] //if true, the next tag will be opened in edit mode
    internal bool FluentModeActive { get; set; }

    /// <summary>
    /// represents the tags as Chip VM<br/>
    /// this is not directly bound to the tag manager but instead filled and updated via subscription. <br/>
    /// This is probably the cause for the sorting issues
    /// </summary>
    private SourceList<ITagEditorChip> _chipSourceList = new();
    /// <summary>
    /// ItemsSource for the ChipList
    /// </summary>
    private ReadOnlyObservableCollection<ITagEditorChip> _chipList;
    /// <summary>
    /// ItemsSource for the ChipList
    /// </summary>
    internal ReadOnlyObservableCollection<ITagEditorChip> ChipList => _chipList;
    private CancellationTokenSource chipUpdateCancellationToken = new();
    private object _updateLock = new();

    public TagEditorVm(IStorageController storageController, ITagView tagView, INotificationService notificationService)
    {
        this.WhenActivated(() =>
        {
            // this appends the + button

            FluentModeActive = false;

            return new[]
            {
                this.WhenAnyValue(vm => vm.TagManager)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .Select(tagManager => tagManager?.Tags ?? Observable.Never(Enumerable.Empty<FractionTag>()))
                    .Switch()
                    .Select(tags=>tags.OrderByDescending(tag=>tag).ToArray())
                    .Retry(3)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(tags =>
                    {
                        chipUpdateCancellationToken.Cancel();
                        chipUpdateCancellationToken = new();

                        try
                        {
                            lock (_updateLock) {
                                var removeList = new List<ITagEditorChip>();
                                var addList = new List<ITagEditorChip>();
                                int addChipIndex = -1;
                                int i=0;
                                foreach(var chip in _chipSourceList.Items)
                                {
                                    if(chip.SourceTag is null)
                                    {
                                        addChipIndex = i;
                                        continue;
                                    }
                                    chipUpdateCancellationToken.Token.ThrowIfCancellationRequested();
                                    var FractionTag = tags.ElementAtOrDefault(i);
                                    if(FractionTag is not null)
                                    {
                                        chip.Distribution = FractionTag.Distribution;
                                        chip.SourceTag = FractionTag.Tag;
                                    }
                                    else
                                        removeList.Add(chip);
                                    i++;
                                }

                                for (; i < tags.Count() - 1; i++)
                                {
                                    var tag = tags.ElementAt(i);
                                    var newChip = Locator.Current.GetService<ITagEditorChip>()!;
                                    newChip.Init(tag.Tag, TagManager, tagView.Data, WorkMode.View,
                                        TagDeletionEnabled, tag.Distribution);
                                    addList.Add(newChip);
                                    chipUpdateCancellationToken.Token.ThrowIfCancellationRequested();
                                }
                                if(removeList.Any() && addList.Any())
                                    throw new InvalidOperationException("this should not be possible");
                                _chipSourceList.Edit(editor =>
                                {
                                    if(chipUpdateCancellationToken.Token.IsCancellationRequested)
                                        return;

                                    if(addList.Any())
                                        editor.AddRange(addList);

                                    if(addChipIndex == -1)
                                    {
                                        var addChip = Locator.Current.GetService<ITagEditorChip>()!;
                                        addChip.Init(null, TagManager, tagView.Data, FluentModeActive
                                            ? WorkMode.Edit
                                            : WorkMode.View, TagDeletionEnabled);
                                        addChip.TagAdded.Take(1).Subscribe(_ => FluentModeActive = FluentModeEnabled);
                                        editor.Add(addChip);
                                    }
                                    else 
                                        editor.Move(addChipIndex,editor.Count-1);


                                    if(removeList.Any())
                                        editor.RemoveMany(removeList);
                                });
                            }
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
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _chipList)
                    .Subscribe()
            };
        });
    }
}