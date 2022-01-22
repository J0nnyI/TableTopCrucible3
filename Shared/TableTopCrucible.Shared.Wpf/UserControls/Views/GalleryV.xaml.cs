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
    /// Interaction logic for GalleryV.xaml
    /// </summary>
    public partial class GalleryV : ReactiveUserControl<GalleryVm>
    {
        public GalleryV()
        {
            InitializeComponent();
            this.WhenActivated(() => new IDisposable[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Images,
                    v => v.Images.ItemsSource),
                this.OneWayBind(ViewModel,
                    vm=>vm.SelectedImageViewer,
                    v=>v.SelectedImage.ViewModel),
                this.WhenAnyValue(v=>v.Images.SelectedItem)
                    .Cast<GalleryItem>()
                    .Select(galleryItem=>galleryItem?.Image)
                    .BindTo(this,v=>v.ViewModel.SelectedImage)
            });
        }
    }
}
