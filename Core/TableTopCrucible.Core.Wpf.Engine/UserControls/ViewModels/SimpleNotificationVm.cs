using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
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
        public int DeleteCountdownTotal => SettingsHelper.NotificationResolution;
        private ObservableAsPropertyHelper<int> _deleteCountdownProgress;
        public int DeleteCountdownProgress => _deleteCountdownProgress.Value;
        private ObservableAsPropertyHelper<bool> _deleteCountdownRunning;
        public bool DeleteCountdownRunning => _deleteCountdownRunning.Value;
        private ObservableAsPropertyHelper<bool> _closable;
        private readonly INotificationService _notificationService;
        public bool Closable => _closable.Value;
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

            var deleteCountdownActiveChanges = this.WhenAnyValue(v => v.Type)
                .Select(t => t == NotificationType.Confirmation);

            return new CompositeDisposable(new IDisposable[]
            {
                deleteCountdownActiveChanges
                    .Where(s=>s)
                    .Select(_=>
                        Observable.Interval(SettingsHelper.NotificationDelay / SettingsHelper.NotificationResolution))
                    .Switch()
                    .Scan(SettingsHelper.NotificationResolution, (a,b)=>a-1)
                    .Take(SettingsHelper.NotificationResolution)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(current=>
                    {
                        if(current == 0)
                            _notificationService.RemoveNotification(Id);
                    })
                    .ToProperty(this, v=>v.DeleteCountdownProgress, out _deleteCountdownProgress),
                deleteCountdownActiveChanges
                    .ToProperty(
                        this, 
                        v=>v.DeleteCountdownRunning,
                        out _deleteCountdownRunning,
                        false,RxApp.MainThreadScheduler),
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
