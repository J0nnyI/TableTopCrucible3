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

using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.Views
{
    /// <summary>
    /// Interaction logic for RecentMasterFileListV.xaml
    /// </summary>
    public partial class RecentMasterFileListV : UserControl, IViewFor<RecentMasterFileListVM>
    {
        public RecentMasterFileListV()
        {
            InitializeComponent();
        }

        public RecentMasterFileListVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as RecentMasterFileListVM; }
    }
}
