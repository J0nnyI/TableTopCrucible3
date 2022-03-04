﻿using System;
using System.Reactive.Disposables;
using DynamicData;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.Models;

public interface ITagMultiSourceProvider
{
    public IObservableList<ITagSourceProvider> SubProviders { get; }
}

public class MultiTagSource : ITagMultiSourceProvider, IDisposable
{
    public IObservableList<ITagSourceProvider> SubProviders { get; }
    private CompositeDisposable _disposables = new();

    public void Dispose()
        => _disposables.Dispose();

    public MultiTagSource(IObservableList<Item> items)
    {
        SubProviders = items
            .Connect()
            .Transform(item => new TagSource(item) as ITagSourceProvider)
            .AsObservableList()
            .DisposeWith(_disposables);
    }
}