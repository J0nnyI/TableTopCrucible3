using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.Database.Models;

namespace TableTopCrucible.Core.Database
{
    [DataContract]
    public class TableDto<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto:IEntityDto<Tid, Tentity>
    {
        public Tdto[] Data { get; set; }
    }
}
