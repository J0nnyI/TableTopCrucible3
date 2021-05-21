using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(DirectoryCardVM))]
    public interface IDirectoryCard
    {

    }
    public class DirectoryCardVM : ReactiveObject, IActivatableViewModel, IDirectoryCard
    {
        [Reactive]
        internal SingleLineDescription Description { get; set; }
        [Reactive]
        internal DirectoryPath DirectoryPath { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public DirectoryCardVM()
        {
            
        }
        
        public void UpdateDirectoryPath(DirectoryPath newPath)
        {
            Description = SingleLineDescription.From(newPath.GetDirectoryName().Value);
            DirectoryPath = newPath;
        }
    }
}
