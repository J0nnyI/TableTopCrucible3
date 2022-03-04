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
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public enum TagEditorDisplayMode
    {
        New,
        Existing
    }
    public enum TagEditorWorkMode
    {
        Edit,
        View
    }
    [Transient]
    public interface ITagEditorChip
    {
        IObservable<Tag> TagAdded { get; }
        TagEditorDisplayMode DisplayMode { get; }
        public Tag SourceTag { get; }

        void Init(
            Tag sourceTag,
            ITagCollection selectedTags,
            IObservableList<Tag> availableTags,
            TagEditorWorkMode workMode,
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
        public IObservable<bool> AreTagsAvailableChanges { get; private set; }
        private readonly Subject<Tag> _tagAdded = new();
        public IObservable<Tag> TagAdded => _tagAdded.AsObservable();
        public bool DeletionEnabled { get; private set; }
        [Reactive]
        public bool IsDropDownOpen{ get; set; } = false;
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddTagCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ToggleDropDown { get; private set; }
        public Interaction<Unit, Unit> FocusEditorInteraction { get; } = new();
        public Interaction<Unit, Unit> UnfocusEditorInteraction { get; } = new();

        public Tag SourceTag { get; private set; }

        [Reactive]
        public string EditedTag { get; set; }
        [Reactive]
        public string SelectedTag { get; set; }
        [Reactive]
        public TagEditorWorkMode WorkMode { get; set; }
        [Reactive]
        public Fraction BackgroundProgress { get; set; }//= (Fraction).5;

        public TagEditorDisplayMode DisplayMode
            => SourceTag is null
            ? TagEditorDisplayMode.New
            : TagEditorDisplayMode.Existing;

        public TagEditorChipVm(IStorageController storageController)
        {
            _storageController = storageController;

            this.WhenActivated(() =>
            {
                var availableTags = this.WhenAnyValue(vm => vm.WorkMode)
                    .Select(editMode => editMode == TagEditorWorkMode.Edit
                        ? _availableTagsSource?.Connect()
                        : Observable.Empty<IChangeSet<Tag>>())
                    .Switch()
                    .Filter(this.WhenAnyValue(vm => vm.EditedTag).Select(filter =>
                        new Func<Tag, bool>(tag => tag.Value.ToLower().Contains(filter.ToLower()))));
                this.AreTagsAvailableChanges = availableTags
                    .Top(1)
                    .ToCollection()
                    .Select(x => x.Count > 0);
                return new[]
                {
                    this.WhenAnyValue(vm=>vm.WorkMode)
                        .Subscribe(workMode =>
                            this.IsDropDownOpen = (WorkMode == TagEditorWorkMode.Edit)),
                    this.ToggleDropDown = ReactiveCommand.Create(
                        () => { IsDropDownOpen =!IsDropDownOpen; },// remove return type
                        this.WhenAnyValue(v=>v.WorkMode)
                            .Select(workMode=>workMode == TagEditorWorkMode.Edit)),

                    // validation
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
                                        DisplayMode == TagEditorDisplayMode.New
                                            ? 1
                                            : 2);
                                }).StartWith(true),
                        "The tag has already been added"),

                    // bind tag suggestions
                    availableTags
                        .Transform(tag=>tag.Value)
                        .Sort((tagA, tagB) => // move the sourcetag to the top of the list
                        {
                            if((Tag)tagA == this.SourceTag)
                                return -1;
                            if((Tag)tagB == this.SourceTag)
                                return 1;
                            return tagA.CompareTo(tagB);
                        })
                        .Top(SettingsHelper.MaxTagCountInDropDown)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Bind(out _availableTags)
                        .Subscribe(),

                    // commands
                    RemoveCommand = ReactiveCommand.Create(Remove),
                    SaveCommand = ReactiveCommand.Create(
                        Confirm,
                        this.NoErrorsChanges()),
                    AddTagCommand = ReactiveCommand.Create(() =>
                    {
                        WorkMode = TagEditorWorkMode.Edit;
                        FocusEditorInteraction.Handle().Take(1).Subscribe();
                    }),
                };
            });
        }
        public void Init(
            Tag sourceTag,
            ITagCollection selectedTags,
            IObservableList<Tag> availableTags,
            TagEditorWorkMode workMode,
            bool deletionEnabled = true)
        {
            _selectedTags = selectedTags;
            _availableTagsSource = availableTags;
            DeletionEnabled = deletionEnabled;
            SourceTag = sourceTag;
            WorkMode = workMode;
            SelectedTag = EditedTag = sourceTag?.Value ?? string.Empty;
        }

        public void Revert()
        {
            EditedTag = SourceTag?.Value ?? string.Empty;
            WorkMode = TagEditorWorkMode.View;
        }

        public void Confirm()
        {
            if (HasErrors)
                return;
            EditTag();
            EditedTag = SourceTag?.Value ?? string.Empty;
            WorkMode = TagEditorWorkMode.View;
            _unfocusEditor();
        }

        public void Remove()
        {
            if (DisplayMode == TagEditorDisplayMode.New || WorkMode == TagEditorWorkMode.Edit)
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
            if (DisplayMode == TagEditorDisplayMode.New)
            {
                _selectedTags.Add((Tag)EditedTag);
                _tagAdded.OnNext((Tag)EditedTag);
            }
            else
            {
                _selectedTags.Replace(SourceTag, (Tag)EditedTag);
            }

            _storageController.AutoSave();
        }

        public void EnterEditMode()
        {
            if (WorkMode == TagEditorWorkMode.Edit)
                return;
            WorkMode = TagEditorWorkMode.Edit;
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
