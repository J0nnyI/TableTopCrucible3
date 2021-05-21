using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;
using DirectoryPathVT= TableTopCrucible.Core.ValueTypes.DirectoryPath;
using TableTopCrucible.Data.Models.Sources;
using ReactiveUI.Validation.Helpers;
using System.ComponentModel;
using TableTopCrucible.Core.WPF.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(DirectoryCardVM))]
    public interface IDirectoryCard
    {

    }
    public class DirectoryCardVM : ReactiveValidationObject, IActivatableViewModel, IDirectoryCard, IValidatableViewModel, INotifyDataErrorInfo
    {
        [Reactive]
        public string Description { get; set; }
        [Reactive]
        public string DirectoryPath { get; set; }
        public IObservable<Unit> OnDirectorySelected { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public IEditSelector EditSelector { get; }

        public DirectoryCardVM(IEditSelector editSelector)
        {
            DirectoryPathVT.RegisterValidator(this, vm => vm.DirectoryPath, true);
            EditSelector = editSelector;
        }

        public void UpdateDirectoryPath(DirectoryPathVT newPath)
        {
            Description = newPath.GetDirectoryName().Value;
            DirectoryPath = newPath.Value;
        }
        public SourceDirectory ToModel()
        {
            throw new NotImplementedException();
        }
    }
}
