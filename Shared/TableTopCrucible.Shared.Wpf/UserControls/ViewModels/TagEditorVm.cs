using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using Microsoft.EntityFrameworkCore.Diagnostics;

using MoreLinq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Views.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public class TagEditorTagController : ReactiveValidationObject, IDisposable
    {
        private readonly Action<Tag, string> _editTag;
        private readonly CompositeDisposable _disposables = new();
        public void Dispose() => _disposables.Dispose();
        public ObservableCollectionExtended<Tag> AvailableTags { get; } = new();

        public TagEditorTagController(
            Tag sourceTag,
            Action<Tag, string> editTag,
            IObservableList<Tag> takenTags,
            IObservableList<Tag> availableTags,
            bool addMode = false,
            bool editModeEnabled = false)
        {

            _editTag = editTag;
            SourceTag = sourceTag;
            AddMode = addMode;
            EditModeEnabled = editModeEnabled;
            EditTag = sourceTag?.Value ?? string.Empty;

            this.ValidationRule(
                vm => vm.EditTag,
                tag => !string.IsNullOrWhiteSpace(tag),
                "The tag must not be empty")
                .DisposeWith(_disposables);

            this.ValidationRule(
                vm => vm.EditTag,
         this.WhenAnyValue(vm => vm.EditTag)
             .CombineLatest(
                takenTags?.Connect().ToCollection() ?? Observable.Empty<IReadOnlyCollection<Tag>>(),
                (tag, tags) =>
                {
                    return tags.Count(t => t.Value == tag) < (
                        WasNew
                        ? 1
                        : 2);
                }).StartWith(true),
                "The tag has already been added")
                .DisposeWith(_disposables);

            this.WhenAnyValue(vm => vm.EditModeEnabled)
                .Select(editMode => editMode
                    ? availableTags.Connect()
                    : Observable.Empty<IChangeSet<Tag>>())
                .Switch()
                .Filter(this.WhenAnyValue(vm => vm.EditTag).Select(filter =>
                    new Func<Tag, bool>(tag => tag.Value.ToLower().Contains(filter.ToLower()))))
                .Top(SettingsHelper.MaxTagCountInDropDown)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(AvailableTags)
                .Subscribe()
                .DisposeWith(_disposables);
        }
        public Tag SourceTag { get; }
        [Reactive]
        public string EditTag { get; set; }

        [Reactive]
        public bool EditModeEnabled { get; set; }
        [Reactive]
        public bool AddMode { get; set; }

        public bool WasNew => SourceTag is null;

        public void Revert()
        {
            EditTag = SourceTag?.Value ?? string.Empty;
            EditModeEnabled = false;
            AddMode = WasNew;
        }

        public bool Confirm()
        {
            if (HasErrors)
                return false;
            _editTag(SourceTag, EditTag);

            EditTag = SourceTag?.Value ?? string.Empty;
            EditModeEnabled = false;
            AddMode = WasNew;
            return true;
        }
    }

    [Transient]
    public interface ITagEditor
    {
        ISourceList<Tag> TagSource { get; set; }
        public bool FluentModeEnabled { get; set; }
    }
    public class TagEditorVm : ReactiveObject, IActivatableViewModel, ITagEditor
    {
        private readonly IStorageController _storageController;
        public ViewModelActivator Activator { get; } = new();
        [Reactive]
        public ISourceList<Tag> TagSource { get; set; }

        [Reactive]
        public bool FluentModeEnabled { get; set; } = true;

        [Reactive]
        public bool TagDeletionEnabled { get; set; } = true;

        [Reactive]//if true, the next tag will be opened in edit mode
        public bool FluentMode { get; set; } = false;

        public ReactiveCommand<TagEditorTagController, Unit> RemoveTagCommand { get; }

        public ObservableCollectionExtended<TagEditorTagController> TagList { get; } = new();

        public TagEditorVm(IStorageController storageController, ITagView tagView)
        {
            _storageController = storageController;
            this.RemoveTagCommand = ReactiveCommand.Create<TagEditorTagController>(RemoveTag);

            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm=>vm.TagSource)
                    .Select(tags=>tags?.Connect()?? Observable.Never(ChangeSet<Tag>.Empty))
                    .Switch()
                    .Transform(tag=>new TagEditorTagController(tag, EditTag,TagSource,tagView.Data))
                    .DisposeMany()
                    .StartWithEmpty()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(TagList)
                    .Subscribe(_ =>
                    {
                        TagList.RemoveWhere(tag=>tag.WasNew)
                            .ForEach(tag=>tag.Dispose());
                        TagList.Add(new TagEditorTagController(null, EditTag,TagSource,tagView.Data, !FluentMode,FluentMode));
                        FluentMode = false;
                    }),

                new ActOnLifecycle(null,()=>TagList.Clear())
            });
        }

        public void RemoveTag(TagEditorTagController tagEditorTagController)
        {
            if (tagEditorTagController.WasNew || tagEditorTagController.EditModeEnabled)
                tagEditorTagController.Revert();
            else
            {
                TagSource!.Remove(tagEditorTagController.SourceTag);
                _storageController.AutoSave();
            }
        }
        public void EditTag(Tag oldTag, string newTag)
        {
            if (oldTag is null)
            {
                FluentMode = FluentModeEnabled;
                TagSource.Add((Tag)newTag);
            }
            else
                TagSource!.Replace(oldTag, (Tag)newTag);
            _storageController.AutoSave();
        }
    }
}