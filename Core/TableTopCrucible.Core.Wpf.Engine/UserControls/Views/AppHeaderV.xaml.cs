using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for AppHeaderV.xaml
    /// </summary>
    public partial class AppHeaderV : ReactiveUserControl<AppHeaderVm>, IActivatableView
    {
        public AppHeaderV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),
                this.ViewModel.NotificationCountChanges
                    .Select(count => count<=0?string.Empty:count.ToString())
                    .BindTo(this, v=>v.NotificationBadge.Badge),
                this.OneWayBind(ViewModel,
                    vm=>vm.CurrentPageTitle,
                    v=>v.CurrentPageTitle.Text),
                this.Bind(ViewModel,
                    vm=>vm.IsNavigationbarExpanded,
                    v=>v.IsNavigationBarExpanded.IsChecked),

                this.OneWayBind(ViewModel,
                    vm => vm.IsNavigationbarExpanded,
                    v => v.IsNavigationBarExpanded.ToolTip,
                    (bool isExpanded) => isExpanded ? "Collapse Sidebar" : "Expand Sidebar"
                ),
            });
        }
    }
}
