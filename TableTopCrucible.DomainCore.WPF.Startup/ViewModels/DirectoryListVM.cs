using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

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

        private readonly SourceList<IDirectoryCard> _items = new SourceList<IDirectoryCard>();
        public IObservableList<IDirectoryCard> Items => _items;

        public DirectoryListVM()
        {


        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public IDirectoryCard TemporaryDirectoryCard { get; }
        public ValidationContext ValidationContext { get; } = new ValidationContext();

        public DirectoryListVM(IDirectoryCard temporaryDirectoryCard)
        {
            TemporaryDirectoryCard = temporaryDirectoryCard;
        }
    }
}
