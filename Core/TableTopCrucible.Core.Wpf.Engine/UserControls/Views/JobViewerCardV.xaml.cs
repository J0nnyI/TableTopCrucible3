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
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for JobViewerCardV.xaml
    /// </summary>
    public partial class JobViewerCardV : ReactiveUserControl<JobViewerCardVm>
    {
        public JobViewerCardV()
        {
            InitializeComponent();
            this.WhenActivated(()=>new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.Viewer.Title.Value,
                    v=>v.Title.Text),
                this.WhenAnyObservable(v=>v.ViewModel.Viewer.TargetProgressChanges)
                    .Select(target=>target.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.Progress.Maximum),
                this.WhenAnyObservable(v=>v.ViewModel.Viewer.CurrentProgressChanges)
                    .Select(target=>target.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.Progress.Value),
            });
        }
    }
}
