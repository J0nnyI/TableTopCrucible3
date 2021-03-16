using System.Collections.Generic;

namespace TableTopCrucible.Core.Data
{
    public interface IEntityChangeset<Tentity, Tid>
        where Tentity : struct, IEntity<Tid>
        where Tid : ITypedId
    {
        Tentity? Origin { get; }
        Tentity ToEntity();
        Tentity Apply();
        IEnumerable<string> GetErrors();
    }
}
