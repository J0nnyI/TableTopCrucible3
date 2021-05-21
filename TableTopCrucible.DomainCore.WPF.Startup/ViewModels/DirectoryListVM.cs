using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(DirectoryListVM))]
    public interface IDirectoryList
    {

    }
    public class DirectoryListVM : ReactiveObject, IDirectoryList, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public IDirectoryCard TemporaryDirectoryCard { get; }
        public DirectoryListVM(IDirectoryCard temporaryDirectoryCard)
        {
            TemporaryDirectoryCard = temporaryDirectoryCard;
        }
    }
}
