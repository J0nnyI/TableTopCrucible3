using System;


namespace TableTopCrucible.Core.Data
{
    public interface IEntity<Tid> : IEntity
        where Tid : ITypedId
    {
        new Tid Id { get; }
        Guid IEntity.Id => Id.ToGuid();
    }

    public interface IEntity
    {
        Guid Id { get; }
        DateTime Created { get; }
        DateTime LastChange { get; }
    }
}
