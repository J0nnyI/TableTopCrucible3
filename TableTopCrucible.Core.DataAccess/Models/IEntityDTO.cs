
using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.FileManagement.Models
{
    public interface IEntityDTO<Tid, Tentity>
        where Tid:IEntityId
        where Tentity : IEntity<Tid>
    {
    }
}
