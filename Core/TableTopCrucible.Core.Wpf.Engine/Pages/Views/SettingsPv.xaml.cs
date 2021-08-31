using System;
using System.Reactive.Disposables;
using ReactiveUI;
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
            this.WhenActivated(()=>new IDisposable[]
            {
                this.OneWayBind(
                    ViewModel, 
                    vm => vm.Pages,
                    v => v.SettingsList.ItemsSource),
            });
        }
    }
}
