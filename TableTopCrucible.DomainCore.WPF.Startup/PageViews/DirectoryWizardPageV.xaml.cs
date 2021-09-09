using ReactiveUI;

using System.Reactive.Disposables;

using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.PageViews
{
    /// <summary>
    /// Interaction logic for DirectoryWizardPageV.xaml
    /// </summary>
    public partial class DirectoryWizardPageV : ReactiveUserControl<DirectoryWizardPageVM>
    {
        public DirectoryWizardPageV()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.DirectoryList, v => v.DirectoryList.ViewModel)
                    .DisposeWith(disposables);

            });
        }

    }
}
