using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

using ReactiveUI;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;
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
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(
                    ViewModel,
                    vm => vm.Pages,
                    v => v.SettingsList.ItemsSource),

                this.OneWayBind(
                    ViewModel,
                    vm=>vm.CurrentPage,
                    v=>v.PageContainer.ViewModel),

                this.Bind(ViewModel,
                    vm=>vm.CurrentPage,
                    v=>v.SettingsList.SelectedItem),

                // undo deselection
                this.WhenAnyValue(v=>v.SettingsList.SelectedItem)
                    .Cast<ISettingsCategoryPage>()
                    .Pairwise()
                    .Where(pair=>!pair.Current.HasValue && pair.Previous.HasValue)
                    .Select(pair=>pair.Previous.Value)
                    .BindTo(ViewModel, v=>v.CurrentPage)
            });
        }
    }
}
