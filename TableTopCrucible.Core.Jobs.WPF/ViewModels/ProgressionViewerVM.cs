using TableTopCrucible.Core.Jobs.Enums;
using TableTopCrucible.Core.Jobs.Managers;

namespace TableTopCrucible.Core.Jobs.WPF.ViewModels
{
    internal class ProgressionViewerVM : ModelWrapperBase<IProgressionViewer>, IProgressionViewer
    {

        public ProgressionViewerVM(IProgressionViewer source) : base(source)
        {
        }

        public JobState State => _source.State;

        public int Target => _source.Target;

        public int Current => _source.Current;

        public string Title => _source.Title;

        public string Details => _source.Details;

        public int Weight => _source.Weight;

    }
}
