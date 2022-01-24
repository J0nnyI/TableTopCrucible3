using System;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public class TagController : ReactiveObject
    {
        private readonly Action<Tag, string> _editTag;

        public TagController(Tag sourceTag, Action<Tag, string> editTag, bool isNew = false)
        {
            _editTag = editTag;
            SourceTag = sourceTag;
            IsNew = isNew;
            EditTag = sourceTag?.Value ?? string.Empty;

        }
        public Tag SourceTag { get; }
        [Reactive]
        public string EditTag { get; set; }

        [Reactive]
        public bool EditModeEnabled { get; set; }
        [Reactive]
        public bool IsNew { get; set; }

        public bool WasNew => SourceTag is null;

        public void Revert()
        {
            EditTag = SourceTag?.Value ?? string.Empty;
            EditModeEnabled = false;
            IsNew = WasNew;
        }

        public void Confirm()
        {
            _editTag(SourceTag, EditTag);
            EditTag = SourceTag?.Value ?? string.Empty;
            EditModeEnabled = false;
            IsNew = WasNew;
        }
    }

    [Transient]
    public interface ITagEditor
    {
        ISourceList<Tag> TagSource { get; set; }
    }
    public class TagEditorVm : ReactiveObject, IActivatableViewModel, ITagEditor
    {
        public ViewModelActivator Activator { get; } = new();
        [Reactive]
        public ISourceList<Tag> TagSource { get; set; }

        [Reactive]
        public bool TagDeletionEnabled { get; set; } = true;

        public ReactiveCommand<TagController, Unit> RemoveTagCommand { get; }

        public ObservableCollectionExtended<TagController> TagList { get; } = new();

        public TagEditorVm()
        {
            this.RemoveTagCommand = ReactiveCommand.Create<TagController>(RemoveTag);
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm=>vm.TagSource)
                    .Select(tags=>tags?.Connect() ?? Observable.Never(ChangeSet<Tag>.Empty))
                    .Switch()
                    .Transform(tag=>new TagController(tag, EditTag))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(TagList)
                    .Subscribe(_ =>
                    {
                        TagList.RemoveWhere(tag=>tag.WasNew);
                        TagList.Add(new TagController(null, EditTag, true));
                    }),
                new ActOnLifecycle(null,()=>TagList.Clear())
            });
        }

        public void RemoveTag(TagController tagController)
        {
            if(tagController.WasNew || tagController.EditModeEnabled)
                tagController.Revert();
            else
                TagSource!.Remove(tagController.SourceTag);

        }
        public void EditTag(Tag oldTag, string newTag)
        {
            if (oldTag is null)
            {
                TagSource.Add((Tag)newTag);
            }
            else
                TagSource!.Replace(oldTag, (Tag)newTag);
        }
    }
}