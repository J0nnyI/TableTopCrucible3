using System;
using System.Reactive.Linq;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class JobFilter : ValueType<Func<ITrackingViewer, IObservable<bool>>, JobFilter>
    {
        public static readonly JobFilter All = From(_ => Observable.Return(true), "All Jobs");
        public string Description { get; private set; }

        public static JobFilter FromState(JobState filterState)
            => From(viewer => viewer
                    .JobStateChanges
                    .Select(state => state == filterState),
                "filter by state " + filterState);

        public static JobFilter From(Func<ITrackingViewer, IObservable<bool>> value, string description)
        {
            var res = From(value);
            res.Description = description;
            return res;
        }
    }
}