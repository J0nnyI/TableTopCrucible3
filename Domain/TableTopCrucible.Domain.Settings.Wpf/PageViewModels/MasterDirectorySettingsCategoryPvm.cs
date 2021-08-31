using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper.Configuration;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using TableTopCtucible.Core.DependencyInjection.Attributes;


namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(MasterDirectorySettingsCategoryPvm))]
    public interface IMasterDirectorySettingsCategoryPage : ISettingsCategoryPage
    {

    }
    public class MasterDirectorySettingsCategoryPvm : IActivatableViewModel, IMasterDirectorySettingsCategoryPage
    {
        public Name Title => Name.From("File Directories");

        public MasterDirectorySettingsCategoryPvm()
        {
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
