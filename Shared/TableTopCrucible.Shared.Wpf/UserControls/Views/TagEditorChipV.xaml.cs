using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using MaterialDesignThemes.Wpf;

using ReactiveUI;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    // ReSharper disable once UnusedMember.Global
    public partial class TagEditorItemV
    {
        public TagEditorItemV()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.TextEditor.Text).Subscribe(x =>
            {

            });

            this.WhenActivated(() => new[]
            {
                #region add button
                this.OneWayBind(ViewModel,
                    vm => vm.AddMode,
                    v => v.AddTag.Visibility,
                    addMode => addMode
                        ? Visibility.Visible
                        : Visibility.Collapsed
                ),
                this.BindCommand(ViewModel,
                    vm=>vm.AddTagCommand,
                    v=>v.AddTag),
                #endregion
                #region edit/viewer container
                this.OneWayBind(ViewModel,
                    vm => vm.AddMode,
                    v => v.ContentContainer.Visibility,
                    addMode => addMode
                        ? Visibility.Collapsed
                        : Visibility.Visible
                ),
                #endregion
                #region text viewer
                this.OneWayBind(ViewModel,
                    vm => vm.EditModeEnabled,
                    v => v.TextViewer.Visibility,
                    addMode => addMode
                        ? Visibility.Collapsed
                        : Visibility.Visible
                ),
                this.OneWayBind(ViewModel,
                    vm=>vm.EditedTag,
                    v=>v.TextViewer.Text),
                #endregion
                #region text editor
                this.OneWayBind(ViewModel,
                    vm => vm.AvailableTags,
                    v => v.TextEditor.ItemsSource),
                this.OneWayBind(ViewModel,
                    vm => vm.EditModeEnabled,
                    v => v.TextEditor.MinWidth,
                    editMode => editMode
                        ? 100
                        : 0
                ),
                this.OneWayBind(ViewModel,
                    vm => vm.EditModeEnabled,
                    v => v.TextEditor.Visibility,
                    editMode => editMode
                        ? Visibility.Visible
                        : Visibility.Collapsed
                ),
                this.Bind(ViewModel,
                    vm=>vm.EditedTag,
                    v=>v.TextEditor.Text),
                #endregion
                #region confirm button
                this.OneWayBind(ViewModel,
                    vm => vm.EditModeEnabled,
                    v => v.Confirm.Visibility,
                    editMode => editMode
                        ? Visibility.Visible
                        : Visibility.Collapsed
                ),
                this.OneWayBind(ViewModel,
                    vm=>vm.HasErrors,
                    v=>v.Confirm.IsEnabled,
                    hasErrors=>!hasErrors),
                this.BindCommand(ViewModel,
                    vm=>vm.SaveCommand,
                    v=>v.Confirm),
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
                    v=>v.ViewModel.EditModeEnabled,
                    v=>v.ViewModel.IsNew,
                    (editModeEnabled, isNew)=>editModeEnabled && !isNew
                        ? PackIconKind.Undo
                        : PackIconKind.Close)
                    .BindTo(this, v=>v.RevertDeleteIcon.Kind),
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
                case Key.Enter:
                    ViewModel!.Confirm();
                    break;
                case Key.Escape:
                    ViewModel!.Revert();
                    break;
                default:
                    return;
            }
        }

        private void Chip_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel!.EnterEditMode();
        }

        private void focusTextBox()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (TextEditor.Visibility != Visibility.Visible)
                    return;
                TextEditor.Focus();
                TextEditor.IsDropDownOpen = true;
            }, (DispatcherPriority)5);
        }
        private void UnfocusTextBox()
        {
            Keyboard.ClearFocus();
            TextEditor.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel!.EditModeEnabled)
                return;

            focusTextBox();
        }
    }
}
