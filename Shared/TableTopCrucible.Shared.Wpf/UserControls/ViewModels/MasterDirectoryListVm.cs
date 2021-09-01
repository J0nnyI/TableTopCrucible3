using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(MasterDirectoryListVm))]
    public interface IMasterDirectoryList
    {

    }
    public class MasterDirectoryListVm: IMasterDirectoryList
    {
    }
}
