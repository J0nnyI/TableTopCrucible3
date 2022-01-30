using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    /// <summary>
    ///     Interaction logic for ItemDataViewerV.xaml
    /// </summary>
    public partial class ItemDataViewerV : ReactiveUserControl<ItemDataViewerVm>
    {
        public ItemDataViewerV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.TagEditor,
                    v=>v.TagEditor.ViewModel)
            });
        }
    }
}