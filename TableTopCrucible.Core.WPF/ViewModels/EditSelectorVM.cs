using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.WPF.ViewModels
{
    [Transient(typeof(EditSelectorVM))]
    public interface IEditSelector
    {
        bool EditModeEnabled { get; set; }

        void SetCommands(ReactiveCommand<Unit, Unit> revertChanges, ReactiveCommand<Unit, Unit> saveChanges);
    }
    public class EditSelectorVM : ReactiveObject, IEditSelector, IActivatableViewModel
    {
        [Reactive]
        public bool EditModeEnabled { get; set; }
        [Reactive]
        
        public ReactiveCommand<Unit, Unit> EnterEditMode { get; private set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> RevertChanges { get; set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> SaveChanges { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public EditSelectorVM()
        {
            this.WhenActivated(disposables =>
            {
                EnterEditMode = ReactiveCommand.Create(() => { EditModeEnabled = true; })
                    .DisposeWith(disposables);

                //RevertChanges = ReactiveCommand.Create(()=>)
            });
        }

        public void SetCommands(ReactiveCommand<Unit, Unit> revertChanges, ReactiveCommand<Unit, Unit> saveChanges)
        {
            if (revertChanges is null)
                throw new ArgumentNullException(nameof(revertChanges));

            if (saveChanges is null)
                throw new ArgumentNullException(nameof(saveChanges));
            this.RevertChanges = revertChanges;
            this.SaveChanges = saveChanges;

        }
    }
}
