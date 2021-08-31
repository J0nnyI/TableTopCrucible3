using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper.Configuration;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using TableTopCtucible.Core.DependencyInjection.Attributes;

using NameVt = TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes.Name;
namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(MasterDirectorySettingsCategoryPvm))]
    public interface IMasterDirectorySettingsCategoryPage : ISettingsCategoryPage
    {

    }
    public class MasterDirectorySettingsCategoryPvm : ViewModelViewHost, IMasterDirectorySettingsCategoryPage
    {
        public NameVt Title
        {
            get =>NameVt.From(Name);
            private set => Name = value.Value;
        }

        public MasterDirectorySettingsCategoryPvm()
        {
            this.Title = NameVt.From("Master Directories");
        }
    }
}
