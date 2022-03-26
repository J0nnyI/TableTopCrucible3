using System;
using System.Collections.Generic;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Shared.Wpf.Models.TagEditor;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public interface ITagManager : IDisposable
{
    public void Add(Tag tag);
    public void Remove(Tag tag);
    public void Replace(Tag oldTag, Tag newTag);
    public IObservable<DisplayMode> DisplayModeChanges { get; }
    public IObservable<IEnumerable<FractionTag>> Tags { get; }
}