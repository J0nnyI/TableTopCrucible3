
using DynamicData;

using System;

using TableTopCrucible.Core.Jobs.Enums;
using TableTopCrucible.Core.Jobs.Managers;

namespace TableTopCrucible.Core.Jobs.WPF.ViewModels
{
    public class JobViewerVM : ModelWrapperBase<IJobViewer>, IJobViewer
    {

        public JobViewerVM(IJobViewer source) : base(source)
        {
            Progress = _source
                .Progress
                .Connect()
                .Transform(prog => new ProgressionViewerVM(prog) as IProgressionViewer)
                .AsObservableList();
        }

        public IProgressionViewer TotalProgress => _source.TotalProgress;

        public IObservableList<IProgressionViewer> Progress { get; }

        public string Title => _source.Title;

        public IObservable<JobState> StateChanges => _source.StateChanges;

    }
}
