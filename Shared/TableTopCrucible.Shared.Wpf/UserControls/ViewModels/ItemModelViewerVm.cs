using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
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
                    .Select(item=>
                        fileRepository
                            .Watch(item)
                            .Select(files =>
                            new {
                                item,
                                files
                            }))
                    .Switch()
                    .Publish()
                    .RefCount()
                    .OutputObservable(out var filesChanges)
                    .Select(x=>x.files.FirstOrDefault()?.Path)
                    .Select(ModelFilePath.From)
                    .Catch((Exception e) =>
                    {
                        Debugger.Break();
                        return Observable.Never<ModelFilePath>();
                    })
                    .BindTo(this, vm=>vm.ModelViewer.Model),

                filesChanges
                    .Select(x=>x.item is null
                        ? (Message)"No Item selected"
                        : x.files.Any()
                            ? null
                            : (Message)"No Model found for this Item")
                    .BindTo(this, vm=>vm.ModelViewer.PlaceholderMessage)
            });
        }

    }
}
