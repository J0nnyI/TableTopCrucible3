using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Services;
using System.Linq;
using System.Reactive.Disposables;
using MoreLinq;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Repositories.Helper;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public class ItemSelectionInfo:ReactiveObject
    {
        [Reactive]
        public bool IsSelected { get; set; }
        public Item Item { get; }
        public ItemSelectionInfo(Item item)
        {
            this.Item = item;
        }
    }
    [Transient]
    public interface IItemList : IDisposable
    {
        IObservableList<Item> SelectedItems { get; }
    }
    public class ItemListVm : ReactiveObject, IItemList, IActivatableViewModel,IDisposable
    {
        public ObservableCollectionExtended<ItemSelectionInfo> Items = new();
        private readonly List<ItemSelectionInfo> _selectedItems = new();
        public IObservableList<Item> SelectedItems { get; }
        private readonly CompositeDisposable _disposables = new();

        public ItemListVm(IItemRepository itemRepository)
        {
            var itemSelectionSrc = itemRepository
                .Updates
                .ToObservableCache(_disposables)
                .Transform(item => new ItemSelectionInfo(item))
                .AsObservableCache()// required to make sure that all subscriber share the same item instance
                .Connect();

            SelectedItems =
                itemSelectionSrc
                    .RemoveKey()
                    .FilterOnObservable(itemInfo => itemInfo
                        .WhenAnyValue(x => x.IsSelected)
                        .AsObservable())
                    .Transform(itemInfo => itemInfo.Item)
                    .AsObservableList();
            SelectedItems.Connect().Subscribe();

            this.WhenActivated(() => new[]{
                itemSelectionSrc
                    .SortBy(itemInfo=>itemInfo.Item.Name.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Items)
                    .Subscribe(),
            });
        }
        
        public ViewModelActivator Activator { get; } = new();
        public void Dispose()
            => _disposables.Dispose();

        private ItemSelectionInfo previouslyClickedItem = null;
        public void OnItemClicked(ItemSelectionInfo itemInfo, MouseButtonEventArgs e)
        {
            var curItem = itemInfo;
            var prevItem = previouslyClickedItem;
            var isStrgPressed = KeyboardHelper.IsKeyPressed(ModifierKeys.Control);
            var isShiftPressed = KeyboardHelper.IsKeyPressed(ModifierKeys.Shift);
            var isAltPressed = KeyboardHelper.IsKeyPressed(ModifierKeys.Alt);

            if (isAltPressed)
                return;
            if (isStrgPressed)
                _toggleSelection(curItem);
            else if (isShiftPressed)
            {
                var section =
                    this.Items
                        .Subsection(curItem, prevItem)
                        .ToArray();

                if (section.Any(item => !item.IsSelected))
                    _selectItem(section);
                else
                    _deselectItems(section);
            }
            else
            {
                this._deselectItems(this._selectedItems.Where(x=>x != curItem).ToArray());
                this._selectItem(curItem);
            }

            e.Handled = true;
            previouslyClickedItem = curItem;

        }

        private void _toggleSelection(ItemSelectionInfo item)
        {
            if(item.IsSelected)
                _deselectItems(item);
            else
                _selectItem(item);
        }
        private void _selectItem(params ItemSelectionInfo[] items)
        {
            _selectedItems.AddRange(items.Where(item=>!item.IsSelected));
            items.ToList().ForEach(item=>item.IsSelected = true);
        }

        private void _deselectItems(params ItemSelectionInfo[] items)
        {
            _selectedItems.RemoveMany(items.Where(item => item.IsSelected));
            items.ToList().ForEach(item => item.IsSelected = false);
        }
    }
}