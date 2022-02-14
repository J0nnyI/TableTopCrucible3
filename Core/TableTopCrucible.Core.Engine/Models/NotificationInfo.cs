using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Models;

/// <summary>
/// additional runtime fields for a notification, used by the service, consumed by the VMs
/// <br/>
/// Disposing this class makes it inoperable (all internal subjects are disposed
/// </summary>
public class NotificationInfo : ReactiveObject, INotification, IDisposable
{
    public INotification Notification { get; }
    private readonly CompositeDisposable _disposables = new();
    void IDisposable.Dispose() => _disposables.Dispose();
    private readonly BehaviorSubject<Unit> _resetTimer = new(Unit.Default);

    public NotificationInfo(INotification notification)
    {
        Notification = notification;

        TimeRemainingChanges =
            _resetTimer.Select(_ =>
                    SettingsHelper.NotificationAutoDeleteTime
                        .StartTimer(this.WhenAnyValue(x => x.CountdownRunning), _onCancelCountdown, null,
                            RxApp.TaskpoolScheduler)
                ).Switch()
                .Select(DeletionTime.From)
                .Replay()
                .RefCount();
        IsCompletedChanges = TimeRemainingChanges
            .Select(time => time.Value <= TimeSpan.Zero)
            .DistinctUntilChanged();


        this.WhenAnyValue(vm => vm.IsExpanded)
            .Subscribe((expanded) =>
            {
                if (expanded)
                    PauseCountdown();
                else if (IsCompleted is false)
                    StartCountdown();
            });

        _disposables.Add(
            TimeRemainingChanges.Subscribe(), // prevents timer reset when TimeRemaining has no subscribers
            IsCompletedChanges.ToPropertyEx(this, vm => vm.IsCompleted, false, RxApp.MainThreadScheduler),
            _onCancelCountdown,
            _resetTimer);
    }

    public void StartCountdown()
    {
        if (IsCompleted)
            throw new InvalidOperationException("the timer has already been completed");
        if (CountdownRunning)
            return;
        CountdownRunning = true;
        _resetTimer.OnNext();
    }

    /// <summary>
    /// pauses the countdown - it can be continued with <see cref="StartCountdown"/>
    /// </summary>
    public void PauseCountdown()
    {
        if (CountdownRunning is false && IsCompleted)
            return;
        CountdownRunning = false;
    }

    /// <summary>
    /// stops the countdown and all timer are considered completed - the countdown can no longer be continued 
    /// </summary>
    public void StopCountdown()
        => _onCancelCountdown.OnNext();

    #region time cycles

    [Reactive]
    public bool CountdownRunning { get; private set; }

    private readonly Subject<Unit> _onCancelCountdown = new(); //bs is used to access

    [ObservableAsProperty]
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public bool IsCompleted { get; }

    public IObservable<bool> IsCompletedChanges { get; }

    public CreationTime Created { get; } = CreationTime.Now;

    /// <summary>
    /// the time until the notification is removed
    /// </summary>
    public IObservable<DeletionTime> TimeRemainingChanges { get; }

    [Reactive]
    public bool IsExpanded { get; set; }

    #endregion


    #region INotification

    public NotificationId Id => Notification.Id;
    public NotificationType Type => Notification.Type;
    public NotificationIdentifier Identifier => Notification.Identifier;
    public Name Title => Notification.Title;
    public Description Content => Notification.Content;

    #endregion
}