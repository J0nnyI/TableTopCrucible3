using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TableTopCrucible.Core.DependencyInjection.Exceptions
{
    public class IncompleteServiceException:Exception
    {
        public IEnumerable<ServiceDescriptor> Services { get; }

        private static string getErrorString(IEnumerable<ServiceDescriptor> services)
        {
            return string.Join(Environment.NewLine + "--------------------" + Environment.NewLine,
                services.Select(
                    service => 
                    string.Join(Environment.NewLine,
                        "Lifetime: " + service.Lifetime,
                        "class: " + service.ImplementationType?.Name,
                        "interface: " + service.ImplementationType?.Name
                    )
                )
            );
        }

        public IncompleteServiceException(IEnumerable<ServiceDescriptor> services):base(getErrorString(services))
        {
            Services = services;
        }
    }
}
