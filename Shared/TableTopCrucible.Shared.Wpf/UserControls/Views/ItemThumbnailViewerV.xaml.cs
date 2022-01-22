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
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ItemThumbnailViewerV.xaml
    /// </summary>
    public partial class ItemThumbnailViewerV : ReactiveUserControl<ItemThumbnailViewerVm>
    {
        public ItemThumbnailViewerV()
        {
            InitializeComponent();

            this.WhenActivated(() => new IDisposable[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.ImageViewer,
                    v => v.ImageViewer.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.Name,
                    v=>v.Name.Text)
            });
        }
    }
}
