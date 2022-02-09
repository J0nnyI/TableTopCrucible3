using System;
using System.Drawing.Text;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using DynamicData.Kernel;

using MaterialDesignThemes.Wpf;

using Microsoft.AspNetCore.Components;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{

    public class QueueItem:ReactiveObject, IDisposable
    {
        private CompositeDisposable _disposables = new();
        public void Dispose() => _disposables.Dispose();
        public Func<object> Action { get;  }
        public IObservable<Unit> Cancel { get;  }
        private BehaviorSubject<Optional<object>> _resultChanges = new(Optional<object>.None);
        [ObservableAsProperty]
        public bool Canceled { get; }
        public IObservable<object> Result => _resultChanges
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .TakeUntil(Cancel)
            .Take(1);

        public QueueItem(Func<object> action, IObservable<Unit> cancel)
        {
            Action = action;
            Cancel = cancel;
            cancel.Select(_ => true)
                .ToPropertyEx(this, x => x.Canceled, false, false)
                .DisposeWith(_disposables);
        }

        public void PushResult(object obj)
            => _resultChanges.OnNext(obj);
    }
    public class LoadingQueue
    {
        private readonly object _lock = new();
        private readonly ReplaySubject<QueueItem> _queue = new();
        public IObservable<T> Add<T>(Func<T> action, IObservable<Unit> cancel)
        {
            var item = new QueueItem(() => action(), cancel );
            lock (_lock)
                _queue.OnNext(item);

            return item.Result.Select(res=>(T)res);
        }

        public LoadingQueue()
        {
            Subject<Unit> throttle = new();
            _queue
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Zip(throttle,(item,_)=>item)
                .Subscribe(
                item =>
                {
                    item.PushResult(item.Action());
                    throttle.OnNext();
                });
            // one for each parallel process
            throttle.OnNext();
            throttle.OnNext();
            throttle.OnNext();
        }
    }


    [Transient]
    public interface IImageViewer
    {
        ImageFilePath ImageFile { get; set; }
        RenderSize RenderSize { get; set; }
    }

    public class ImageViewerVm : ReactiveObject, IActivatableViewModel, IImageViewer
    {
        private static readonly LoadingQueue _queue = new();
        public ImageViewerVm()
        {
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm.ImageFile)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(file =>
                    {
                        if (file is null || !file.Exists())
                        {
                            return Observable.Return(
                            new
                            {
                                imageSource = null as BitmapImage,
                                errorText = (Message)"No file selected",
                                placeholderIcon = PackIconKind.ImageOutline
                            });
                        }

                        return _queue.Add(() =>
                        {
                            try
                            {
                                using var stream = file.OpenRead();
                                var src = new BitmapImage();
                                src.BeginInit();
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.StreamSource = stream;
                                src.EndInit();
                                src.DecodePixelWidth = Convert.ToInt32(RenderSize?.Value ?? 0);
                                src.Freeze();

                                return new
                                {
                                    imageSource = src,
                                    errorText = null as Message,
                                    placeholderIcon = PackIconKind.ImageOffOutline,
                                };
                            }
                            catch (Exception e)
                            {
                                return new
                                {
                                    imageSource = null as BitmapImage,
                                    errorText = (Message)"File could not be loaded",
                                    placeholderIcon = PackIconKind.ImageOffOutline,
                                };
                            }
                        },this.Activator.Deactivated)
                            .StartWith(new
                            {
                                imageSource = null as BitmapImage,
                                errorText = (Message)"loading",
                                placeholderIcon = PackIconKind.ImageOffOutline,
                            });
                    })
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x =>
                    {
                        ImageSource = x.imageSource;
                        ErrorText = x.errorText;
                        PlaceholderIcon = x.placeholderIcon;
                    })
            });
        }

        [Reactive]
        public ImageSource ImageSource { get; private set; }

        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ImageFilePath ImageFile { get; set; }

        public RenderSize RenderSize { get; set; }

        [Reactive]
        public Message ErrorText { get; set; }

        [Reactive]
        public PackIconKind PlaceholderIcon { get; set; }
    }
}