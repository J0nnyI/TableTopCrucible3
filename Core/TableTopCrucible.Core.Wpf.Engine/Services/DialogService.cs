﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(DialogService))]
    public interface IDialogService
    {

        IYesNoDialog OpenYesNoDialog(string text);
    }
    public class DialogService : IDialogService
    {
        public IYesNoDialog OpenYesNoDialog(string text)
        {
            var dialog = new YesNoDialogVm(text);
            DialogHost.Show(new ViewModelViewHost(){ViewModel = dialog},"List",
                (sender, args) =>
                {
                    dialog.Session = args.Session;
                },
                (sender, args) =>
                {
                    dialog.Close();
                });
            return dialog;
        }
    }
}
