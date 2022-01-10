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
    }
    public class ModelViewerVm : ReactiveObject, IModelViewer, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ModelFilePath Model { get; set; }
        [Reactive]
        public bool IsLoading { get; set; }
        [Reactive]
        public string PlaceholderText { get; set; }
        [Reactive]
        public Model3DGroup ViewportContent { get; set; }

        public Interaction<Unit, Unit> BringIntoView { get; } = new();
        public ModelViewerVm()
        {
            this.WhenActivated(() =>
            {
                var res = new[]
                {
                    this.WhenAnyValue(v => v.Model)
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .Do(model =>
                        {
                            PlaceholderText =
                                model is null
                                    ? "No File selected"
                                    : "Loading";
                            if(model is not null && 
                               model.GetSize().Value > SettingsHelper.FileMinLoadingScreenSize)
                                IsLoading = true;

                            ViewportContent = null;
                        })
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
                            {
                                return;
                            }

                            ViewportContent = model;
                            BringIntoView.Handle(Unit.Default);
                            IsLoading = false;
                        })
                };
                return res;
            });
        }
    }
}
