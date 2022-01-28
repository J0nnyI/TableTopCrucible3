using System.Windows;
using System.Windows.Controls;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : UserControl
    {
        public Message Text
        {
            get => (Message)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(Message), typeof(LoadingScreen), new PropertyMetadata(Message.From("Loading")));
        
        public LoadingScreen()
        {
            InitializeComponent();
        }
    }
}
