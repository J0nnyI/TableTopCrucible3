using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

using Splat;

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(DirectoryListVM))]
    public interface IDirectoryList
    {

    }
    public class DirectoryListVM : ReactiveObject, IDirectoryList, IActivatableViewModel, IValidatableViewModel
    {
        [Reactive]
        public string Filter { get; set; }

        [Reactive]
        public ReadOnlyObservableCollection<IDirectoryCard> DirectoryCards { get; private set; }

        public Interaction<Unit, DirectoryPath> GetDirectoryFromUser { get; private set; } = new Interaction<Unit, DirectoryPath>();

        public DirectoryListVM(ISourceDirectoryService sourceDirectoryService)
        {
            this.WhenActivated(disposables =>
            {
                AddDirectory = ReactiveCommand.Create(
                    () =>
                    {
                        GetDirectoryFromUser.Handle(Unit.Default)
                            .Take(1)
                            .WhereNotNull()
                            .Subscribe(dir =>
                                sourceDirectoryService.AddOrUpdateDirectory(new SourceDirectory(dir))
                            );
                    }
                );


                sourceDirectoryService.Directories
                    .Connect()
                    .Transform(model =>
                        Locator.Current.GetService<IDirectoryCard>()
                            .SetSourceModel(model)
                    )
                    .Filter(
                        this.WhenAnyValue(vm => vm.Filter)
                            .ToFilter((IDirectoryCard vm, string filter)
                                => vm.Name?.Contains(filter ?? "") ?? true))
                    .Sort(vm => vm.Name)
                    .Bind(out var directoryCards)
                    .Subscribe()
                    .DisposeWith(disposables);
                this.DirectoryCards = directoryCards;
            });

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ValidationContext ValidationContext { get; } = new ValidationContext();
        public ReactiveCommand<Unit, Unit> AddDirectory { get; private set; }
    }
}
