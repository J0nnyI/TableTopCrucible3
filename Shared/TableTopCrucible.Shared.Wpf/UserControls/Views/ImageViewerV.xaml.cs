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
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ImageViewerV.xaml
    /// </summary>
    public partial class ImageViewerV : ReactiveUserControl<ImageViewerVm>
    {
        public ImageViewerV()
        {
            InitializeComponent();

            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.ImageSource,
                    v=>v.Image.Source),

                this.WhenAnyValue(v=>v.ViewModel.ImageSource)
                    .Select(img=>img is null
                        ?Visibility.Visible
                        :Visibility.Collapsed)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, vm=>vm.Placeholder.Visibility),

                this.WhenAnyValue(v=>v.Width)
                    .BindTo(this, v=>v.Placeholder.Width),
                this.WhenAnyValue(v=>v.Height)
                    .BindTo(this, v=>v.Placeholder.Height),

                this.WhenAnyValue(v=>v.ViewModel.ImageSource)
                    .Select(img=>img is not null
                        ?Visibility.Visible
                        :Visibility.Collapsed)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, vm=>vm.Image.Visibility)

            });
        }
    }
}
