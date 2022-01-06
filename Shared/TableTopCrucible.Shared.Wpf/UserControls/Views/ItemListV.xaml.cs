using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ReactiveUI;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for ItemListV.xaml
    /// </summary>
    public partial class ItemListV : ReactiveUserControl<ItemListVm>, IActivatableView
    {
        public ItemListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new []
            {
                this.OneWayBind(ViewModel,
                    vm => vm.FileSyncCommand,
                    v => v.FileSync.Command),
                this.OneWayBind(ViewModel,
                    vm => vm.Items,
                    v => v.Items.ItemsSource),
                this.Items.Events().SelectionChanged
                    .Subscribe(args =>
                        ViewModel.UpdateSelection(
                            args.AddedItems.Cast<Item>(),
                            args.RemovedItems.Cast<Item>())
                    )
            });
        }
    }
}