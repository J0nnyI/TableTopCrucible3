using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.WPF.ViewModels
{
    [Singleton(typeof(EditSelectorVM))]
    public interface IEditSelector
    {

    }
    public class EditSelectorVM : ReactiveObject, IEditSelector
    {
        [Reactive]
        public bool EditModeEnabled { get; set; }
    }
}
