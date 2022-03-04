using System.Reactive.Linq;
using System.Windows;

using ReactiveUI;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views;

/// <summary>
///     Interaction logic for ModelHeaderV.xaml
/// </summary>
public partial class ItemViewerHeaderV : ReactiveUserControl<ItemViewerHeaderVm>
{
    private static double nameSpacingBuffer = 4;

    public ItemViewerHeaderV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.WhenAnyValue(
                v=>v.SensorLeft.ActualWidth,
                v=>v.SensorRight.ActualWidth,
                v=>v.LeftGrid.ActualWidth,
                (left, right, total)=>
                    (total-left-right-nameSpacingBuffer) .Min(5))
                .BindTo(this, v=>v.NameColumn.MaxWidth),
            this.ViewModel.TitleTooltipChanges
                .Do(x=>{})
                .BindTo(this, v=>v.NameDisplay.ToolTip),

            ViewModel!.SelectionCountChanges
                .Select(count => count.ToString())
                .BindTo(this, vm => vm.ItemCount.Text),
            ViewModel!.ShowItemCountChanges
                .Select(visible => visible
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .BindTo(this, vm => vm.ItemCountBorder.Visibility),
            this.Bind(ViewModel,
                vm => vm.TabStrip,
                v => v.TabStrip.ViewModel),

            //editMode = true
            this.Bind(ViewModel,
                vm => vm.EditedName,
                v => v.NameEditor.Text),
            this.Bind(ViewModel,
                vm => vm.EditedName,
                v => v.NameEditor.ToolTip),
            this.ViewModel
                .EditModeChanges
                .Select(editMode => editMode
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .OutputObservable(out var editorVisible)
                .BindTo(this,
                    vm => vm.NameEditor.Visibility),
            editorVisible.BindTo(this,
                vm => vm.RevertName.Visibility),
            this.BindCommand(ViewModel,
                vm => vm.RevertNameCommand,
                v => v.RevertName),
            editorVisible.BindTo(this,
                vm => vm.SaveName.Visibility),
            this.BindCommand(ViewModel,
                vm => vm.SaveNameCommand,
                v => v.SaveName),

            //editMode = false
            this.Bind(ViewModel,
                vm => vm.EditedName,
                v => v.NameDisplay.Text),
            this.ViewModel
                .EditModeChanges
                .Select(editMode => editMode
                    ? Visibility.Collapsed
                    : Visibility.Visible)
                .OutputObservable(out var viewerVisible)
                .BindTo(this,
                    vm => vm.NameDisplay.Visibility),
            viewerVisible.BindTo(this,
                vm => vm.EditName.Visibility),
            this.BindCommand(ViewModel,
                vm => vm.EditNameCommand,
                v => v.EditName),
            //editable
            ViewModel.EditableChanges
                .Select(editable => editable
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .BindTo(this, vm => vm.EditorButtonContainer.Visibility)
        });
    }
}