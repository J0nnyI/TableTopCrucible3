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
using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.Views
{
    /// <summary>
    /// Interaction logic for JobQueuePageV.xaml
    /// </summary>
    public partial class JobQueuePageV : ReactiveUserControl<JobQueuePagePageVm>
    {
        public JobQueuePageV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.AllQueue,
                    v=>v.AllHost.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.TodoQueue,
                    v=>v.ToDoHost.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.InProgressQueue,
                    v=>v.InProgressHost.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.DoneQueue,
                    v=>v.DoneHost.ViewModel),
            });
        }
    }
}
