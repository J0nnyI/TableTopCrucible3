using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Services;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient(typeof(BannerListVm))]
    public interface IBannerList
    {

    }
    public class BannerListVm:IBannerList
    {
        public BannerListVm(INotificationService notificationService)
        {
            
        }
    }
}
