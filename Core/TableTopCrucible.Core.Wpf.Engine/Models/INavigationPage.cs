using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    public enum NavigationPageLocation
    {
        Lower,
        Upper
    }

    public interface INavigationPage
    {
        PackIconKind? Icon { get; }
        Name Title { get; }
        NavigationPageLocation PageLocation { get; }
        SortingOrder Position { get; }
    }
}