using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Infrastructure.Views.Services;

[Singleton]
public interface ITagView
{
    public IObservableList<Tag> Data { get; }
}

internal class TagView : ITagView
{
    public TagView(IItemRepository itemRepository)
    {
        this.Data = itemRepository
            .Data
            .Connect()
            .RemoveKey()
            .TransformMany(item => item.Tags)
            .DistinctValues(tag => tag)
            .AsObservableList();
    }

    public IObservableList<Tag> Data { get; }
}