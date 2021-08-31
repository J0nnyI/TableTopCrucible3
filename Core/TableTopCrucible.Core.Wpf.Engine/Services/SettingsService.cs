using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DynamicData;
using Splat;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(SettingsService))]
    public interface ISettingsService
    {
        void RegisterPage(ISettingsCategoryPage categoryPage);
    }
    internal class SettingsService:ISettingsService
    {
        private SourceCache<ISettingsCategoryPage, Name> pages = new SourceCache<ISettingsCategoryPage, Name>(page => page.Title);
        public void RegisterPage(ISettingsCategoryPage categoryPage)
        {
            throw new NotImplementedException();
        }

        public SettingsService()
        {
            this.pages
                .AddOrUpdate(
                    AssemblyHelper.GetSolutionTypes()
                        !.Where(t => t.IsAssignableFrom(typeof(ISettingsCategoryPage)))
                        !.Select(t=>Locator.Current.GetService(t) as ISettingsCategoryPage)
                        !.ToArray());

        }
    }
}
