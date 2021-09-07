using Ookii.Dialogs.Wpf;

using ReactiveUI;

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using FluentValidation;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.UserControls
{
    public class DirectoryPickerVm : ReactiveValidationObject, IActivatableViewModel
    {
        [Reactive]
        public string UserText { get; set; }
        public DirectoryPickerVm()
        {
            this.WhenActivated(()=>new []
            {
                
                DirectoryPath.RegisterValidator(this, vm=>vm.UserText, true)
            });
        }

        public ViewModelActivator Activator { get; } = new();
    }


    public partial class DirectoryPicker : ReactiveUserControl<DirectoryPickerVm>, IActivatableView, INotifyDataErrorInfo
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
            get => (DirectoryPath)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(
                nameof(Path),
                typeof(DirectoryPath),
                typeof(DirectoryPicker),
                new PropertyMetadata(null));

        public string UserText
        {
            get => (string)GetValue(UserTextProperty);
            set => SetValue(UserTextProperty, value);
        }
        public static readonly DependencyProperty UserTextProperty =
            DependencyProperty.Register(
                nameof(UserText),
                typeof(string),
                typeof(DirectoryPicker),
                new PropertyMetadata(string.Empty));



        public DirectoryPicker()
        {
            this.DataContext =this.ViewModel= new DirectoryPickerVm();
            InitializeComponent();
            // sync usertext and path
            this.WhenActivated(() => new[]
            {
                //sync UserText (raw input) and Path (validated input)
                Observable.CombineLatest(
                    this.WhenAnyValue(v => v.Path)
                        .Select(value => new {value, timeStamp = DateTime.Now}),
                    this.WhenAnyValue(v => v.UserText)
                        .Select(value => new {value, timeStamp = DateTime.Now}),
                    (path, userText)=>new {path, userText})
                .Where(x=>x?.userText?.value != x?.path?.value?.Value)
                .Subscribe(x =>
                {
                    if (x.path.timeStamp > x.userText.timeStamp)
                    {
                        this.UserText = Path.Value;
                        return;
                    }
                    if(DirectoryPath.IsValid(x.userText.value) == null)
                        this.Path = DirectoryPath.From(x.userText.value);
                }),
                // custom 2-way bind between vm.UserText and v.UserText to prevent loosing the initial value (initial sync vm.UserText => v.UserText)
                this.WhenAnyValue(v=>v.UserText)
                    .Where(text=>text != ViewModel.UserText)
                    .BindTo(this, v=>v.ViewModel.UserText),
                this.ObservableForProperty(v=>v.ViewModel.UserText)
                    .Select(c=>c.Value)
                    .Where(text=>text != this.UserText)
                    .BindTo(this, v=>v.UserText),
                
                // bug: filepicker error popup visible without error
            });
        }

        private void _buttonBase_OnClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            if(dialog.ShowDialog() == true)
                ViewModel.UserText = dialog.SelectedPath;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return ViewModel.GetErrors(propertyName);
        }

        public bool HasErrors => ViewModel.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => ViewModel.ErrorsChanged += value;
            remove => ViewModel.ErrorsChanged -= value;
        }
    }
}
