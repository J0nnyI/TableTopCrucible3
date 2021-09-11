using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for NotificationListV.xaml
    /// </summary>
    public partial class NavigationListV : ReactiveUserControl<NavigationListVm>, IActivatableView
    {
        [Reactive]
        public INavigationPage Selection { get; set; }

        public NavigationListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),

                this.OneWayBind(ViewModel,
                    vm=>vm.UpperList,
                    v=>v.UpperList.ItemsSource),

                this.OneWayBind(ViewModel,
                    vm=>vm.LowerList,
                    v=>v.LowerList.ItemsSource),

                this.Bind(ViewModel,
                    vm=>vm.UpperSelection,
                    v=>v.UpperList.SelectedItem,
                    m=>m.HasContent?m:null,
                    m =>
                         m as FlaggedNavigationItem ?? new FlaggedNavigationItem(NavigationPageLocation.Upper, true)),
                this.Bind(ViewModel,
                    vm=>vm.LowerSelection,
                    v=>v.LowerList.SelectedItem,
                    m=>m.HasContent?m:null,
                    m=>m as FlaggedNavigationItem ?? new FlaggedNavigationItem(NavigationPageLocation.Lower, true)),
                
            });
        }

        private void BlockOnControl(object sender, MouseButtonEventArgs e)
            => e.Handled = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }
}
