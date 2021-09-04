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

using Ookii.Dialogs.Wpf;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.UserControls
{



    public partial class DirectoryPicker : UserControl
    {

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value); 
        }
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register(
                nameof(Hint),
                typeof(string), 
                typeof(DirectoryPicker),
                new PropertyMetadata("Directory"));





        public DirectoryPath Path
        {
            get =>(DirectoryPath)GetValue(PathProperty); 
            set => SetValue(PathProperty, value); 
        }
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(
                nameof(Path), 
                typeof(DirectoryPath), 
                typeof(DirectoryPicker),
                new PropertyMetadata(null));




        public DirectoryPicker()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            dialog.ShowDialog();
            Path =DirectoryPath.From(dialog.SelectedPath);
        }
    }
}
