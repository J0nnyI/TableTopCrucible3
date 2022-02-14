using System.Linq;
using System.Windows;
using ReactiveUI;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views;

/// <summary>
/// Interaction logic for SingleItemViewerV.xaml
/// </summary>
public partial class SingleItemViewerV
{
    public SingleItemViewerV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.Bind(ViewModel,
                vm => vm.ModelViewer,
                v => v.ModelViewer.ViewModel),
            this.Bind(ViewModel,
                vm => vm.DataViewer,
                v => v.DataViewer.ViewModel),
            this.Bind(ViewModel,
                vm => vm.ViewerHeader,
                v => v.ViewerHeader.ViewModel),
            this.Bind(ViewModel,
                vm => vm.FileList,
                v => v.FileList.ViewModel),
            this.Bind(ViewModel,
                vm => vm.ItemGallery,
                v => v.Gallery.ViewModel),
        });
    }

    private void UIElement_OnDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent("FileDrop"))
            return;

        var paths =
            (e.Data.GetData("FileDrop") as string[])
            .Select(FilePath.From).ToArray()
            .ToArray();
        ViewModel.HandleFileDrop(paths);
    }
}