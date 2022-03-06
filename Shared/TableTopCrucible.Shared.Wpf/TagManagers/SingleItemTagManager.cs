using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public class SingleItemTagManager : ReactiveObject, ITagManager
{
    private readonly CompositeDisposable _disposables = new();
    public void Dispose() => _disposables.Dispose();

    [ObservableAsProperty]
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public Item CurrentItem { get; }

    public SingleItemTagManager(IObservable<Item> itemChanges)
    {
        Tags = itemChanges
            .Select(item =>
                ObservableListEx.Transform<Tag, FractionTag>(item
                        .Tags
                        .Connect(), tag => FractionTag.From(tag, null))
            )
            .Switch()
            .ToCollection();

        itemChanges
            .ToPropertyEx(this, t => t.CurrentItem)
            .DisposeWith(_disposables);
    }

    public void Add(Tag tag)
        => CurrentItem.Tags.Add(tag);

    public void Remove(Tag tag)
        => CurrentItem.Tags.Remove(tag);

    public void Replace(Tag oldTag, Tag newTag)
        => CurrentItem.Tags.Replace(oldTag, newTag);

    public IObservable<IEnumerable<FractionTag>> Tags { get; }
}