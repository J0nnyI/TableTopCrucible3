using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public class ItemSelectionInfo : ReactiveObject
    {
        public ItemSelectionInfo(Item item)
        {
            Item = item;
        }

        [Reactive]
        public bool IsSelected { get; set; }

        public Item Item { get; }
    }

    [Transient]
    public interface IItemList : IDisposable
    {
        IObservableList<Item> SelectedItems { get; }
    }

    public class ItemListVm : ReactiveObject, IItemList, IActivatableViewModel, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly List<ItemSelectionInfo> _selectedItems = new();
        public ObservableCollectionExtended<ItemSelectionInfo> Items = new();

        private ItemSelectionInfo previouslyClickedItem;

        public ItemListVm(IItemRepository itemRepository)
        {
            var itemSelectionSrc = itemRepository
                .Data
                .Connect()
                .Transform(item => new ItemSelectionInfo(item))
                .AsObservableCache() // required to make sure that all subscriber share the same item instance
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

            this.WhenActivated(() => new[]
            {
                itemSelectionSrc
                    .SortBy(itemInfo => itemInfo.Item.Name.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Items)
                    .Subscribe()
            });
        }

        public ViewModelActivator Activator { get; } = new();
        public IObservableList<Item> SelectedItems { get; }

        public void Dispose()
            => _disposables.Dispose();

        public void OnItemClicked(ItemSelectionInfo itemInfo, MouseButtonEventArgs e)
        {
            var curItem = itemInfo;
            var prevItem = previouslyClickedItem;
            var isCtrlPressed = KeyboardHelper.IsKeyPressed(ModifierKeys.Control);
            var isShiftPressed = KeyboardHelper.IsKeyPressed(ModifierKeys.Shift);
            var isAltPressed = KeyboardHelper.IsKeyPressed(ModifierKeys.Alt);

            if (isAltPressed)
                return;
            if (isCtrlPressed)
            {
                _toggleSelection(curItem);
            }
            else if (isShiftPressed)
            {
                var section =
                    Items
                        .Subsection(curItem, prevItem)
                        .ToArray();

                if (section.Any(item => !item.IsSelected))
                    _selectItem(section);
                else
                    _deselectItems(section);
            }
            else
            {
                _deselectItems(_selectedItems.Where(x => x != curItem).ToArray());
                _selectItem(curItem);
            }

            e.Handled = true;
            previouslyClickedItem = curItem;
        }

        private void _toggleSelection(ItemSelectionInfo item)
        {
            if (item.IsSelected)
                _deselectItems(item);
            else
                _selectItem(item);
        }

        private void _selectItem(params ItemSelectionInfo[] items)
        {
            _selectedItems.AddRange(items.Where(item => !item.IsSelected));
            items.ToList().ForEach(item => item.IsSelected = true);
        }

        private void _deselectItems(params ItemSelectionInfo[] items)
        {
            _selectedItems.RemoveMany(items.Where(item => item.IsSelected));
            items.ToList().ForEach(item => item.IsSelected = false);
        }
    }
}