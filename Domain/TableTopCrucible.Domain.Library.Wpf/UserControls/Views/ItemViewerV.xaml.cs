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

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ItemViewerV.xaml
    /// </summary>
    public partial class ItemViewerV 
    {
        public ItemViewerV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.Header,
                    v=>v.Header.ViewModel),
                this.Bind(ViewModel,
                    vm=>vm.TabContainer,
                    v=>v.TabContainer.ViewModel)
            });
        }
    }
}
