using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Domain.Library.Wpf.Services;

// required for DI
// ReSharper disable SuggestBaseTypeForParameterInConstructor

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

[Transient]
public interface IItemViewer
{
}

public class ItemViewerVm : ReactiveObject, IActivatableViewModel, IItemViewer
{
    public ITabContainer TabContainer { get; }
    public IItemViewerHeader Header { get; }
    public ViewModelActivator Activator { get; } = new();

    public ItemViewerVm(
        ITabContainer tabContainer,
        IItemViewerHeader header,
        ILibraryService libraryService,
        IItemFileList fileList,
        IItemGallery itemGallery,
        IItemDataViewer dataViewer,
        IItemModelViewer modelViewer
    )
    {
        TabContainer = tabContainer;
        tabContainer.Init(libraryService);
        Header = header;
        libraryService.AddTab(modelViewer, fileList, itemGallery, dataViewer);
    }
}