using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.ValueTypes;

namespace TableTopCrucible.Shared.Services
{
    [Singleton]
    public interface IThumbnailGenerationService
    {
        /// <summary>
        ///     generates a thumbnail for the item and adds it to its gallery
        /// </summary>
        /// <param name="item"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public ImageFilePath GenerateThumbnail(Item item, CameraView view = null);

        public ImageFilePath GenerateAutoPositionThumbnail(Item item);
    }
}