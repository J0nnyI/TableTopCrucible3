using System.ComponentModel;

using MaterialDesignThemes.Wpf;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    public interface ITabPage : INotifyPropertyChanged
    {
        public Name Title { get; }
        public PackIconKind SelectedIcon { get; }
        public PackIconKind UnselectedIcon { get; }
        public bool IsSelectable { get; }
        SortingOrder Position { get; }
        //executed when the user switches from this tab to another
        public void InitiatingClose();
    }
}