using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for NotificastionListV.xaml
    /// </summary>
    public partial class BannerListV : ReactiveUserControl<BannerListVm>, IActivatableView
    {
        public BannerListV()
        {
            InitializeComponent();
            this.WhenActivated(()=>new []
            {
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.NotificationList,
                            v=>v.Notifications.ItemsSource),
            });
        }
    }
}
