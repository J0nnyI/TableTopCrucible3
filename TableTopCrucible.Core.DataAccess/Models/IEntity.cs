namespace TableTopCrucible.Core.DataAccess.Models
{
    public interface IEntity<Tid> where Tid : IEntityId
    {
        public Tid Id { get; }
    }
}
