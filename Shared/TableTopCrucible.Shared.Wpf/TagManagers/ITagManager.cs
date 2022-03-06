using System;
using System.Collections.Generic;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public interface ITagManager : IDisposable
{
    public void Add(Tag tag);
    public void Remove(Tag tag);
    public void Replace(Tag oldTag, Tag newTag);
    public IObservable<IEnumerable<FractionTag>> Tags { get; }
}