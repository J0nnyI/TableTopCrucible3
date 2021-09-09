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
using TableTopCrucible.Domain.Automation.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Automation.Wpf.Pages.Views
{
    /// <summary>
    /// Interaction logic for TagAutomationPageV.xaml
    /// </summary>
    public partial class TagAutomationPageV : ReactiveUserControl<TagAutomationPageVm>
    {
        public TagAutomationPageV()
        {
            InitializeComponent();
        }
    }
}
