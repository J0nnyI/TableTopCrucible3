using System;
using System.Windows;
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
        public NavigationListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel).BindTo(this, v=>v.DataContext),
                this.OneWayBind(ViewModel,
                    vm=>vm.UpperList,
                    v=>v.UpperList.ItemsSource),
                this.OneWayBind(ViewModel,
                    vm=>vm.LowerList,
                    v=>v.LowerList.ItemsSource),
                this.WhenAnyValue(
                        v=>v.UpperList.SelectedItem
                        )
                    .BindTo(this, v=>v.ViewModel.SelectedItem),
                this.WhenAnyValue(
                        v=>v.LowerList.SelectedItem
                    )
                    .BindTo(this, v=>v.ViewModel.SelectedItem),

                this.ToggleMenuItem
                    .Events()
                    .MouseUp
                    .Subscribe(_=>ViewModel!.ToggleExpansionCommand.Execute(null)),
            });
        }
    }
}
