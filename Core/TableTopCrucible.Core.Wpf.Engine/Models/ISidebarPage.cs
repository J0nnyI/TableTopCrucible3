using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    /// <summary>
    ///     Viewmodel for a Page which can be shown at the right side of the screen
    /// </summary>
    public interface ISidebarPage
    {
        Name Title { get; }
        SidebarWidth Width { get; }
    }
}