using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.WPF.ViewModels;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using TableTopCrucible.Data.Models.Sources;

using DirectoryPathVT = TableTopCrucible.Core.ValueTypes.DirectoryPath;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(DirectoryCardVM))]
    public interface IDirectoryCard
    {
        SourceDirectoryId Id { get; }
        string Name { get; }

        IDirectoryCard SetSourceModel(SourceDirectory model);
        SourceDirectory ToModel();
    }
    public class DirectoryCardVM : ReactiveValidationObject, IActivatableViewModel, IDirectoryCard, IValidatableViewModel, INotifyDataErrorInfo
    {
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string Directory { get; set; }
        [Reactive]
        public string ThumbnailDirectory { get; set; }

        public SourceDirectoryId Id { get; private set; } = SourceDirectoryId.New();
        public IObservable<Unit> OnDirectorySelected { get; }
        public Interaction<string, bool> UserConfirmationInteraction { get; } = new Interaction<string, bool>();
        [Reactive]
        public SourceDirectory? OriginalData { get; private set; }
        private readonly ISourceDirectoryService _sourceDirectoryService;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public IEditSelector EditSelector { get; }

        public DirectoryCardVM(
            IEditSelector editSelector,
            ISourceDirectoryService sourceDirectoryService)
        {
            DirectoryPathVT.RegisterValidator(this, vm => vm.Directory, true);
            DirectoryPathVT.RegisterValidator(this, vm => vm.ThumbnailDirectory, false);
            EditSelector = editSelector;
            _sourceDirectoryService = sourceDirectoryService;

            this.WhenActivated(disposables =>
            {
                EditSelector.SetCommands(
                    revertChanges: ReactiveCommand.Create(
                        () =>
                        {
                            EditSelector.EditModeEnabled = false;
                            Name = OriginalData?.Name?.Value;
                            Directory = OriginalData?.Directory?.Value;
                            ThumbnailDirectory = OriginalData?.ThumbnailPath?.Value;
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
                            this.UserConfirmationInteraction.Handle("Do you really want to remove this directory form the registration?")
                                .Take(1)
                                .Where(x => x == true)
                                .Subscribe(_ =>
                                {
                                    editSelector.EditModeEnabled = false;// triggers the removal from the temporary list
                                    this._sourceDirectoryService.RemoveSourceDirectory(this.Id);
                                });

                        })
                    );

            });
        }
        public void UpdateDirectoryPath(DirectoryPathVT newPath)
        {
            Name = newPath.GetDirectoryName().Value;
            Directory = newPath.Value;
            ThumbnailDirectory = (newPath + DirectoryName.From("Thumbnails")).Value;
        }
        public IDirectoryCard SetSourceModel(SourceDirectory model)
        {
            this.Name = model.Name.Value;
            this.Directory = model.Directory.Value;
            this.Id = model.Id;
            this.ThumbnailDirectory = model.ThumbnailPath.Value;
            this.OriginalData = model;
            return this;
        }
        public SourceDirectory ToModel()
            => new SourceDirectory(Id, DirectoryPathVT.From(Directory), DirectoryPathVT.From(ThumbnailDirectory), DirectorySetupName.From(Name));

    }
}
