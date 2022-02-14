using System;
using System.Reactive.Subjects;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

public enum YesNoDialogResult
{
    Yes,
    No
}

public interface IYesNoDialog
{
    IObservable<YesNoDialogResult> Result { get; }
}

public class YesNoDialogVm : ReactiveObject, IActivatableViewModel, IYesNoDialog
{
    private readonly Subject<YesNoDialogResult> _dialogResult = new();

    private bool closed;

    internal YesNoDialogVm(string text)
    {
        Text = text;

        this.WhenActivated(() => new[]
        {
            ReactiveCommandHelper.Create(
                () =>
                {
                    closed = true;
                    Session.Close();
                    _dialogResult.OnNext(YesNoDialogResult.Yes);
                    _dialogResult.OnCompleted();
                },
                c => YesClickedCommand = c
            ),
            ReactiveCommandHelper.Create(Close,
                c => NoClickedCommand = c
            )
        });
    }

    public string Text { get; }
    public ICommand YesClickedCommand { get; private set; }
    public ICommand NoClickedCommand { get; private set; }
    public DialogSession Session { get; set; }
    public ViewModelActivator Activator { get; } = new();
    public IObservable<YesNoDialogResult> Result => _dialogResult;

    public void Close()
    {
        if (closed)
            return;
        closed = true;
        Session.Close();
        _dialogResult.OnNext(YesNoDialogResult.No);
        _dialogResult.OnCompleted();
    }
}