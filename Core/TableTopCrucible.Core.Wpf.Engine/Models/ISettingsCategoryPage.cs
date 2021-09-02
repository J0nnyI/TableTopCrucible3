using System;
using System.Collections.Generic;
using System.Text;

using ReactiveUI;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    public interface ISettingsCategoryPage
    {
        Name Title { get; }
        SortingOrder  Position { get; }
    }
}
