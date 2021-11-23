using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
                    vm=>vm.TodoQueue,
                    v=>v.ToDoHost.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.InProgressQueue,
                    v=>v.InProgressHost.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.DoneQueue,
                    v=>v.DoneHost.ViewModel),

                this.WhenAnyObservable(v=>v.ViewModel.TodoQueue.JobCountChanges)
                    .Select(count=>count.Value==0?null:count)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.ToDoBadge.Badge),
                this.WhenAnyObservable(v=>v.ViewModel.InProgressQueue.JobCountChanges)
                    .Select(count=>count.Value==0?null:count)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.InProgressBadge.Badge),
                this.WhenAnyObservable(v=>v.ViewModel.DoneQueue.JobCountChanges)
                    .Select(count=>count.Value==0?null:count)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.DoneBadge.Badge),

                this.Bind(ViewModel, 
                    vm=>vm.ToDoExpanded, 
                    v=>v.ToDoExpander.IsExpanded),
                this.Bind(ViewModel,
                    vm=>vm.InProgressExpanded,
                    v=>v.InProgressExpander.IsExpanded),
                this.Bind(ViewModel,
                    vm=>vm.DoneExpanded,
                    v=>v.DoneExpander.IsExpanded)
            });
        }
    }
}
