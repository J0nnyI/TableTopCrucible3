using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient(typeof(BannerListVm))]
    public interface IBannerList
    {

    }
    public class BannerListVm:ReactiveObject ,IBannerList,IActivatableViewModel
    {
        private readonly ObservableCollectionExtended<INotification> _notificationList = new();
        public ObservableCollectionExtended<INotification> NotificationList =>_notificationList;
        public BannerListVm(INotificationService notificationService)
        {
            this.WhenActivated(()=>new []
            {
                notificationService.Notifications
                    .Connect()
                    .Bind(_notificationList)
                    .Subscribe(),
            });
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
