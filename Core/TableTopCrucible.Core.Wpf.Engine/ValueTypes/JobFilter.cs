using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation.Results;

using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.ValueTypes;
using ValueOf;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class JobFilter : ValueOf<Func<ITrackingViewer, IObservable<bool>>, JobFilter>
    {
        public string Description { get; private set; }
        public static readonly JobFilter All = From(_ => Observable.Return(true), "All Jobs");

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

        protected override void Validate()
        {
            if (Value == null)
                throw new ArgumentNullException(nameof(Value));
        }
    }
}
