
using TableTopCrucible.Core.Jobs.Enums;

namespace TableTopCrucible.Core.Jobs.Models
{
    public struct ProgressionSnapshot
    {
        public static ProgressionSnapshot Factory(string title, JobState state, int target, int current, string details, int weight)
            => new ProgressionSnapshot(title, state, target, current, details, weight);
        public ProgressionSnapshot(string title, JobState state, int target, int current, string details, int weight)
        {
            Title = title;
            State = state;
            Target = target;
            Current = current;
            Details = details;
            Weight = weight;
        }

        public string Title { get; }
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
