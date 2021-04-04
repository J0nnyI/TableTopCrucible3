using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Enums;

namespace TableTopCrucible.Core.Jobs.Managers
{
    class JobManager : IJobHandler
    {
        ISourceList<IProgressionViewer> _progress { get; } = new SourceList<IProgressionViewer>();
        public IObservable<JobState> StateChanges { get; }

        public IObservableList<IProgressionViewer> Progress => _progress.AsObservableList();

        [Reactive]
        public string Title { get; set; }
        public IProgressionViewer TotalProgress { get; }

        public JobManager()
        {
            this.TotalProgress = new DerivedProgressionManager(Progress);

            this.StateChanges = Progress
                .Connect()
                .Transform(prog => prog.WhenAnyValue(vm => vm.State))
                .ToCollection()
                .Select(Observable.CombineLatest)
                .Switch()
                .Select(JobStateHelper.MergeStates);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IProgressionHandler TrackProgression(string details, int target, int weight)
        {
            var prog = new ProgressionManager(details, target, weight);
            this._progress.Add(prog);
            return prog;
        }
    }

    public interface IJobHandler : IJobViewer
    {
        new string Title { get; set; }
        IProgressionHandler TrackProgression(string details, int target, int weight);
    }

    public interface IJobViewer : INotifyPropertyChanged
    {
        IProgressionViewer TotalProgress { get; }
        IObservableList<IProgressionViewer> Progress { get; }
        string Title { get; }
        public IObservable<JobState> StateChanges { get; }
    }
}
