﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels
{
    [Singleton(typeof(LibraryPageVm))]
    public interface ILibraryPage
    {

    }
    public class LibraryPageVm:ReactiveObject, IActivatableViewModel, ILibraryPage, INavigationPage
    {
        public ViewModelActivator Activator { get; } = new();
        public PackIconKind? Icon => null;
        public Name Title => Name.From("Item Library");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
        public SortingOrder Position => SortingOrder.From(1);
    }
}
