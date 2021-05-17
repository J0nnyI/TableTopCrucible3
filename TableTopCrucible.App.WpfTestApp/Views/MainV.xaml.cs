using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
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

using TableTopCrucible.App.WpfTestApp.Pages;
using TableTopCrucible.App.WpfTestApp.PageViewModels;
using TableTopCrucible.App.WpfTestApp.ViewModels;
using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.App.WpfTestApp.Views
{
    /// <summary>
    /// Interaction logic for MainV.xaml
    /// </summary>
    public partial class MainV : UserControl, IViewFor<MainVM>, IActivatableView
    {


        public MainV()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.GoNext, x => x.GoNextButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.GoBack, x => x.GoBackButton)
                    .DisposeWith(disposables);
            });


        }

        public MainVM ViewModel { get; set; }
        object IViewFor.ViewModel { get=>ViewModel; set=>ViewModel = value as MainVM; }
    }
}
