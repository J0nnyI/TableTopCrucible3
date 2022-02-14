using TableTopCrucible.Infrastructure.Models.Automation.Actions;
using TableTopCrucible.Infrastructure.Models.Automation.Filters;

namespace TableTopCrucible.Infrastructure.Models.Automation.ActionConfiguration;

internal interface IActionConfiguration
{
    IFilter Filter { get; set; }
    IAction Action { get; set; }
}

public class ActionConfiguration : IActionConfiguration
{
    public IFilter Filter { get; set; }
    public IAction Action { get; set; }
}