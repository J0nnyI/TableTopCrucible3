using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    public class SimpleNotificationVm : ReactiveObject, INotification, IActivatableViewModel
    {
        private readonly Subject<Unit> _closedByUser = new();
        private readonly NotificationIdentifier _identifier;


        private readonly INotificationService _notificationService;

        private ObservableAsPropertyHelper<double> _cardOpacity;

        private ObservableAsPropertyHelper<bool> _closable;

        private ObservableAsPropertyHelper<double> _deleteCountdownProgress;

        private ObservableAsPropertyHelper<bool> _deleteCountdownRunning;


        internal SimpleNotificationVm(Name title, Description content, NotificationType type,
            NotificationIdentifier identifier)
        {
            _identifier = identifier;
            Title = title;
            Content = content;
            Type = type;
            _notificationService = Locator.Current!.GetService<INotificationService>();
            this.WhenActivated(() => new[]
            {
                _initDeleteCountdown(),
                ReactiveCommandHelper.Create(() => _closedByUser.OnNext(Unit.Default)
                    , cmd => CloseNotificationCommand = cmd),
                this.WhenAnyValue(vm => vm.Type, type => type != NotificationType.Error)
                    .ToProperty(this, vm => vm.Closable, out _closable)
            });
        }

        public Name Title { get; }
        public Description Content { get; }

        public double DeleteCountdownTotal => SettingsHelper.NotificationResolution;
        public double DeleteCountdownProgress => _deleteCountdownProgress.Value;
        public double CardOpacity => _cardOpacity.Value;
        public bool DeleteCountdownRunning => _deleteCountdownRunning.Value;
        public bool Closable => _closable.Value;
        public ICommand CloseNotificationCommand { get; private set; }

        public ViewModelActivator Activator { get; } = new();
        public NotificationId Id { get; } = NotificationId.New();
        public NotificationIdentifier Identifier { get; init; }
        public DateTime Timestamp { get; } = DateTime.Now;
        public NotificationType Type { get; }

        private IDisposable _initDeleteCountdown()
        {
            if (!SettingsHelper.AutoCloseEnabled)
                return new CompositeDisposable();

            return new CompositeDisposable(this.WhenAnyValue(v => v.Type)
                .Select(t => t == NotificationType.Confirmation)
                .OutputObservable(out var deleteCountdownActiveChanges)
                .ToProperty(
                    this,
                    v => v.DeleteCountdownRunning,
                    out _deleteCountdownRunning,
                    false, RxApp.MainThreadScheduler), deleteCountdownActiveChanges
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Where(s => s)
                .Take(1)
                .Select(_ =>
                    ObservableHelper.AnimateValue(
                        100,
                        0,
                        SettingsHelper.NotificationDelay,
                        RxApp.TaskpoolScheduler)
                )
                .Switch()
                .OutputObservable(out var deleteCountdownProgressChanges)
                .ToProperty(this, vm => vm.DeleteCountdownProgress, out _deleteCountdownProgress, false,
                    RxApp.MainThreadScheduler), deleteCountdownProgressChanges
                .Select(_ => 1.0) // ignore all updates
                .DistinctUntilChanged()
                .TakeUntil(_closedByUser) // start last closing animation once the button is clicked
                .Concat(
                    ObservableHelper.AnimateValue(
                        1,
                        .1,
                        SettingsHelper.AnimationDuration,
                        RxApp.TaskpoolScheduler)
                )
                .Finally(() => _notificationService.RemoveNotification(Id))
                .ObserveOn(RxApp.MainThreadScheduler)
                .StartWith(1)
                .ToProperty(this, vm => vm.CardOpacity, out _cardOpacity, false, RxApp.MainThreadScheduler));
        }
    }
}