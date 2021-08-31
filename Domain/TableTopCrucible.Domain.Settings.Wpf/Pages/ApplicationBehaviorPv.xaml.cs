using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for ApplicationBehavior.xaml
    /// </summary>
    public partial class ApplicationBehaviorPv : ReactiveUserControl<ApplicationBehaviorPvm>
    {
        public ApplicationBehaviorPv()
        {
            InitializeComponent();
        }
    }
}
