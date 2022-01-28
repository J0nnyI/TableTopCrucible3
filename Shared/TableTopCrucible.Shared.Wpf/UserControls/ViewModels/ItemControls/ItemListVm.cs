using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

using DynamicData;
using DynamicData.Binding;

using MoreLinq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    public class ItemSelectionInfo : ReactiveObject
    {
        public Item Item => ThumbnailViewer.Item;
        public IItemThumbnailViewer ThumbnailViewer { get; }
        public ItemSelectionInfo(Item item)
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
        IObservableList<Item> SelectedItems { get; }
        Func<Item, bool> Filter { get; set; }
    }

    public class ItemListVm : ReactiveObject, IItemList, IActivatableViewModel, IDisposable
    {
        private readonly IFileRepository _fileRepository;
        private readonly CompositeDisposable _disposables = new();
        public ObservableCollectionExtended<ItemSelectionInfo> Items = new();

        private ItemSelectionInfo previouslyClickedItem;

        public ItemListVm(IItemRepository itemRepository, IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _selectedItemInfo = itemRepository
                .Data
                .Connect()
                .Transform(item => new ItemSelectionInfo(item))
                .RemoveKey()
                .AsObservableList(); // required to make sure that all subscriber share the same item instance

            SourceList<ItemSelectionInfo> selectedItems = new();

            void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != nameof(ItemSelectionInfo.IsSelected) || sender is not ItemSelectionInfo itemInfo)
                    return;

                if (itemInfo.IsSelected)
                    selectedItems.Add(itemInfo);
                else
                    selectedItems.Remove(itemInfo);
            }

            SelectedItems = selectedItems.Connect()
                .Transform(itemInfo => itemInfo.Item)
                .AsObservableList();
            this.WhenActivated(() => new[]
            {
                _selectedItemInfo
                    .Connect()
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Filter(this.WhenAnyValue(vm=>vm.Filter)
                        .Select<Func<Item,bool>,Func<ItemSelectionInfo, bool>>(
                            filter=>
                                info=>
                                    filter(info.Item))
                        .Throttle(SettingsHelper.FilterThrottleSpan)
                        .ObserveOn(RxApp.TaskpoolScheduler))
                    .Sort(itemInfo => itemInfo.ThumbnailViewer.Item.Name.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .OnItemAdded(item=>item.PropertyChanged +=ItemOnPropertyChanged)
                    .OnItemRemoved(item=>item.PropertyChanged -=ItemOnPropertyChanged)
                    .Bind(Items)
                    .Subscribe(),

                this.WhenAnyValue(vm=>vm.Filter)
                    .Subscribe(filter =>
                    {
                        selectedItems.Items
                            .Where(info => !filter(info.Item))
                            .ToArray()
                            .ForEach(item=>item.IsSelected = false);
                    }),

            });
        }
        public ViewModelActivator Activator { get; } = new();
        private IObservableList<ItemSelectionInfo> _selectedItemInfo { get; }
        public IObservableList<Item> SelectedItems { get; }

        [Reactive]
        public Func<Item, bool> Filter { get; set; } = _ => true;

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
                _toggleSelection(curItem);
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
                var otherItems = _selectedItemInfo.Items.Where(itemInfo => itemInfo != curItem).ToArray();
                otherItems.ForEach(item => item.IsSelected = false);
                curItem.IsSelected = true;
            }

            e.Handled = true;
            previouslyClickedItem = curItem;
        }

        private void _toggleSelection(ItemSelectionInfo item)
        {
            item.IsSelected = !item.IsSelected;
        }

        private void _selectItem(params ItemSelectionInfo[] items)
        {
            items.ToList().ForEach(item => item.IsSelected = true);
        }

        private void _deselectItems(params ItemSelectionInfo[] items)
        {
            items.ToList().ForEach(item => item.IsSelected = false);
        }

        public void InitiateDrag(DependencyObject source)
        {
            var files = this.SelectedItems.Items
                .Select(item=>item.FileKey3d)
                .Select(_fileRepository.SingleByHashKey)
                .Select(file=>file?.Path?.Value)
                .Where(x => x != null)
                .ToStringCollection();

            DataObject dragData = new DataObject();
            dragData.SetFileDropList(files);
            DragDrop.DoDragDrop(source, dragData, DragDropEffects.Move);
        }
    }
}