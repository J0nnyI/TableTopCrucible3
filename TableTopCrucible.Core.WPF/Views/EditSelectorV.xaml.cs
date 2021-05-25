using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                    vmp => vmp ? Visibility.Visible : Visibility.Collapsed);

                this.BindCommand(ViewModel, vm => vm.EnterEditMode, v => v.EnterEdit);
                this.BindCommand(ViewModel, vm => vm.RevertChanges, v => v.RevertChanges);
                this.BindCommand(ViewModel, vm => vm.SaveChanges, v => v.SaveChanges);
            });
        }
    }
}
