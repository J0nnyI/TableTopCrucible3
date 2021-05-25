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
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Services.Sources;

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
        public DirectoryListVM(ISourceDirectoryService sourceDirectoryService, IDirectoryCard temporaryDirectoryCard)
        {
            TemporaryDirectoryCard = temporaryDirectoryCard;
            this.WhenActivated(disposables =>
            {
                sourceDirectoryService.Directories
                    .Connect()
                    .Transform(model =>
                        Locator.Current.GetService<IDirectoryCard>()
                            .SetSourceModel(model)
                    ).Bind(out var directoryCards)
                    .Subscribe()
                    .DisposeWith(disposables);
                this.DirectoryCards = directoryCards;
            });

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public IDirectoryCard TemporaryDirectoryCard { get; }
        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}
