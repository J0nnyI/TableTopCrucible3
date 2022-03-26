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
using System.Reactive.Disposables;
using System.ComponentModel;
using TableTopCrucible.Shared.Wpf.Models.TagEditor;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface ITagEditorChip : IComparable<ITagEditorChip>,IComparable, INotifyPropertyChanged
    {
        IObservable<Tag> TagAdded { get; }
        DisplayMode DisplayMode { get; }
        [Reactive]
        public Tag SourceTag { get; set; }
        public Fraction Distribution { get; set; }

        void Init(
            Tag sourceTag,
            ITagManager tagManager,
            IObservableList<Tag> availableTags,
            WorkMode workMode,
            bool deletionEnabled = true,
            Fraction distribution = null);
    }
    public class TagEditorChipVm : ReactiveValidationObject, IActivatableViewModel, ITagEditorChip
    {
        private readonly IStorageController _storageController;
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        private ITagManager _tagManager { get; set; }
        [Reactive]
        private IObservableList<Tag> _availableTagsSource { get; set; }
        private ReadOnlyObservableCollection<string> _availableTags;
        public ReadOnlyObservableCollection<string> AvailableTags => _availableTags;
        public IObservable<bool> AreTagsAvailableChanges { get; private set; }
        private readonly Subject<Tag> _tagAdded = new();
        public IObservable<Tag> TagAdded => _tagAdded.AsObservable();
        public bool DeletionEnabled { get; private set; }
        [Reactive]
        public bool IsDropDownOpen { get; set; } = false;
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddTagCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ToggleDropDown { get; private set; }
        public Interaction<Unit, Unit> FocusEditorInteraction { get; } = new();
        public Interaction<Unit, Unit> UnfocusEditorInteraction { get; } = new();

        [Reactive]
        public Tag SourceTag { get; set; }

        [Reactive]
        public string EditedTag { get; set; }
        [Reactive]
        public string SelectedTag { get; set; }
        [Reactive]
        public WorkMode WorkMode { get; set; } = WorkMode.View;
        /// <summary>
        /// used for complex view, determines the fill state of the background
        /// </summary>
        [Reactive]
        public Fraction Distribution { get; set; }//= (Fraction).5;

        //has to be set on init to be able to sort add and other displayModes
        [Reactive]
        public DisplayMode DisplayMode { get; private set; }

        public TagEditorChipVm(IStorageController storageController)
        {
            _storageController = storageController;

            this.WhenActivated(() =>
            {
                this.WhenAnyValue(vm => vm.EditedTag)
                    .Subscribe(tag => SelectedTag = null);
                var availableTags = this.WhenAnyValue(vm => vm.WorkMode)
                    .Select(editMode => editMode == WorkMode.Edit
                        && _availableTagsSource is not null
                        ? _availableTagsSource.Connect()
                        : Observable.Empty<IChangeSet<Tag>>())
                    .Switch()
                    .Filter(
                        this.WhenAnyValue(vm => vm.EditedTag)
                            .Select(filter =>
                                new Func<Tag, bool>(tag =>
                                    tag.Value.ToLower()
                                        .Contains(filter.ToLower()))
                            )
                    );
                this.AreTagsAvailableChanges = availableTags
                    .Top(1)
                    .ToCollection()
                    .Select(x => x.Count > 0);
                return new IDisposable[]
                {
                    Disposable.Create(() => {
                        this.Revert();
                        IsDropDownOpen=false;
                    }),

                    this.WhenAnyValue(
                        vm => vm._tagManager,
                        vm => vm.SourceTag,
                        (tagSource, sourceTag) =>
                            tagSource  is null || sourceTag is null
                                ? Observable.Return(sourceTag is null? DisplayMode.New:DisplayMode.Simple)
                                : tagSource.DisplayModeChanges)
                        .Switch()
                        .Subscribe(displayMode=>DisplayMode=displayMode),
                    this.WhenAnyValue(vm=>vm.WorkMode)
                        .Subscribe(workMode =>
                            this.IsDropDownOpen = (WorkMode == WorkMode.Edit)),
                    this.ToggleDropDown = ReactiveCommand.Create(
                        () => { IsDropDownOpen =!IsDropDownOpen; },// remove return type
                        this.WhenAnyValue(v=>v.WorkMode)
                            .Select(workMode=>workMode == WorkMode.Edit)
                            .ObserveOn(RxApp.MainThreadScheduler)),

                    // validation
                    this.ValidationRule(
                            vm => vm.EditedTag,
                            tag => !string.IsNullOrWhiteSpace(tag),
                            "The tag must not be empty"),
                    this.ValidationRule(
                        vm => vm.EditedTag,
                        this.WhenAnyValue(vm => vm.EditedTag)
                            .CombineLatest(
                                _tagManager.Tags ,
                                (tag, tags) =>
                                {
                                    return tags.Count(t => t.Tag.Value.ToLower() == tag.ToLower()) < (
                                        DisplayMode == DisplayMode.New
                                            ? 1
                                            : 2);
                                })
                            .StartWith(true),
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
                        this.NoErrorsChanges().ObserveOn(RxApp.MainThreadScheduler)),
                    AddTagCommand = ReactiveCommand.Create(() =>
                    {
                        WorkMode = WorkMode.Edit;
                        FocusEditorInteraction.Handle().Take(1).Subscribe();
                    }),
                };
            });
        }

        public void Init(
            Tag sourceTag,
            ITagManager tagManager,
            IObservableList<Tag> availableTags,
            WorkMode workMode,
            bool deletionEnabled = true,
            Fraction distribution = null)
        {
            _tagManager = tagManager ?? throw new ArgumentException(nameof(tagManager) + " must not be null");
            _availableTagsSource = availableTags;
            DeletionEnabled = deletionEnabled;
            SourceTag = sourceTag;
            WorkMode = workMode;
            SelectedTag = EditedTag = sourceTag?.Value ?? string.Empty;
            DisplayMode = sourceTag is null ? DisplayMode.New : DisplayMode.Simple;
            this.Distribution = distribution;
        }

        public void Revert()
        {
            EditedTag = SourceTag?.Value ?? string.Empty;
            WorkMode = WorkMode.View;
        }

        public void Confirm()
        {
            if (HasErrors)
                return;

            if (DisplayMode == DisplayMode.New)
            {
                _tagManager.Add((Tag)EditedTag);
                _tagAdded.OnNext((Tag)EditedTag);
                SelectedTag = null;
                EditedTag = string.Empty;
            }
            else if (SourceTag == (Tag)EditedTag
                && DisplayMode == DisplayMode.Fraction)
                _tagManager.Add(SourceTag);
            else
                _tagManager.Replace(SourceTag, (Tag)EditedTag);
            EditedTag = SourceTag?.Value ?? string.Empty;
            WorkMode = WorkMode.View;
            IsDropDownOpen = false;
            _unfocusEditor();
        }

        public void Remove()
        {
            if (DisplayMode == DisplayMode.New || WorkMode == WorkMode.Edit)
                Revert();
            else
            {
                _tagManager!.Remove(SourceTag);
                _storageController.AutoSave();
            }
            _unfocusEditor();
        }

        public void EnterEditMode()
        {
            if (WorkMode == WorkMode.Edit)
                return;
            WorkMode = WorkMode.Edit;
            _focusEditor();
        }

        private void _focusEditor()
            => FocusEditorInteraction.Handle().Take(1).Subscribe();
        private void _unfocusEditor()
            => UnfocusEditorInteraction.Handle().Take(1).Subscribe();

        public int CompareTo(ITagEditorChip? other)
        {
            if (other is null)
                return -1;
            if (this.DisplayMode == other.DisplayMode
                && this.DisplayMode == DisplayMode.New)
                return 0;
            if (this.DisplayMode == DisplayMode.New)
                return 1;
            if (other.DisplayMode == DisplayMode.New)
                return -1;
            return this.DisplayMode == DisplayMode.Fraction
                    && Distribution is not null
                ? this.Distribution.CompareTo(other.Distribution)
                : this.SourceTag.CompareTo(other.SourceTag);
        }
        public override string ToString()
            => $"TagChip '{SourceTag}' | {DisplayMode} | { WorkMode}";

        public int CompareTo(object obj)
            => CompareTo(obj as ITagEditorChip);
    }
}
