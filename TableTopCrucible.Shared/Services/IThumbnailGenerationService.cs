using System;
using System.Collections.Generic;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Models;
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
        public ImageFilePath Generate(Item item, CameraView view = null);

        public ImageFilePath GenerateWithAutoPosition(Item item);

        public ITrackingViewer GenerateManyAsync(IEnumerable<Item> items, ThreadCount parallelThreads = null);
        public void GenerateManyAsync(IObservable<FileData> source, ISourceTracker tracker, ThreadCount parallelThreads = null);
    }
}