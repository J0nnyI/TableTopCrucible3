using ReactiveUI;

using System.Windows;

using TableTopCrucible.Core.WPF.ViewModels;

namespace TableTopCrucible.Core.WPF.Views
{
    /// <summary>
    /// Interaction logic for EditSelectorV.xaml
    /// </summary>
    public partial class EditSelectorV : ReactiveUserControl<EditSelectorVM>
    {
        public EditSelectorV()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.EditModeEnabled, v => v.EnterEdit.Visibility,
                    vmp => !vmp ? Visibility.Visible : Visibility.Collapsed);
                this.OneWayBind(ViewModel, vm => vm.EditModeEnabled, v => v.RevertChanges.Visibility,
                    vmp => vmp ? Visibility.Visible : Visibility.Collapsed);
                this.OneWayBind(ViewModel, vm => vm.EditModeEnabled, v => v.SaveChanges.Visibility,
                    vmp => vmp ? Visibility.Visible : Visibility.Hidden);
                this.OneWayBind(ViewModel, vm => vm.EditModeEnabled, v => v.Remove.Visibility,
                    vmp => vmp ? Visibility.Visible : Visibility.Hidden);

                this.BindCommand(ViewModel, vm => vm.EnterEditMode, v => v.EnterEdit);
                this.BindCommand(ViewModel, vm => vm.RevertChanges, v => v.RevertChanges);
                this.BindCommand(ViewModel, vm => vm.SaveChanges, v => v.SaveChanges);
                this.BindCommand(ViewModel, vm => vm.Remove, v => v.Remove);
            });
        }
    }
}
