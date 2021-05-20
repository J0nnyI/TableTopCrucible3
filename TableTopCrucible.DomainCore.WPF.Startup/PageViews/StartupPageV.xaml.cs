using ReactiveUI;

using System;
using System.Collections.Generic;
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
    /// Interaction logic for StartupPageV.xaml
    /// </summary>
    public partial class StartupPageV : UserControl, IViewFor<StartupPageVM>
    {
        public StartupPageV()
        {
            InitializeComponent();
        }

        public StartupPageVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as StartupPageVM; }
    }
}
