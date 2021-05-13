using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.WPF.Navigation.Models
{
    public interface INavigationPage : IActivatableViewModel
    {
    }
    public interface INavigationPage<T> : INavigationPage
    {
    }
}
