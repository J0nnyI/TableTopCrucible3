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
using DynamicData;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for JobQueueV.xaml
    /// </summary>
    public partial class JobQueueV : ReactiveUserControl<JobQueueVm>
    {
        public JobQueueV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.Cards,
                    v=>v.Jobs.ItemsSource),

                this.WhenAnyValue(
                    v=>v.ViewModel.Cards.Count)
                    .Select(count => count > 0)
                    .DistinctUntilChanged()
                    .Select(show=>
                        ObservableHelper.AnimateValue(1,0)
                            .Select(value=>show?value:1-value))
                    .Switch()
                    .BindTo(this, v=>v.EmptyListText.Opacity)
            });
        }
    }
}
