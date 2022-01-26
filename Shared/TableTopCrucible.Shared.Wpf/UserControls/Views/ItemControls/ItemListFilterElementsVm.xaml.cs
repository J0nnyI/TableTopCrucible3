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
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    /// <summary>
    /// Interaction logic for ItemListFilterElementsVm.xaml
    /// </summary>
    public partial class ItemListFilterElementsVm : ReactiveUserControl<ItemListFilterElementsV>
    {
        public ItemListFilterElementsVm()
        {
            InitializeComponent();
            this.WhenActivated(() => new IDisposable[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.FilterMode,
                    v=>v.Header.Text,
                    mode=>mode == FilterMode.Include
                        ?"Include"
                        :"Exclude"),
                this.Bind(ViewModel,
                    vm=>vm.NameFilter,
                    v=>v.Name.Text),
                this.Bind(ViewModel,
                    vm=>vm.TagEditor,
                    v=>v.TagEditor.ViewModel),
            });
        }
    }
}
