using System;
using System.Collections.Generic;
using System.Text;

using ReactiveUI;

using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    public interface ISettingsCategoryPage
    {
        Name Title { get; }

    }
}
