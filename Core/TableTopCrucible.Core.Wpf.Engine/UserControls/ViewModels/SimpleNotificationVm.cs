using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    
    public class SimpleNotificationVm:ReactiveObject, INotification
    {
        public Name Title { get; }
        public Description Content { get; }
        public NotificationId Id { get; } = NotificationId.New();
        public DateTime Timestamp { get; } = DateTime.Now;
        public NotificationType Type { get; }

        internal SimpleNotificationVm(Name title, Description content, NotificationType type)
        {
            Title = title;
            Content = content;
            Type = type;
        }

    }
}
