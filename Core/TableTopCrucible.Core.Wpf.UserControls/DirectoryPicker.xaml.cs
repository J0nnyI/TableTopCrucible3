using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Wpf.UserControls
{
    public class DirectoryPickerVm : ReactiveValidationObject, IActivatableViewModel
    {
        public DirectoryPickerVm()
        {
            this.WhenActivated(() => new[]
            {
                DirectoryPath.RegisterValidator(this, vm => vm.UserText)
            });
        }

        [Reactive]
        public string UserText { get; set; }

        public ViewModelActivator Activator { get; } = new();
    }


    public partial class DirectoryPicker : ReactiveUserControl<DirectoryPickerVm>, IActivatableView,
        INotifyDataErrorInfo
    {
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register(
                nameof(Hint),
                typeof(string),
                typeof(DirectoryPicker),
                new PropertyMetadata("Directory"));

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(
                nameof(Path),
                typeof(DirectoryPath),
                typeof(DirectoryPicker),
                new PropertyMetadata(null));

        public static readonly DependencyProperty UserTextProperty =
            DependencyProperty.Register(
                nameof(UserText),
                typeof(string),
                typeof(DirectoryPicker),
                new PropertyMetadata(string.Empty));

        private readonly Subject<DirectoryPath> _dialogConfirmed = new();


        public DirectoryPicker()
        {
            DataContext = ViewModel = new DirectoryPickerVm();
            InitializeComponent();
            // sync user text and path
            this.WhenActivated(() => new[]
            {
                //sync UserText (raw input) and Path (validated input)
                this.WhenAnyValue(v => v.Path)
                    .Select(value => new { value, timeStamp = DateTime.Now }).CombineLatest(this
                            .WhenAnyValue(v => v.UserText)
                            .Select(value => new { value, timeStamp = DateTime.Now }),
                        (path, userText) => new { path, userText })
                    .Where(x => x?.userText?.value != x?.path?.value?.Value)
                    .Subscribe(x =>
                    {
                        if (x.path.timeStamp > x.userText.timeStamp)
                        {
                            UserText = Path.Value;
                            return;
                        }

                        if (DirectoryPath.IsValid(x.userText.value) == null)
                            Path = DirectoryPath.From(x.userText.value);
                    }),
                // custom 2-way bind between vm.UserText and v.UserText to prevent loosing the initial value (initial sync vm.UserText => v.UserText)
                this.WhenAnyValue(v => v.UserText)
                    .Where(text => text != ViewModel.UserText)
                    .BindTo(this, v => v.ViewModel.UserText),
                this.ObservableForProperty(v => v.ViewModel.UserText)
                    .Select(c => c.Value)
                    .Where(text => text != UserText)
                    .BindTo(this, v => v.UserText)

                // bug: filePicker error popup visible without error
            });
        }

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public DirectoryPath Path
        {
            get => (DirectoryPath)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public string UserText
        {
            get => (string)GetValue(UserTextProperty);
            set => SetValue(UserTextProperty, value);
        }

        public IObservable<DirectoryPath> DialogConfirmed => _dialogConfirmed;

        public IEnumerable GetErrors(string propertyName) => ViewModel.GetErrors(propertyName);

        public bool HasErrors => ViewModel.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => ViewModel.ErrorsChanged += value;
            remove => ViewModel.ErrorsChanged -= value;
        }

        private void _buttonBase_OnClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() == true)
            {
                ViewModel.UserText = dialog.SelectedPath;
                _dialogConfirmed.OnNext((DirectoryPath)dialog.SelectedPath);
            }
        }
    }
}