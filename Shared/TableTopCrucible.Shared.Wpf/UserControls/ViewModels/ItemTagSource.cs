using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public interface ITagSourceProvider
    {
        public ITagCollection Tags { get; }
    }
    public class ItemTagSource:ITagSourceProvider
    {
        public ITagCollection Tags { get; }

        public ItemTagSource(Item item)
        {
            Tags = item.Tags;
        }
    }
}