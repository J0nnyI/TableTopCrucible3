﻿

using System.Collections.Generic;

namespace TableTopCrucible.Core.Database.Models
{
    public interface IEntity<Tid>
        where Tid : IEntityId
    {
        public Tid Id { get; }
    }

    // validation is easier when using native fields instead of changeSets. you need one property per field which you want to edit
    public abstract class EntityBase<Tid>
        : IEntity<Tid>
        where Tid : IEntityId
    {
        public Tid Id { get; }
    }
}
