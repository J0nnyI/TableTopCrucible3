using ReactiveUI;

using System.Windows.Controls;

namespace TableTopCrucible.DomainCore.WPF.ItemList.Views
{
    /// <summary>
    /// Interaction logic for ItemListV.xaml
    /// </summary>
    public partial class ItemListV : UserControl, IViewFor<ItemListV>
    {
        public ItemListV()
        {
            InitializeComponent();
        }

        public ItemListV ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as ItemListV;
        }
    }
}
