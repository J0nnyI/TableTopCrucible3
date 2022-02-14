using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Services;

namespace TableTopCrucible.Starter;

/// <summary>
///     a Service which is used to generate test data on startup
/// </summary>
[Singleton]
public interface ITestingService
{
    void CreateData();
}

internal class TestingService : ITestingService
{
    private readonly IProgressTrackingService _progressTrackingService;

    public TestingService(IProgressTrackingService progressTrackingService)
    {
        _progressTrackingService = progressTrackingService;
    }

    public void CreateData()
    {
    }
}