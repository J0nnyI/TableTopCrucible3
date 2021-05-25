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
using DirectoryPathVT = TableTopCrucible.Core.ValueTypes.DirectoryPath;
using TableTopCrucible.Data.Models.Sources;
using ReactiveUI.Validation.Helpers;
using System.ComponentModel;
using TableTopCrucible.Core.WPF.ViewModels;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using DynamicData;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(DirectoryCardVM))]
    public interface IDirectoryCard
    {
        SourceDirectoryId Id { get; }

        IDirectoryCard SetSourceModel(SourceDirectory model);
        SourceDirectory ToModel();
        void SetEditMode(bool isEnabled);

        public IObservable<bool> EditModeChanges { get; }
    }
    public class DirectoryCardVM : ReactiveValidationObject, IActivatableViewModel, IDirectoryCard, IValidatableViewModel, INotifyDataErrorInfo
    {
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string Directory { get; set; }

        public SourceDirectoryId Id { get; private set; } = SourceDirectoryId.New();
        public IObservable<Unit> OnDirectorySelected { get; }
        [Reactive]
        private SourceDirectory _originalData { get; set; }
        private readonly ISourceDirectoryService _sourceDirectoryService;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public IEditSelector EditSelector { get; }

        public IObservable<bool> EditModeChanges => this.WhenAnyValue(vm => vm.EditSelector.EditModeEnabled).DistinctUntilChanged();

        public DirectoryCardVM(
            IEditSelector editSelector,
            ISourceDirectoryService sourceDirectoryService)
        {
            DirectoryPathVT.RegisterValidator(this, vm => vm.Directory, true);
            EditSelector = editSelector;
            _sourceDirectoryService = sourceDirectoryService;

            this.WhenActivated(disposables =>
            {
                EditSelector.SetCommands(
                    revertChanges: ReactiveCommand.Create(
                        () =>
                        {
                            EditSelector.EditModeEnabled = false;
                            Name = _originalData.Name?.Value;
                            Directory = _originalData.Directory?.Value;
                        },
                        this._sourceDirectoryService
                            .Directories
                            .WatchValue(Id)
                            .Select(_ => true)
                            .StartWith(false)
                        ).DisposeWith(disposables),
                    saveChanges: ReactiveCommand.Create(
                        () =>
                        {
                            EditSelector.EditModeEnabled = false;
                            this._sourceDirectoryService.AddOrUpdateDirectory(this.ToModel());
                        }, this.IsValid())
                        .DisposeWith(disposables),
                    deleteChanges: ReactiveCommand.Create(
                        () =>
                        {
                            editSelector.EditModeEnabled = false;// triggers the removal from the temporary list
                            this._sourceDirectoryService.RemoveSourceDirectory(this.Id);
                        })
                    );

            });
        }
        public void UpdateDirectoryPath(DirectoryPathVT newPath)
        {
            Name = newPath.GetDirectoryName().Value;
            Directory = newPath.Value;
        }
        public IDirectoryCard SetSourceModel(SourceDirectory model)
        {
            this.Name = model.Name.Value;
            this.Directory = model.Directory.Value;
            this.Id = model.Id;
            this._originalData = model;
            return this;
        }
        public SourceDirectory ToModel()
            => new SourceDirectory(Id, DirectoryPathVT.From(Directory), null, DirectorySetupName.From(Name));

        public void SetEditMode(bool isEnabled)
            => EditSelector.EditModeEnabled = isEnabled;
    }
}
