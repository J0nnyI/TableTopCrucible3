using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.Views
{
    /// <summary>
    /// Interaction logic for SettingsPv.xaml
    /// </summary>
    public partial class SettingsPv : ReactiveUserControl<SettingsPvm>, IActivatableView
    {
        public SettingsPv()
        {
            InitializeComponent();
            this.WhenActivated(()=>new[]
            {
                this.WhenAnyValue(v=>v.SettingsList.SelectedItem)
                    .Pairwise()
                    .Subscribe(pair =>
                    {
                        // disable deselection
                        if (pair.Current == null && pair.Previous != null)
                            SettingsList.SelectedItem = pair.Previous;
                    }),
                this.OneWayBind(
                    ViewModel, 
                    vm => vm.Pages,
                    v => v.SettingsList.ItemsSource),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.CurrentPage,
                    v=>v.PageContainer.ViewModel),
                this.WhenAnyValue(v=>v.SettingsList.SelectedItem)
                    .BindTo(ViewModel, vm=>vm.CurrentPage),
            });
        }
    }
}
