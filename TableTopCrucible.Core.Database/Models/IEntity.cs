using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.FileManagement.Models
{
    public interface IEntity<Tid> where Tid : IEntityId
    {
        public Tid Id { get; }
    }
}
