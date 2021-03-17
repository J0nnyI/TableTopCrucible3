using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Domain.WPF.Library.PageViews;

namespace TableTopCrucible.Domain.WPF.Library.PageViewModels
{
    [Singleton(typeof(LibraryPageVM))]
    public interface ILibraryPage
    {

    }
    [ViewModel(typeof(LibraryPageV))]
    internal class LibraryPageVM : ILibraryPage
    {
    }
}
