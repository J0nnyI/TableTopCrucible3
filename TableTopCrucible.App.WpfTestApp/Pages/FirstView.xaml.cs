using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;

using TableTopCrucible.App.WpfTestApp.PageViewModels;

namespace TableTopCrucible.App.WpfTestApp.Pages
{
    /// <summary>
    /// Interaction logic for FirstView.xaml
    /// </summary>
    public partial class FirstView : UserControl, IActivatableView, IViewFor<FirstViewModel>
    {
        public FirstView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.UrlPathSegment, v => v.PathTextBlock.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Value, v => v.DemoTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.SuffixFills, v => v.suffixFills.Text, v => v.ToString())
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.SetText, v => v.setText)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel,
                        vm => vm.AddText,
                        v => v.addText,
                        this.WhenAnyValue(v => v.parameter.Text))
                    .DisposeWith(disposables);

                this.parameter
                    .WhenAnyValue(v => v.Text)
                    .Select(txt => !string.IsNullOrWhiteSpace(txt))
                    .BindTo(ViewModel, vm => vm.SuffixFills)
                    .DisposeWith(disposables);

            });

        }
        [Reactive]
        public FirstViewModel ViewModel { get; set; }
        [Reactive]
        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = value as FirstViewModel;
        }
    }
}
