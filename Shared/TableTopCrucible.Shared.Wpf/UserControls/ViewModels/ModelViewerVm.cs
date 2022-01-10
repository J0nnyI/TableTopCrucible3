using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IModelViewer
    {
        public ModelFilePath Model { get; set; }
        // will be shown instead of the model if set
        public Message PlaceholderMessage { get; set; }
    }
    public class ModelViewerVm : ReactiveObject, IModelViewer, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ModelFilePath Model { get; set; }
        [Reactive]
        public Message PlaceholderMessage { get; set; }
        [Reactive]
        public bool IsLoading { get; set; }
        [Reactive]
        public Message PlaceholderText { get; set; }
        [Reactive]
        public Model3DGroup ViewportContent { get; set; }

        public Interaction<Unit, Unit> BringIntoView { get; } = new();
        public ModelViewerVm()
        {
            this.WhenActivated(() => new[]
                {
                    this.WhenAnyValue(
                            v => v.Model, 
                            v=>v.PlaceholderMessage,
                            (model, message)=>new{model, message})
                        .Do((x) =>
                        {
                            PlaceholderText =
                                x.message ??
                                (x.model is null
                                    ? (Message)"No File selected"
                                    : (Message)"Loading");
                            if(x.model is null || 
                               x.message is not null ||
                               x.model.GetSize().Value > SettingsHelper.FileMinLoadingScreenSize
                               )
                                IsLoading = true;

                            ViewportContent = null;
                        })
                        .Select(x=>x.model)
                        .WhereNotNull()
                        .Select(file =>
                        {
                            return file is null
                                ? Observable.Return(null as Model3DGroup)
                                : Observable.Start(() =>
                                {
                                    var material = Materials.LightGray;
                                    var model = new ModelImporter()
                                    {
                                        DefaultMaterial = material
                                    }.Load(file.Value);
                                    model.PlaceAtOrigin();
                                    model.Freeze();
                                    return model;
                                }, RxApp.TaskpoolScheduler);
                        })
                        .Switch()
                        .Subscribe(model =>
                        {
                            if (model is null)
                                return;

                            RxApp.MainThreadScheduler.Schedule(model, (_, model) =>
                            {
                                ViewportContent = model;
                                BringIntoView.Handle(Unit.Default);
                                IsLoading = false;
                                return null;
                            });
                        })
            });
        }
    }
}
