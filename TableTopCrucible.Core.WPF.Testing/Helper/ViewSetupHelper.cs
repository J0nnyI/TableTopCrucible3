using ReactiveUI;

using Splat;

using System.Linq;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.WPF.Testing.Helper
{
    public static class ViewSetupHelper
    {
        public static void PrepareForTests() =>
        AssemblyHelper
            .GetSolutionAssemblies()
                .ToList()
                .ForEach(Locator.CurrentMutable.RegisterViewsForViewModels);
    }
}
