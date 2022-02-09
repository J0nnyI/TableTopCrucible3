using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Services;
using TableTopCrucible.Shared.Wpf.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    [Transient]
    public interface IItemModelViewer : ITabPage
    {
    }

    public class ItemModelViewerVm : ReactiveObject, IItemModelViewer, IActivatableViewModel
    {
        public ReactiveCommand<Unit, Unit> GenerateThumbnailCommand { get; }
        public ItemModelViewerVm(
            IModelViewer modelViewer,
            IFileRepository fileRepository,
            ILibraryService libraryService)
        {
            ModelViewer = modelViewer;

            _isSelectable = libraryService
                .SelectedItems
                .CountChanged
                .Select(count => count > 0)
                .ToProperty(this, vm => vm.IsSelectable);

            this.WhenActivated(() => new[]
            {
                libraryService.SingleSelectedItemChanges
                    .Select(item=>item.WhenAnyValue(i=>i.FileKey3d))
                    .Switch()
                    .Select(item =>
                        fileRepository
                            .Watch(item)
                            .ToCollection()
                            .Select(files =>
                                new
                                {
                                    item,
                                    files
                                }))
                    .Switch()
                    .OutputObservable(out var filesChanges)
                    .Select(x => x.files.FirstOrDefault()?.Path)
                    .Select(ModelFilePath.From)
                    .Catch((Exception e) =>
                    {
                        Debugger.Break();
                        return Observable.Never<ModelFilePath>();
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, vm => vm.ModelViewer.Model),

                filesChanges
                    .Select(x => x.item is null
                        ? (Message)"No Item selected"
                        : x.files.Any()
                            ? null
                            : (Message)"No Model found for this Item")
                    .BindTo(this, vm => vm.ModelViewer.PlaceholderMessage)
            });
        }

        public IModelViewer ModelViewer { get; }
        public ViewModelActivator Activator { get; } = new();


        #region ITabPage

        public Name Title => (Name)"Model Viewer";
        public PackIconKind SelectedIcon => PackIconKind.Rotate3d;
        public PackIconKind UnselectedIcon => PackIconKind.Rotate3dVariant;


        private readonly ObservableAsPropertyHelper<bool> _isSelectable;
        public bool IsSelectable => _isSelectable.Value;

        public SortingOrder Position => (SortingOrder)1;

        public void InitiatingClose()
        {
            this.ModelViewer.DisposeCurrentModel = true;
        }

        #endregion
    }
}