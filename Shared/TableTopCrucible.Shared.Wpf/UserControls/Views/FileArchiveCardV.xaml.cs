using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    public partial class FileArchiveCardV : ReactiveUserControl<FileArchiveCardVm>, IActivatableView
    {
        public FileArchiveCardV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),
                this.Bind(
                    ViewModel,
                    vm=>vm.SaveChangesCommand,
                    v=>v.SaveChanges.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.UndoChangesCommand,
                    v=>v.UndoChanges.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.RemoveDirectoryCommand,
                    v=>v.RemoveDirectory.Command),
            });
        }
    }
}
