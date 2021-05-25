using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

using Splat;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

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

        private SourceCache<IDirectoryCard, SourceDirectoryId> _temporaryDirectoryCards { get; }
            = new SourceCache<IDirectoryCard, SourceDirectoryId>(card => card.Id);
        public DirectoryListVM(ISourceDirectoryService sourceDirectoryService)
        {
            this.WhenActivated(disposables =>
            {
                AddDirectory = ReactiveCommand.Create(
                    () => {
                        var card = Locator.Current.GetService<IDirectoryCard>();
                        card.SetEditMode(true);
                        card.EditModeChanges
                            .Where(x => x == false)
                            .Take(1)
                            .Subscribe(_ => _temporaryDirectoryCards.RemoveKey(card.Id));
                        _temporaryDirectoryCards.AddOrUpdate(card);
                    }
                );


                sourceDirectoryService.Directories
                    .Connect()
                    .Transform(model =>
                        Locator.Current.GetService<IDirectoryCard>()
                            .SetSourceModel(model)
                    ).FullJoin(
                        _temporaryDirectoryCards.Connect(),
                        x => x.Id,
                        (saved, temp) => saved.HasValue ? saved.Value : temp.Value
                    ) // basically a concat
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
