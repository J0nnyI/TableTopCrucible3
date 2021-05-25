using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.Jobs.Managers;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    public interface ISaveFileManager
    {
        void Validate();
        IProgressionViewer OnFileSaved();
        IProgressionViewer OnFileOpened();
    }
}
