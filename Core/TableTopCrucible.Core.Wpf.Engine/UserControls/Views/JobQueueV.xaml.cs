using System.Reactive.Linq;
using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views;

/// <summary>
///     Interaction logic for JobQueueV.xaml
/// </summary>
public partial class JobQueueV : ReactiveUserControl<JobQueueVm>
{
    public JobQueueV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.Cards,
                v => v.Jobs.ItemsSource),

            this.WhenAnyValue(
                    v => v.ViewModel.Cards.Count)
                .Select(count => count > 0)
                .DistinctUntilChanged()
                .Select(show => show
                    ? 0
                    : 1)
                .BindTo(this, v => v.EmptyListText.Opacity)
        });
    }
}