using System;
using System.Collections.Generic;
using System.Linq;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Jobs.Enums
{
    public enum JobState : byte
    {
        ToDo = 01,
        InProgress = 02,
        Done = 10,
        Failed = 11,
        Canceled = 12
        // invalid = 255
    }
    public static class JobStateHelper
    {
        public static JobState MergeStates(IEnumerable<JobState> states)
        {
            return states.Aggregate(JobState.Done, (aggregation, current) =>
            {
                if (JobState.Failed.IsIn(aggregation, current))
                    return JobState.Failed;
                if (JobState.Canceled.IsIn(aggregation, current))
                    return JobState.Canceled;
                if (JobState.InProgress.IsIn(aggregation, current))
                    return JobState.InProgress;
                if (JobState.ToDo.IsIn(aggregation, current))
                    return JobState.ToDo;
                if (JobState.Done.IsIn(aggregation, current))
                    return JobState.Done;

                throw new InvalidOperationException();
            });
        }
    }
}
