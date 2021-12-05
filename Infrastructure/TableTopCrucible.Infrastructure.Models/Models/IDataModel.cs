using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Models
{
    public interface IDataModel<TId>
        where TId : IDataId
    {
        TId Id { get; }
    }
}