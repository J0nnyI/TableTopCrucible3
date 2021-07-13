namespace TableTopCrucible.Core.DataAccess.Models
{
    public interface IEntityDTO<Tid, Tentity>
        where Tid:IEntityId
        where Tentity : IEntity<Tid>
    {
    }
}
