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

using TableTopCrucible.Core.WPF.MainWindow.ViewModels;

namespace TableTopCrucible.Core.WPF.MainWindow.Views
{

    /// <summary>
    /// Interaction logic for MainWindowV.xaml
    /// </summary>
    public partial class MainWindowV : UserControl, IViewFor<MainPageVM>
    {
        public MainWindowV()
        {
            InitializeComponent();
        }

        public MainPageVM ViewModel { get => DataContext as MainPageVM; set => DataContext = value; }
        object IViewFor.ViewModel { get => DataContext; set => DataContext = value; }
    }
}
