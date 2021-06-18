using AutoMapper;

using System.Collections.Generic;
using System.Linq;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Data.Library.DataTransfer.Services
{
    [Singleton(typeof(MapperService))]
    public interface IMapperService
    {
        Tout Map<Tout>(object input);
        public static IEnumerable<string> RegisteredAssemblies =
            new string[] { "TableTopCrucible.Data.Library.DataTransfer" };
    }
    class MapperService : IMapperService
    {
        private IMapper getMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                IMapperService.RegisteredAssemblies
                    .ToList()
                    .ForEach(lib => cfg.AddMaps(lib));
            }).CreateMapper();

        }
        public Tout Map<Tout>(object input)
        {
            var mapper = getMapper();
            return mapper.Map<Tout>(input);
        }
    }
}
