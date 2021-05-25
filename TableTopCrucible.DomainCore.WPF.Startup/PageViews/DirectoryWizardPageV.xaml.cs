using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
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
