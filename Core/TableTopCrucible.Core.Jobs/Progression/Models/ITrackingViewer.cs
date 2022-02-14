using System;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models;

// interface for getting the progress of a source tracker or tracker collection
public interface ITrackingViewer
{
    IObservable<CurrentProgress> CurrentProgressChanges { get; }
    IObservable<TargetProgress> TargetProgressChanges { get; }
    Name Title { get; }
    IObservable<JobState> JobStateChanges { get; }
}

public interface IWeightedTrackingViewer : ITrackingViewer
{
    JobWeight Weight { get; }
}