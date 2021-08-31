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
using ReactiveUI;
using TableTopCrucible.Domain.Settings.Wpf.PageViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for MasterDirectorySettingsPv.xaml
    /// </summary>
    public partial class MasterDirectorySettingsPv : UserControl, IViewFor<MasterDirectorySettingsCategoryPvm>
    {
        public MasterDirectorySettingsPv()
        {
            InitializeComponent();
        }

        object? IViewFor.ViewModel
        {
            get => DataContext;
            set => DataContext = value;
        }

        public MasterDirectorySettingsCategoryPvm ViewModel
        {
            get => DataContext as MasterDirectorySettingsCategoryPvm;
            set => DataContext = value;
        }

    }
}
