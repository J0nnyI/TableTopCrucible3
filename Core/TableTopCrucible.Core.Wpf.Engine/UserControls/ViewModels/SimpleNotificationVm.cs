using ReactiveUI;

using Splat;

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{

    public class SimpleNotificationVm : ReactiveObject, INotification, IActivatableViewModel
    {
        public Name Title { get; }
        public Description Content { get; }
        public NotificationId Id { get; } = NotificationId.New();
        public DateTime Timestamp { get; } = DateTime.Now;
        public NotificationType Type { get; }

        public ViewModelActivator Activator { get; } = new();

        public double DeleteCountdownTotal => SettingsHelper.NotificationResolution;

        private ObservableAsPropertyHelper<double> _deleteCountdownProgress;
        public double DeleteCountdownProgress => _deleteCountdownProgress.Value;

        private ObservableAsPropertyHelper<double> _cardOpacity;
        public double CardOpacity => _cardOpacity.Value;

        private ObservableAsPropertyHelper<bool> _deleteCountdownRunning;
        public bool DeleteCountdownRunning => _deleteCountdownRunning.Value;

        private ObservableAsPropertyHelper<bool> _closable;
        public bool Closable => _closable.Value;


        private readonly INotificationService _notificationService;
        public ICommand CloseNotificationCommand { get; private set; }


        internal SimpleNotificationVm(Name title, Description content, NotificationType type)
        {
            Title = title;
            Content = content;
            Type = type;
            this._notificationService = Locator.Current!.GetService<INotificationService>();
            this.WhenActivated(() => new[]
            {
                _initDeleteCountdown(),
                _initCommands(),
                this.WhenAnyValue(vm=>vm.Type, type=>type != NotificationType.Error)
                    .ToProperty(this, vm=>vm.Closable, out _closable)
            });
        }

        private IDisposable _initDeleteCountdown()
        {
            if (!SettingsHelper.AutocloseEnabled)
                return new CompositeDisposable();

            return new CompositeDisposable(new IDisposable[]
            {
                // timer enabled
                this.WhenAnyValue(v => v.Type)
                    .Select(t => t == NotificationType.Confirmation)
                    .OutputObservable(out var deleteCountdownActiveChanges)
                    .ToProperty(
                        this,
                        v=>v.DeleteCountdownRunning,
                        out _deleteCountdownRunning,
                        false,RxApp.MainThreadScheduler),

                // timer countdown
                deleteCountdownActiveChanges
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Where(s => s)
                    .Take(1)
                    .Select(_ =>
                        ObservableHelper.AnimateValue(100, 0,
                            SettingsHelper.NotificationDelay)
                    )
                    .Switch()
                    .OutputObservable(out var deleteCountdownProgressChanges)
                    .ToProperty(this, vm=>vm.DeleteCountdownProgress, out _deleteCountdownProgress),

                // card fadeout
                deleteCountdownProgressChanges
                    .Select(_ => 1.0) // ignore all updates
                    .DistinctUntilChanged()
                    .Concat(ObservableHelper.AnimateValue(1, .3))// when countdown
                    .Finally(() =>
                    {
                        _notificationService.RemoveNotification(Id);
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, vm => vm.CardOpacity, out _cardOpacity),
            });
        }
        private IDisposable _initCommands()
        {
            var close = ReactiveCommand.Create(() =>
                _notificationService.RemoveNotification(Id)
            );
            this.CloseNotificationCommand = close;
            return new CompositeDisposable(new[]
            {
                close
            });
        }

    }
}
