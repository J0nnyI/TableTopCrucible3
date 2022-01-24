using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface ITagEditor
    {
        ISourceList<Tag> Tags { get; set; }
    }
    public class TagEditorVm:ReactiveObject,IActivatableViewModel, ITagEditor
    {
        public ViewModelActivator Activator { get; } = new();
        [Reactive]
        public ISourceList<Tag> Tags { get; set; }

        private ReadOnlyObservableCollection<Tag> _tagList;
        public ReadOnlyObservableCollection<Tag> TagsList => _tagList;

        public TagEditorVm()
        {
            this.WhenActivated(() =>new[]
            {
                this.WhenAnyValue(vm=>vm.Tags)
                    .Select(tags=>tags.Connect())
                    .Switch()
                    .Bind(out _tagList)
                    .Subscribe()
            });
        }
    }
}
