using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Domain.Library.Services;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemModelViewer
    {
        public Item Item { get; set; }
        ICommand GenerateThumbnailCommand { get; }
    }

    public class ItemModelViewerVm : ReactiveObject, IItemModelViewer, IActivatableViewModel
    {
        public ItemModelViewerVm(
            IModelViewer modelViewer,
            IFileRepository fileRepository,
            IGalleryService galleryService,
            INotificationService notificationService,
            IWpfThumbnailGenerationService thumbnailGenerationService)
        {
            ModelViewer = modelViewer;
            GenerateThumbnailCommand = ReactiveCommand.Create(() =>
                {
                    try
                    {
                        if (Item is null)
                            throw new InvalidOperationException("the item must be selected to create an thumbnail");
                        var imgPath = modelViewer.IsActivated
                            ? modelViewer.GenerateThumbnail(Item.Name)
                            : thumbnailGenerationService.GenerateAutoPositionThumbnail(Item);

                        galleryService.AddImagesToItem(Item, imgPath);

                        notificationService.AddNotification(
                            (Name)"Thumbnail Generated",
                            (Description)
                            $"The Thumbnail for item {Item.Name} has been generated and was saved at {imgPath}",
                            NotificationType.Confirmation);
                    }
                    catch (Exception e)
                    {
                        notificationService.AddNotification(
                            (Name)"Thumbnail generation failed",
                            (Description)$"The thumbnail could not be generated: {Environment.NewLine}{e}",
                            NotificationType.Error);
                        Debugger.Break();
                        throw;
                    }
                },
                this.WhenAnyValue(vm => vm.Item)
                    .Select(item => fileRepository.WatchSingle(item?.FileKey3d))
                    .Switch()
                    .Select(file => file is not null)
                    .DistinctUntilChanged()
                , RxApp.MainThreadScheduler, RxApp.MainThreadScheduler);

            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm.Item.FileKey3d)
                    .Select(item =>
                        fileRepository
                            .Watch(item)
                            .ToCollection()
                            .Select(files =>
                                new
                                {
                                    item,
                                    files
                                }))
                    .Switch()
                    .OutputObservable(out var filesChanges)
                    .Select(x => x.files.FirstOrDefault()?.Path)
                    .Select(ModelFilePath.From)
                    .Catch((Exception e) =>
                    {
                        Debugger.Break();
                        return Observable.Never<ModelFilePath>();
                    })
                    .BindTo(this, vm => vm.ModelViewer.Model),

                filesChanges
                    .Select(x => x.item is null
                        ? (Message)"No Item selected"
                        : x.files.Any()
                            ? null
                            : (Message)"No Model found for this Item")
                    .BindTo(this, vm => vm.ModelViewer.PlaceholderMessage)
            });
        }

        public IModelViewer ModelViewer { get; }
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public Item Item { get; set; }


        public ICommand GenerateThumbnailCommand { get; }
    }
}