using System.Linq;
using Ninject;

namespace TableTopCrucible.Backend.Domain.Library
{
    public class Starter
    {
        public void Init()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ILibraryControllerFactory>()
                .To<LibraryControllerFactory>()
                .InThreadScope();

            var srv = kernel.Get<ILibraryControllerFactory>();
            

        }
    }
}