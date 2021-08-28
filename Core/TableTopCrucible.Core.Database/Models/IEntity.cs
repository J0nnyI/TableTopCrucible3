namespace TableTopCrucible.Core.Database.Models
{
    public interface IEntity<Tid> where Tid : IEntityId
    {
        public Tid Id { get; }
    }
}
