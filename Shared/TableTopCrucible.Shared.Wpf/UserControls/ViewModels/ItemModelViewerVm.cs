using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemModelViewer
    {
        public Item Item{get; set; }
    }
    public class ItemModelViewerVm:ReactiveObject,IActivatableViewModel, IItemModelViewer
    {
        public ViewModelActivator Activator { get; } = new();
        [Reactive]
        public Item Item { get; set; }
        public IModelViewer ModelViewer { get; }

        public ItemModelViewerVm(IModelViewer modelViewer, IFileRepository fileRepository)
        {
            ModelViewer = modelViewer;
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm=>vm.Item.FileKey3d)
                    .Select(fileRepository.Watch)
                    .Switch()
                    .Select(files=>files.FirstOrDefault()?.Path)
                    .Select(ModelFilePath.From)
                    .Catch((Exception e) =>
                    {
                        return Observable.Never<ModelFilePath>();
                    })
                    .BindTo(this, vm=>vm.ModelViewer.Model)
            });
        }

    }
}
