using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.Jobs.Enums;

namespace TableTopCrucible.Core.Jobs.Models
{
    public struct ProgressionSnapshot
    {
        public static ProgressionSnapshot Factory(JobState state, int target, int current, string details, int weight)
            => new ProgressionSnapshot(state, target, current, details, weight);
        public ProgressionSnapshot(JobState state, int target, int current, string details, int weight)
        {
            State = state;
            Target = target;
            Current = current;
            Details = details;
            Weight = weight;
        }

        public JobState State { get; }
        public int Target { get; }
        public int Current { get; }
        public string Details { get; }
        /// <summary>
        /// Not supported in derived progressuib managers
        /// </summary>
        public int Weight { get; }
    }
}
