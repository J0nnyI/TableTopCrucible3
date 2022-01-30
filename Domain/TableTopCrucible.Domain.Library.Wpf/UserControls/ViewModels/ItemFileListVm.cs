using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemFileList : IDisposable, ITabPage
    {
    }

    public class ItemFileListVm : ReactiveObject, IItemFileList, IActivatableViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        public ItemFileListVm(IFileRepository fileRepository, ILibraryService libraryService)
        {
            _isSelectable = libraryService
                .SingleSelectedItemChanges
                .Select(item => item is not null)
                .ToProperty(this, vm => vm.IsSelectable);
            this.WhenActivated(() => new[]
            {
                libraryService
                    .SingleSelectedItemChanges
                    .Select(item => item?.FileKey3d)
                    .Select(fileRepository.Watch)
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .OnItemAdded(file =>
                    {
                        Files.Clear();
                        Files.Add(file);
                    })
                    .Subscribe(),
            });
        }

        public ObservableCollection<FileData> Files { get; } = new();
        public ViewModelActivator Activator { get; } = new();

        public void Dispose()
            => _disposables.Dispose();

        #region ITabPage

        public Name Title => (Name)"Files";
        public PackIconKind SelectedIcon => PackIconKind.File;
        public PackIconKind UnselectedIcon => PackIconKind.FileOutline;

        private readonly ObservableAsPropertyHelper<bool> _isSelectable;
        public bool IsSelectable => _isSelectable.Value;

        public SortingOrder Position => (SortingOrder)4;

        #endregion
    }
}