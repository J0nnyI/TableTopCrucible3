using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

[Transient]
public interface ILoadingScreen
{
    public Message Text { get; set; }
}

public class LoadingScreenVm : ReactiveObject, ILoadingScreen
{
    public Message Text { get; set; }
}