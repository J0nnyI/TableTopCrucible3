using MaterialDesignThemes.Wpf;

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
        public PackIconKind? Icon { get; }
        public Name Title { get; }
        public NavigationPageLocation PageLocation { get; }
        public SortingOrder Position { get; }
    }

}
