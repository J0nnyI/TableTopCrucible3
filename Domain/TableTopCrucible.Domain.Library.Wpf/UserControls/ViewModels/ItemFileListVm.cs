using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemFileList: IDisposable
    {
        Item Item { get; set; }
    }
    public class ItemFileListVm : ReactiveObject, IItemFileList, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new(); 
        private readonly CompositeDisposable _disposables = new();
        [Reactive]
        public Item Item { get; set; }

        public ObservableCollection<FileData> Files { get; } = new();

        public ItemFileListVm()
        {
            this.WhenActivated(()=>new []
            {
                this.WhenAnyValue(vm => vm.Item)
                    .Do(x=>{})
                    .Pairwise(false)
                    .Subscribe(pair =>
                    {
                        if (pair.Previous.HasValue)
                            pair.Previous.Value.Files.CollectionChanged -= Files_CollectionChanged;

                        if (!pair.Current.HasValue)
                            return;

                        pair.Current.Value.Files.CollectionChanged += Files_CollectionChanged;
                        Files.Clear();
                        Files.AddRange(pair.Current.Value.Files);
                    })
            });
        }

        private void Files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Files.Clear();
            Files.AddRange(Item.Files);
        }

        public void Dispose()
            => _disposables.Dispose();
    }
}
