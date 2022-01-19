using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ninject;

namespace TableTopCrucible.Backend.Domain.Library
{
    public interface ILibraryControllerFactory
    {
        public Task<TestItem> GetData();

    }
    public class LibraryControllerFactory: ILibraryControllerFactory
    {
        private readonly StandardKernel _diKernel;

        public LibraryControllerFactory(StandardKernel diKernel)
        {
            _diKernel = diKernel;
        }
        public Task<TestItem> GetData()
        {
            var scope = _diKernel.CreateScope();
            var srv = scope.ServiceProvider.GetRequiredService<ILibraryController>();
            var task = Task.Run(srv.getItem);

            task.Start();
            return task;
        }
    }
}