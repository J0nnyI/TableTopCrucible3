using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(NotificationFactory))]
    internal interface INotificationFactory
    {
        
    }
    internal class NotificationFactory:INotificationFactory
    {
        INotification CreateSimple(Name title, Description content)
        {
            return new SimpleNotificationVm(title, content);
        }
    }
}
