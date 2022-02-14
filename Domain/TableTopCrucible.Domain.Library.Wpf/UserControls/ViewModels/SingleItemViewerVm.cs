﻿using System;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Shared.Services;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

[Transient]
public interface ISingleItemViewer : IDisposable
{
}

public class SingleItemViewerVm : ReactiveObject, IActivatableViewModel, ISingleItemViewer
{
    private readonly IGalleryService _galleryService;
    private readonly ILibraryService _libraryService;
    private readonly CompositeDisposable _disposables = new();
    public IItemActions Actions { get; }
    public IItemFileList FileList { get; }
    public IItemGallery ItemGallery { get; }
    public IItemDataViewer DataViewer { get; }
    public IItemModelViewer ModelViewer { get; }
    public IItemViewerHeader ViewerHeader { get; }
    public ViewModelActivator Activator { get; } = new();

    public SingleItemViewerVm(
        IItemActions actions,
        IItemFileList fileList,
        IItemGallery itemGallery,
        IItemDataViewer dataViewer,
        IItemModelViewer modelViewer,
        IItemViewerHeader viewerHeader,
        IGalleryService galleryService,
        ILibraryService libraryService)
    {
        _galleryService = galleryService;
        _libraryService = libraryService;
        Actions = actions;
        FileList = fileList.DisposeWith(_disposables);
        ItemGallery = itemGallery;
        DataViewer = dataViewer;
        ModelViewer = modelViewer;
        ViewerHeader = viewerHeader;
    }

    public void Dispose()
        => _disposables.Dispose();

    public void HandleFileDrop(FilePath[] filePaths)
    {
        var images =
            filePaths
                .Where(file => file.IsImage())
                .Select(img => img.ToImagePath())
                .ToArray();
        _galleryService.AddImagesToItem(_libraryService.SingleSelectedItem, images);
    }
}