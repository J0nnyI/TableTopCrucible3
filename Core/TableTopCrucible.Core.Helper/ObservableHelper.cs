using DynamicData.Kernel;

using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace TableTopCrucible.Core.Helper
{
    public struct PreviousAndCurrentValue<T>
    {
        internal PreviousAndCurrentValue(Optional<T> previous, Optional<T> current, ObservableHelper.PCValueType previousType)
        {
            Previous = previous;
            Current = current;
            Type = ObservableHelper.PCValueType.Full;
            if (previousType == ObservableHelper.PCValueType.Seed)
                Type = ObservableHelper.PCValueType.Initial;
            // seed => initial
            // initial => full
            // full => full
        }

        public Optional<T> Previous { get; }
        public Optional<T> Current { get; }
        internal ObservableHelper.PCValueType Type { get; init; }
    }
    public static class ObservableHelper
    {
        internal enum PCValueType : byte
        {
            Seed = 1,
            Initial = 2,
            Full = 3
        }


        // returns the previous and then current value, waiting for the 2nd update to push values
        public static IObservable<PreviousAndCurrentValue<T>> Pairwise<T>(this IObservable<T> src, bool skipInitial = true)
        {
            return src.Scan(new PreviousAndCurrentValue<T> { Type = PCValueType.Seed },
                    (previous, current) =>
                        new PreviousAndCurrentValue<T>(previous.Current, current, previous.Type))
                .Where(x =>
                    x.Type == PCValueType.Full
                    || (!skipInitial && x.Type == PCValueType.Initial));
        }

        public static IObservable<T> OutputObservable<T>(this IObservable<T> src, out IObservable<T> newObservable)
        {
            newObservable = src;
            return src;
        }

        public static IObservable<T> AnimateValue<T>(T seed, Func<T, T> accumulator, TimeSpan? duration = null, IScheduler scheduler = null)
            => Observable.Interval(SettingsHelper.AnimationResolution, scheduler ?? Scheduler.CurrentThread)
                .Take(MathHelper.CeilingInt(
                    (duration ?? SettingsHelper.AnimationDuration) /
                    SettingsHelper.AnimationResolution
                ))
                .Scan(seed, (acc, _) => accumulator(acc));
        public static IObservable<double> AnimateValue(double from, double to, TimeSpan? duration = null, IScheduler scheduler = null)
        {
            var frameCount = (duration ?? SettingsHelper.AnimationDuration) /
                             SettingsHelper.AnimationResolution;
            double stepSize = (to - from) / frameCount;

            return AnimateValue(from, v => v + stepSize, duration, scheduler);
        }
    }
}
