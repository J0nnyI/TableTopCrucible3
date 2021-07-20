namespace TableTopCrucible.Core.DataAccess.Models
{
    public interface IEntityDto<Tid, Tentity>
        where Tid:IEntityId
        where Tentity : IEntity<Tid>
    {
    }
}
