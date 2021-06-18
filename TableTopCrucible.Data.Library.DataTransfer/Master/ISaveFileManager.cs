
using TableTopCrucible.Core.Jobs.Managers;

namespace TableTopCrucible.Data.Library.DataTransfer.Master
{
    public interface ISaveFileManager
    {
        void Validate();
        IProgressionViewer OnFileSaved();
        IProgressionViewer OnFileOpened();
    }
}
