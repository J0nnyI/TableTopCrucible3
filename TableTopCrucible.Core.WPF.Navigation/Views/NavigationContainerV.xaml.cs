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

using TableTopCrucible.Core.WPF.Navigation.ViewModels;

namespace TableTopCrucible.Core.WPF.Navigation.Views
{
    /// <summary>
    /// Interaction logic for NavigationContainerV.xaml
    /// </summary>
    public partial class NavigationContainerV : UserControl, IViewFor<NavigationContainerVM>
    {
        public NavigationContainerV()
        {
            InitializeComponent();
        }

        public NavigationContainerVM ViewModel { get; set; }
        object IViewFor.ViewModel { get; set; }
    }
}
