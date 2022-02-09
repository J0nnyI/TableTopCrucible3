using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;
// required for DI
// ReSharper disable SuggestBaseTypeForParameterInConstructor

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemViewer
    {

    }
    public class ItemViewerVm:ReactiveObject, IActivatableViewModel, IItemViewer
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
}
