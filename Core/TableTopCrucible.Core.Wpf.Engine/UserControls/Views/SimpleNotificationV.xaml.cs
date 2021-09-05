using System.Windows.Controls;
using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for SimpleNotification.xaml
    /// </summary>
    public partial class SimpleNotification : ReactiveUserControl<SimpleNotificationVm>
    {
        public SimpleNotification()
        {
            InitializeComponent();
        }
    }
}
