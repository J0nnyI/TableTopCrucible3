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
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ModelHeaderV.xaml
    /// </summary>
    public partial class ItemViewerHeaderV : ReactiveUserControl<ItemViewerHeaderVm>
    {
        public ItemViewerHeaderV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel.Item.Name)
                    .Select(name=>name.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.Name.Text)
            });
        }
    }
}
