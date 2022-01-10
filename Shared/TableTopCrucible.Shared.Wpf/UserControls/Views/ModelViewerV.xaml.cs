using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HelixToolkit.Wpf;

using Microsoft.EntityFrameworkCore.Diagnostics;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ModelViewerV.xaml
    /// </summary>
    public partial class ModelViewerV : ReactiveUserControl<ModelViewerVm>
    {
        public ModelViewerV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                ViewModel!.BringIntoView.RegisterHandler(context =>
                    {
                        context.SetOutput(Unit.Default);
                    }),

                this.WhenAnyValue(v=>v.ViewModel.PlaceholderText.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.PlaceholderText.Text),

                this.WhenAnyValue(vm=>vm.ViewModel.IsLoading)
                    .Select(loading=>
                        loading
                            ? Visibility.Visible
                            : Visibility.Collapsed)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this,
                        v=>v.PlaceholderContainer.Visibility),

                this.WhenAnyValue(v=>v.ViewModel.ViewportContent)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.ContainerVisual.Content),
            });
        }
    }
}
