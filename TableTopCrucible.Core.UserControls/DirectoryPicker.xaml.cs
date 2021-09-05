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
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.UserControls
{



    public partial class DirectoryPicker : UserControl, IActivatableView, INotifyDataErrorInfo
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
            InitializeComponent();
            // sync usertext and path
            this.WhenActivated(() => new[]
            {
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
                // bug: filepicker error popup visible without error

                this.WhenAnyValue(v=>v.UserText)
                    .Select(_=>GetErrors(nameof(UserText)) == null)
                    .BindTo(this, v=>v.HasNoErrors),

                this.WhenAnyValue(v=>v.UserText)
                    .Select(userText=>GetErrors(nameof(userText)) == null)
                    //.DistinctUntilChanged()
                    .Subscribe(hasErrors =>
                    {
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserText)));
                    }, e =>
                    {

                    })
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            if(dialog.ShowDialog() == true)
                UserText = dialog.SelectedPath;
        }


        public IEnumerable GetErrors(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(UserText):
                    var err = DirectoryPath.IsValid(UserText, true)?.Message;
                    return err == null
                        ? null
                        : new[] { err };
                default:
                    return null;
            }
        }

        [Reactive]
        public bool HasNoErrors { get; private set; }
        public bool HasErrors => GetErrors(nameof(UserText)) != null;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}
