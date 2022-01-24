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
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    public class ItemSelectionInfo : ReactiveObject
    {
        public IItemThumbnailViewer ThumbnailViewer { get; }
        public ItemSelectionInfo(Infrastructure.Models.Entities.Item item)
        {
            this.ThumbnailViewer = Locator.Current.GetService<IItemThumbnailViewer>();
            ThumbnailViewer!.Item = item;
        }

        [Reactive]
        public bool IsSelected { get; set; }
    }

    [Transient]
    public interface IItemList : IDisposable
    {
        IObservableList<Infrastructure.Models.Entities.Item> SelectedItems { get; }
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
                    .Transform(itemInfo => itemInfo.ThumbnailViewer.Item)
                    .AsObservableList();
            SelectedItems.Connect().Subscribe();

            this.WhenActivated(() => new[]
            {
                itemSelectionSrc
                    .SortBy(itemInfo => itemInfo.ThumbnailViewer.Item.Name.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Items)
                    .Subscribe()
            });
        }

        public ViewModelActivator Activator { get; } = new();
        public IObservableList<Infrastructure.Models.Entities.Item> SelectedItems { get; }

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