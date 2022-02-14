using System.Reactive.Linq;
using System.Windows;

using ReactiveUI;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for ModelHeaderV.xaml
    /// </summary>
    public partial class ItemViewerHeaderV : ReactiveUserControl<ItemViewerHeaderVm>
    {
        private static readonly GridLength GridLengthStar = new(1, GridUnitType.Star);
        private static readonly GridLength GridLengthAuto = GridLength.Auto;
        private static readonly GridLength GridLengthZero = new(0);
        public ItemViewerHeaderV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                ViewModel!.SelectionCountChanges
                    .Select(count=>count.ToString())
                    .BindTo(this, vm=>vm.ItemCount.Text),
                ViewModel!.ShowItemCountChanges
                    .Select(visible=>visible?Visibility.Visible:Visibility.Collapsed)
                    .BindTo(this, vm=>vm.ItemCountBorder.Visibility),
                this.Bind(ViewModel,
                    vm=>vm.TabStrip,
                    v=>v.TabStrip.ViewModel),

                /* name edit bindings */
                ViewModel.SelectionCountChanges
                    .Select(count=>
                        count > 1
                            ? GridLengthStar 
                            : GridLengthAuto)
                    .BindTo(this, v=>v.NameColumnDefinition.Width),

                ViewModel.SelectionCountChanges
                    .Select(count=>
                        count > 1
                            ? GridLengthZero
                            : GridLengthStar)
                    .BindTo(this, v=>v.SpacerColumnDefinition.Width),

                //editMode = true
                this.Bind(ViewModel,
                    vm=>vm.EditedName,
                    v=>v.NameEditor.Text),
                this.ViewModel
                    .EditModeChanges
                    .Select(editMode=>editMode?Visibility.Visible:Visibility.Collapsed)
                    .OutputObservable(out var editorVisible)
                    .BindTo(this,
                        vm=>vm.NameEditor.Visibility),
                editorVisible.BindTo(this,
                    vm=>vm.RevertName.Visibility),
                this.BindCommand(ViewModel,
                    vm=>vm.RevertNameCommand,
                    v=>v.RevertName),
                editorVisible.BindTo(this,
                    vm=>vm.SaveName.Visibility),
                this.BindCommand(ViewModel,
                    vm=>vm.SaveNameCommand,
                    v=>v.SaveName),

                //editMode = false
                this.Bind(ViewModel,
                    vm=>vm.EditedName,
                    v=>v.NameDisplay.Text),
                this.ViewModel
                    .EditModeChanges
                    .Select(editMode=>editMode?Visibility.Collapsed:Visibility.Visible)
                    .OutputObservable(out var viewerVisible)
                    .BindTo(this,
                        vm=>vm.NameDisplay.Visibility),
                viewerVisible.BindTo(this,
                    vm=>vm.EditName.Visibility),
                this.BindCommand(ViewModel,
                    vm=>vm.EditNameCommand,
                    v=>v.EditName),
                //editable
                ViewModel.EditableChanges
                    .Select(editable=>editable?Visibility.Visible:Visibility.Collapsed)
                    .BindTo(this, vm=>vm.EditorButtonContainer.Visibility)
            });
        }
    }
}