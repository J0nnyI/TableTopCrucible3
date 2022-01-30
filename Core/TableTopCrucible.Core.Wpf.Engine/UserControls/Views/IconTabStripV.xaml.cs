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
    /// Interaction logic for IconTabStripV.xaml
    /// </summary>
    public partial class IconTabStripV
    {
        public IconTabStripV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.Tabs,
                    v=>v.TabItems.ItemsSource)
            });
        }
    }
}
