using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(BannerService))]
    public interface IBannerService
    {

    }
    class BannerService
    {
    }
}
