using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Shared.Wpf.Models.TagEditor;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

// ReSharper disable once UnusedMember.Global
public partial class TagEditorChipV
{
    private CompositeDisposable _disposables;
    public TagEditorChipV()
    {
        InitializeComponent();

        TextEditor.CaretIndex = 0;

        this.WhenActivated(() => new[]
        {
            new ActOnLifecycle(null, () =>
            {
                _disposables?.Dispose();
                // prevent memory leak of inifinitly growing dispose list
                _disposables = new CompositeDisposable();
            }),

            this.OneWayBind(ViewModel,
                vm => vm.DisplayMode,
                v => v.MainContent.Visibility,
                displayMode => displayMode != DisplayMode.New
                    ? Visibility.Visible
                    : Visibility.Collapsed
            ),
            this.OneWayBind(ViewModel,
                vm => vm.DisplayMode,
                v => v.AddTag.Visibility,
                displayMode => displayMode == DisplayMode.New
                    ? Visibility.Visible
                    : Visibility.Collapsed
            ),

            #region dropdown
            this.OneWayBind(ViewModel,
                vm=>vm.IsDropDownOpen,
                v=>v.Dropdown.IsOpen),
            #endregion
            
            #region progress bar
            this.WhenAnyValue(v=>v.MainContent.ActualWidth)
                .BindTo(this, v=>v.ProgressBarContainer.Width),

            this.OneWayBind(ViewModel,
                vm=>vm.Distribution,
                v=>v.ProgressBar.Value,
                (Fraction dis)=>dis?.Value ?? 0),
            #endregion

            #region edit/viewer container
            this.OneWayBind(ViewModel,
                vm => vm.WorkMode,
                v => v.ContentContainer.Visibility,
                addMode => addMode == WorkMode.Edit
                    ? Visibility.Visible
                    : Visibility.Collapsed
            ),
            #endregion

            #region edit/view controls


            #region text viewer
            this.WhenAnyValue(
                v=>v.ViewModel!.DisplayMode,
                v=>v.ViewModel!.WorkMode,
                (displayMode, workMode)=>
                    displayMode != DisplayMode.New
                    && workMode == WorkMode.View
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .BindTo(this, v=>v.TextViewer.Visibility),
            this.OneWayBind(ViewModel,
                vm => vm.EditedTag,
                v => v.TextViewer.Text),
            #endregion

            #region text editor
            //this.OneWayBind(ViewModel,
            //    vm => vm.AvailableTags,
            //    v => v.TextEditor.ItemsSource),
            this.OneWayBind(ViewModel,
                vm => vm.WorkMode,
                v => v.TextEditor.MinWidth,
                editMode => editMode == WorkMode.Edit
                    ? 100
                    : 0),
            #endregion
            
            #region listview
            this.Bind(ViewModel,
                vm=>vm.SelectedTag,
                v=>v.SuggestedTags.SelectedItem),
            this.OneWayBind(ViewModel,
                vm=>vm.AvailableTags,
                v=>v.SuggestedTags.ItemsSource),
            this.ViewModel!
                .AreTagsAvailableChanges
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(tagsAvailable=>tagsAvailable
                    ? Visibility.Collapsed
                    : Visibility.Visible)
                .BindTo(this, v=>v.EmptyPlaceholder.Visibility),
            #endregion

            #endregion

            #region buttons
            #region toggle dropdwon button
            this.BindCommand(ViewModel,
                vm=>vm.ToggleDropDown,
                v=>v.ToggleDropDown),
            #endregion
            #region add button
            this.WhenAnyValue(
                v=>v.ViewModel!.DisplayMode,
                v=>v.ViewModel!.WorkMode,
                (displayMode, workMode)=>
                    displayMode == DisplayMode.New
                    && workMode == WorkMode.View
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .BindTo(this, v=>v.AddTag.Visibility),
            this.BindCommand(ViewModel,
                vm => vm.AddTagCommand,
                v => v.AddTag),
            #endregion
            #region confirm button
            this.OneWayBind(ViewModel,
                vm => vm.HasErrors,
                v => v.Confirm.IsEnabled,
                hasErrors => !hasErrors),
            this.BindCommand(ViewModel,
                vm => vm.SaveCommand,
                v => v.Confirm),
            #endregion
            #region delete/revert button
            this.OneWayBind(ViewModel,
                vm => vm.DeletionEnabled,
                v => v.RevertDelete.Visibility,
                editMode => editMode
                    ? Visibility.Visible
                    : Visibility.Collapsed
            ),
            this.BindCommand(ViewModel,
                vm=>vm.RemoveCommand,
                v=>v.RevertDelete),
            this.WhenAnyValue(
                v=>v.ViewModel!.DisplayMode,
                v=>v.ViewModel!.WorkMode,
                (displayMode, workMode)=>
                    displayMode == DisplayMode.New
                    ||  workMode == WorkMode.View
                    ? PackIconKind.Close
                    : PackIconKind.Undo)
                .BindTo(this, v=>v.RevertDeleteIcon.Kind),
            this.WhenAnyValue(
                v=>v.ViewModel!.DisplayMode,
                v=>v.ViewModel!.WorkMode,
                (displayMode, workMode)=>
                    displayMode != DisplayMode.New
                    ||  workMode == WorkMode.Edit
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .BindTo(this, v=>v.RevertDelete.Visibility),
            this.WhenAnyValue(
                v=>v.ViewModel!.DisplayMode,
                v=>v.ViewModel!.WorkMode,
                (displayMode, workMode)=>
                    displayMode == DisplayMode.New
                    ? "Cancel new Tag"
                    : workMode == WorkMode.Edit
                    ? "Revert Changes"
                    : "Delete Tag")
                .BindTo(this, v=>v.RevertDelete.ToolTip),
            #endregion
            #endregion

            #region interactions
            ViewModel!.FocusEditorInteraction.RegisterAutoCompleteHandler(focusTextBox),
            ViewModel!.UnfocusEditorInteraction.RegisterAutoCompleteHandler(UnfocusTextBox),
            #endregion
            });
    }
    private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case Key.Down:
                ViewModel!.IsDropDownOpen = true;
                SuggestedTags.SelectNext();
                break;
            case Key.Up:
                ViewModel!.IsDropDownOpen = true;
                SuggestedTags.SelectPrevious();
                break;
        }
        handleEnterEscape(e.Key);
    }

    private void Chip_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        => ViewModel!.EnterEditMode();

    private void focusTextBox()
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (ContentContainer.Visibility != Visibility.Visible)
                return;
            TextEditor.Focus();
            ViewModel!.IsDropDownOpen = true; ;
        }, (DispatcherPriority)5);
    }
    private void UnfocusTextBox()
    {
        Keyboard.ClearFocus();
        TextEditor.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
    }

    private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel!.WorkMode == WorkMode.Edit)
            return;

        focusTextBox();
    }

    private void ListBoxItem_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListViewItem item || item.IsSelected == false)
            return;
        ViewModel!.EditedTag = ViewModel.SelectedTag;
        ViewModel!.Confirm();
    }
    private void handleEnterEscape(Key key)
    {
        switch (key)
        {
            case Key.Enter:
                if (ViewModel!.SelectedTag is not null
                    && ViewModel.IsDropDownOpen)
                    ViewModel.EditedTag = ViewModel.SelectedTag;
                ViewModel!.Confirm();
                break;
            case Key.Escape:
                if (ViewModel!.IsDropDownOpen)
                {
                    ViewModel.SelectedTag = null;
                    ViewModel.IsDropDownOpen = false;
                }
                else
                    ViewModel!.Revert();
                break;
        }
    }
    private void Dropdown_PreviewKeyDown(object sender, KeyEventArgs e)
        => handleEnterEscape(e.Key);

}
