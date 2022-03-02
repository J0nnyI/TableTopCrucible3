using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Core.Wpf.Helper;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface ITagEditorChip
    {
        IObservable<Tag> TagAdded { get; }
        bool IsNew { get; }

        void Init(
            Tag sourceTag,
            ITagCollection selectedTags,
            IObservableList<Tag> availableTags,
            bool addMode = false,
            bool editModeEnabled = false,
            bool deletionEnabled = true);
    }
    public class TagEditorChipVm : ReactiveValidationObject, IActivatableViewModel, ITagEditorChip
    {
        private readonly IStorageController _storageController;
        public ViewModelActivator Activator { get; } = new();

        private ITagCollection _selectedTags;
        private IObservableList<Tag> _availableTagsSource;
        private ReadOnlyObservableCollection<string> _availableTags;
        public ReadOnlyObservableCollection<string> AvailableTags => _availableTags;
        private readonly Subject<Tag> _tagAdded = new();
        public IObservable<Tag> TagAdded => _tagAdded.AsObservable();
        public bool DeletionEnabled { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddTagCommand { get; private set; }
        public Interaction<Unit, Unit> FocusEditorInteraction { get; } = new();
        public Interaction<Unit, Unit> UnfocusEditorInteraction { get; } = new();

        public TagEditorChipVm(IStorageController storageController)
        {
            _storageController = storageController;

            this.WhenActivated(() => new[]
            {
                this.ValidationRule(
                        vm => vm.EditedTag,
                        tag => !string.IsNullOrWhiteSpace(tag),
                        "The tag must not be empty"),

                this.ValidationRule(
                vm => vm.EditedTag,
                this.WhenAnyValue(vm => vm.EditedTag)
                    .CombineLatest(
                        _selectedTags?.Connect().ToCollection() ?? Observable.Empty<IReadOnlyCollection<Tag>>(),
                        (tag, tags) =>
                        {
                            return tags.Count(t => t.Value == tag) < (
                                IsNew
                                    ? 1
                                    : 2);
                        }).StartWith(true),
                "The tag has already been added"),

                this.WhenAnyValue(vm => vm.EditModeEnabled)
                    .Select(editMode => editMode
                        ? _availableTagsSource?.Connect()
                        : Observable.Empty<IChangeSet<Tag>>())
                    .Switch()
                    .Filter(this.WhenAnyValue(vm => vm.EditedTag).Select(filter =>
                        new Func<Tag, bool>(tag => tag.Value.ToLower().Contains(filter.ToLower()))))
                    .Top(SettingsHelper.MaxTagCountInDropDown)
                    .Transform(tag=>tag.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _availableTags)
                    .Subscribe(),
                RemoveCommand = ReactiveCommand.Create(Remove),
                SaveCommand = ReactiveCommand.Create(
                    Confirm,
                    this.NoErrorsChanges()),
                AddTagCommand = ReactiveCommand.Create(() =>
                {
                    AddMode = false;
                    EditModeEnabled = false;
                    FocusEditorInteraction.Handle().Take(1).Subscribe();
                }),
            });
        }
        public void Init(
            Tag sourceTag,
            ITagCollection selectedTags,
            IObservableList<Tag> availableTags,
            bool addMode = false,
            bool editModeEnabled = false,
            bool deletionEnabled = true)
        {
            _selectedTags = selectedTags;
            _availableTagsSource = availableTags;
            DeletionEnabled = deletionEnabled;
            SourceTag = sourceTag;
            AddMode = addMode;
            EditModeEnabled = editModeEnabled;
            EditedTag = sourceTag?.Value ?? string.Empty;

        }

        public Tag SourceTag { get; private set; }

        [Reactive]
        public string EditedTag { get; set; }

        [Reactive]
        public bool EditModeEnabled { get; set; }

        [Reactive]
        public bool AddMode { get; set; }

        public bool IsNew => SourceTag is null;

        public void Revert()
        {
            EditedTag = SourceTag?.Value ?? string.Empty;
            EditModeEnabled = false;
            AddMode = IsNew;
        }

        public void Confirm()
        {
            if (HasErrors)
                return ;
            EditTag();
            EditedTag = SourceTag?.Value ?? string.Empty;
            EditModeEnabled = false;
            AddMode = IsNew;
            _unfocusEditor();
        }

        public void Remove()
        {
            if (IsNew || EditModeEnabled)
                Revert();
            else
            {
                _selectedTags!.Remove(SourceTag);
                _storageController.AutoSave();
            }
            _unfocusEditor();
        }

        public void EditTag()
        {
            if (IsNew)
            {
                _selectedTags.Add((Tag)EditedTag);
                _tagAdded.OnNext((Tag)EditedTag);
            }
            else
                _selectedTags.Replace(SourceTag, (Tag)EditedTag);

            _storageController.AutoSave();
        }

        public void EnterEditMode()
        {
            if (EditModeEnabled)
                return;
            EditModeEnabled = true;
            _focusEditor();
        }

        private void _focusEditor()
        {
            FocusEditorInteraction.Handle().Take(1).Subscribe();
        }
        private void _unfocusEditor()
        {
            UnfocusEditorInteraction.Handle().Take(1).Subscribe();
        }
    }
}
