using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

/// <summary>
/// Interaction logic for TagEditorV.xaml
/// </summary>
public partial class TagEditorV : ReactiveUserControl<TagEditorVm>
{
    public TagEditorV()
    {
        InitializeComponent();
        this.WhenActivated(() => new IDisposable[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.ChipList,
                v => v.TagList.ItemsSource),
            this.WhenAnyValue(v=>v.ViewModel.IsBusy)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(busy =>
                {
                    if (busy)
                    {
                        BusyIndicator.Visibility = Visibility.Visible;
                        TagList.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        BusyIndicator.Visibility = Visibility.Collapsed;
                        TagList.Visibility = Visibility.Visible;
                    }
                }),
        });
    }
}