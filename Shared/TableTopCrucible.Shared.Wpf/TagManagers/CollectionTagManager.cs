using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Controller;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public class CollectionTagManager : ITagManager
{
    public void Add(Tag tag)
        => TagCollection.Add(tag);

    public void Remove(Tag tag)
        => TagCollection.Remove(tag);

    public void Replace(Tag oldTag, Tag newTag)
        => TagCollection.Replace(oldTag, newTag);
    IObservable<IEnumerable<FractionTag>> ITagManager.Tags => TagCollection
        .Connect()
        .ToCollection()
        .Select(tags => Enumerable.Select<Tag, FractionTag>(tags, tag=>FractionTag.From(tag,null)));

    public ITagCollection TagCollection { get; } = new TagCollection();
    public void Dispose()
    {
        TagCollection.Dispose();
    }
}