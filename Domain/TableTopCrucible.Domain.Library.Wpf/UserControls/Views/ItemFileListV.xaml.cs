using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ItemFileListVm.xaml
    /// </summary>
    public partial class ItemFileListV : ReactiveUserControl<ItemFileListVm>
    {
        public ItemFileListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Files,
                    v => v.Files.ItemsSource),
                this.WhenAnyValue(v=>v.ViewModel).BindTo(this, v=>v.DataContext),
            });
        }
    }
}
