using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.Engine.Helper;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Models
{
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

        public NotificationInfo(INotification notification)
        {
            Notification = notification;

            TimeRemainingChanges =
                SettingsHelper.NotificationAutoDeleteTime
                    .StartTimer(_onCountdownRunning, _onCancelCountdown, null, RxApp.TaskpoolScheduler)
                    .Select(DeletionTime.From)
                    .Replay()
                    .RefCount();
            IsCompletedChanges = TimeRemainingChanges
                .Select(time => time.Value <= TimeSpan.Zero)
                .DistinctUntilChanged();

            _disposables.Add(
                TimeRemainingChanges.Subscribe(),// prevents timer reset when TimeRemaining has no subscribers
                IsCompletedChanges.ToPropertyEx(this, vm => vm.IsCompleted, false, RxApp.MainThreadScheduler),
                _onCountdownRunning,
                _onCancelCountdown);
        }

        public void StartCountdown()
        {
            if (IsCompleted)
                throw new InvalidOperationException("the timer has already been completed");
            if (_onCountdownRunning.Value)
                return;
            _onCountdownRunning.OnNext(true);
        }

        /// <summary>
        /// pauses the countdown - it can be continued with <see cref="StartCountdown"/>
        /// </summary>
        public void PauseCountdown()
        {
            if (_onCountdownRunning.Value is false && IsCompleted)
                return;
            _onCountdownRunning.OnNext(false);
        }
        /// <summary>
        /// stops the countdown and all timer are considered completed - the countdown can no longer be continued 
        /// </summary>
        public void StopCountdown()
            => _onCancelCountdown.OnNext();

        #region time cycles
        private readonly BehaviorSubject<bool> _onCountdownRunning = new(true);
        private readonly Subject<Unit> _onCancelCountdown = new();//bs is used to access

        [ObservableAsProperty]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool IsCompleted { get; }
        public IObservable<bool> IsCompletedChanges { get; }

        public CreationTime Created { get; } = CreationTime.Now;
        /// <summary>
        /// the time until the notification is removed
        /// </summary>
        public IObservable<DeletionTime> TimeRemainingChanges { get; }
        #endregion


        #region  INotification
        public NotificationId Id => Notification.Id;
        public NotificationType Type => Notification.Type;
        public NotificationIdentifier Identifier => Notification.Identifier;
        public Name Title => Notification.Title;
        public Description Content => Notification.Content;
        #endregion
    }
}