using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using HelixToolkit.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

/// <summary>
///     Interaction logic for ModelViewerV.xaml
/// </summary>
public partial class ModelViewerV : ReactiveUserControl<ModelViewerVm>
{
    public ModelViewerV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            // interactions
            ViewModel!.BringIntoView.RegisterHandler(context =>
            {
                var bounds = ViewModel.ViewportContent.Bounds;
                Viewport.Camera.ZoomExtents(Viewport.Viewport, bounds);
                context.SetOutput(Unit.Default);
            }),

            // bindings
            this.WhenAnyValue(v => v.ViewModel.PlaceholderText)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.LoadingScreen.Text),

            this.WhenAnyValue(vm => vm.ViewModel.IsLoading)
                .Select(loading =>
                    loading
                        ? Visibility.Visible
                        : Visibility.Collapsed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this,
                    v => v.LoadingScreen.Visibility),

            this.WhenAnyValue(v => v.ViewModel.ViewportContent)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.ContainerVisual.Content),
            new ActOnLifecycle(
                () => ViewModel.Viewport = Viewport,
                () =>
                {
                    ViewModel.Viewport = null;
                    ContainerVisual.Content = null;
                })
        });
    }
}