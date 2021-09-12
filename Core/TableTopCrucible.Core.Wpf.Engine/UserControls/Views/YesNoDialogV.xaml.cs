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

using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class YesNoDialogV : ReactiveUserControl<YesNoDialogVm>, IActivatableView
    {
        public YesNoDialogV()
        {
            InitializeComponent();
            this.WhenActivated(() =>new []
            {
                this.Bind(ViewModel,
                    vm => vm.Text,
                    v => v.Text.Text),
                this.Bind(ViewModel,
                    vm => vm.YesClickedCommand,
                    v => v.YesButton.Command),
                this.Bind(ViewModel,
                    vm => vm.NoClickedCommand,
                    v =>   v.NoButton.Command),

            });
        }
    }
}
