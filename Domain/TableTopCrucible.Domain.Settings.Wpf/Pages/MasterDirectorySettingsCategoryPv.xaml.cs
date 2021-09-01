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
    public partial class MasterDirectorySettingsCategoryPv : ReactiveUserControl<MasterDirectorySettingsCategoryPvm>, IActivatableView
    {
        public MasterDirectorySettingsCategoryPv()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.DirectoryList,
                    v=>v.DirectoryList.ViewModel)
            });
        }
    }
}
