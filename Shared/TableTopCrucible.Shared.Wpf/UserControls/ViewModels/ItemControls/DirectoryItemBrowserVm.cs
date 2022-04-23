using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    [Transient]
    public interface IDirectoryItemBrowser
    {

    }
    public class DirectoryItemBrowserVm:IDirectoryItemBrowser
    {
    }
}
