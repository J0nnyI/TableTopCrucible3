using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32.SafeHandles;
using Image = System.Drawing.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class VisualUtility
    {
        public static BitmapSource CreateBitmap(
            this FrameworkElement elementToRender,
            Size? size = null,
            bool undoTransformation = true)
            => CreateBitmap(elementToRender as Visual,
                size ?? new Size(elementToRender.ActualWidth, elementToRender.ActualHeight),
                undoTransformation);


        public static BitmapSource CreateBitmap(
            this Visual visualToRender,
            Size size,
            bool undoTransformation = true)
        {
            if (visualToRender == null)
                return null;

            // The PixelsPerInch() helper method is used to read the screen DPI setting.
            // If you need to create a bitmap with a specified resolution, you could directly
            // pass the specified dpiX and dpiY values to RenderTargetBitmap constructor.
            RenderTargetBitmap bmp = new(
                (int)Math.Ceiling(size.Width),
                (int)Math.Ceiling(size.Height),
                DeviceHelper.PixelsPerInch(Orientation.Horizontal),
                DeviceHelper.PixelsPerInch(Orientation.Vertical),
                PixelFormats.Pbgra32);

            // If we want to undo the transform, we could use VisualBrush trick.
            if (undoTransformation)
            {
                var dv = new DrawingVisual();
                using (var dc = dv.RenderOpen())
                {
                    var vb = new VisualBrush(visualToRender);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), size));
                }

                bmp.Render(dv);
            }
            else
            {
                bmp.Render(visualToRender);
            }

            return bmp;
        }
    }

    internal class DeviceHelper
    {
        public static int PixelsPerInch(Orientation orientation)
        {
            var capIndex = orientation == Orientation.Horizontal
                ? 0x58
                : 90;
            using (var handle = UnsafeNativeMethods.CreateDC("DISPLAY"))
            {
                return handle.IsInvalid
                    ? 0x60
                    : UnsafeNativeMethods.GetDeviceCaps(handle, capIndex);
            }
        }
    }

    internal sealed class DCSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private DCSafeHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle() => UnsafeNativeMethods.DeleteDC(handle);
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDeviceCaps(DCSafeHandle hDC, int nIndex);

        [DllImport("gdi32.dll", EntryPoint = "CreateDC", CharSet = CharSet.Auto)]
        public static extern DCSafeHandle IntCreateDC(string lpszDriver,
            string lpszDeviceName, string lpszOutput, IntPtr devMode);

        public static DCSafeHandle CreateDC(string lpszDriver) => IntCreateDC(lpszDriver, null, null, IntPtr.Zero);
    }

    public static class VisualRender
    {
        private const uint _SWP_NOSIZE = 0x0001;
        private const uint _SWP_NOMOVE = 0x0002;
        private const uint _SWP_NOACTIVATE = 0x0010;

        private static readonly IntPtr _HwndBottom = new(1);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            uint uFlags);


        public static async Task<BitmapSource> RenderAsync(this UIElement uiElement)
        {
            var brush = new VisualBrush(uiElement);
            var topLeft = uiElement.PointToScreen(new Point(0, 0));
            var bottomRight =
                uiElement.PointToScreen(new Point(uiElement.RenderSize.Width, uiElement.RenderSize.Height));

            var helperWindowLocation = Rect.Empty;
            helperWindowLocation.Union(topLeft);
            helperWindowLocation.Union(bottomRight);

            var width = helperWindowLocation.Width;
            var height = helperWindowLocation.Height;

            //
            var windowsScaleTransform = uiElement.GetWindowsScaleTransform();
            helperWindowLocation.X /= windowsScaleTransform;
            helperWindowLocation.Y /= windowsScaleTransform;
            helperWindowLocation.Width /= windowsScaleTransform;
            helperWindowLocation.Height /= windowsScaleTransform;

            var bitmapSourceT = await RenderAsync(brush, helperWindowLocation, new Size(width, height));
            bitmapSourceT.Freeze();
            return bitmapSourceT;
        }

        private static async Task<BitmapSource> RenderAsync(Brush brush, Rect helperWindowLocation,
            Size snapshotSize)
        {
            // Create a tmp window with its own hwnd to render it later
            var window = new Window
            {
                WindowStyle = WindowStyle.None, ResizeMode = ResizeMode.NoResize, ShowInTaskbar = false,
                Background = Brushes.White
            };
            window.Left = helperWindowLocation.X;
            window.Top = helperWindowLocation.Y;
            window.Width = helperWindowLocation.Width;
            window.Height = helperWindowLocation.Height;
            window.ShowActivated = false;
            var content = new Grid { Background = brush };
            RenderOptions.SetBitmapScalingMode(content, BitmapScalingMode.HighQuality);
            window.Content = content;
            var handle = new WindowInteropHelper(window).EnsureHandle();
            window.Show();
            // Make sure the tmp window is under our mainwindow to hide it
            SetWindowPos(handle, _HwndBottom, 0, 0, 0, 0, _SWP_NOMOVE | _SWP_NOSIZE | _SWP_NOACTIVATE);

            //Wait for the element to render
            //await popupChild.WaitForLoaded();
            await window.WaitForFullyRendered();

            ////Why we have to wait here fore the visualbrush to finish its lazy rendering Process? 
            //// Todo: It seems like very complex uielements does not finish its rendering process within one renderloop
            //// Check https://stackoverflow.com/questions/2851236/rendertargetbitmap-resourced-visualbrush-incomplete-image

            // Async BitBlt the tmp Window
            var bitmapSourceT = await Task.Run(() =>
            {
                Bitmap bitmap = VisualToBitmapConverter.GetBitmap(handle,
                    (int)snapshotSize.Width, (int)snapshotSize.Height);

                var bitmapSource = new SharedBitmapSource(bitmap);
                bitmapSource.Freeze();
                return bitmapSource;
            });
            // Close the Window
            window.Close();

            return bitmapSourceT;
        }

        public static class VisualToBitmapConverter
        {
            [DllImport("gdi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight,
                IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

            public static Bitmap GetBitmap(IntPtr hwnd, int width, int height)
            {
                var bitmap = new Bitmap(width, height);
                using (Graphics graphicsFromVisual = Graphics.FromHwnd(hwnd))
                {
                    using (Graphics graphicsFromImage = Graphics.FromImage(bitmap))
                    {
                        IntPtr source = graphicsFromVisual.GetHdc();
                        IntPtr destination = graphicsFromImage.GetHdc();

                        BitBlt(destination, 0, 0, bitmap.Width, bitmap.Height, source, 0, 0,
                            TernaryRasterOperations.SRCCOPY);

                        graphicsFromVisual.ReleaseHdc(source);
                        graphicsFromImage.ReleaseHdc(destination);
                    }
                }

                return bitmap;
            }

            private enum TernaryRasterOperations : uint
            {
                SRCCOPY = 0x00CC0020,
                SRCPAINT = 0x00EE0086,
                SRCAND = 0x008800C6,
                SRCINVERT = 0x00660046,
                SRCERASE = 0x00440328,
                NOTSRCCOPY = 0x00330008,
                NOTSRCERASE = 0x001100A6,
                MERGECOPY = 0x00C000CA,
                MERGEPAINT = 0x00BB0226,
                PATCOPY = 0x00F00021,
                PATPAINT = 0x00FB0A09,
                PATINVERT = 0x005A0049,
                DSTINVERT = 0x00550009,
                BLACKNESS = 0x00000042,
                WHITENESS = 0x00FF0062,
                CAPTUREBLT = 0x40000000
            }
        }
    }

    internal static class Extensions
    {
        public static Task WaitForLoaded(this FrameworkElement element)
        {
            var tcs = new TaskCompletionSource<object>();
            RoutedEventHandler handler = null;
            handler = (s, e) =>
            {
                element.Loaded -= handler;
                tcs.SetResult(null);
            };
            element.Loaded += handler;
            return tcs.Task;
        }


        public static Task WaitForFullyRendered(this Window window)
        {
            var tcs = new TaskCompletionSource<object>();
            EventHandler handler = null;
            handler = (s, e) =>
            {
                window.ContentRendered -= handler;
                tcs.SetResult(null);
            };
            window.ContentRendered += handler;
            return tcs.Task;
        }

        public static double GetWindowsScaleTransform(this Visual visual)
        {
            var m = Matrix.Identity;
            var presentationSource = PresentationSource.FromVisual(visual);
            if (presentationSource != null)
                if (presentationSource.CompositionTarget != null)
                    m = presentationSource.CompositionTarget.TransformToDevice;

            var totalTransform = m.M11;
            return totalTransform;
        }
    }

    internal class SharedBitmapSource : BitmapSource, IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     I made it public so u can reuse it and get the best our of both namespaces
        /// </summary>
        public Bitmap Bitmap { get; }

        public override double DpiX => Bitmap.HorizontalResolution;

        public override double DpiY => Bitmap.VerticalResolution;

        public override int PixelHeight => Bitmap.Height;

        public override int PixelWidth => Bitmap.Width;

        public override PixelFormat Format => ConvertPixelFormat(Bitmap.PixelFormat);

        public override BitmapPalette Palette => null;

        #endregion

        #region Constructor/Destructor

        public SharedBitmapSource(int width, int height, System.Drawing.Imaging.PixelFormat sourceFormat)
            : this(new Bitmap(width, height, sourceFormat))
        {
        }

        public SharedBitmapSource(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        // Use C# destructor syntax for finalization code.
        ~SharedBitmapSource()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

        #region Overrides

        public override void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            BitmapData sourceData = Bitmap.LockBits(
                new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height),
                ImageLockMode.ReadOnly,
                Bitmap.PixelFormat);

            var length = sourceData.Stride * sourceData.Height;

            if (pixels is byte[])
            {
                var bytes = pixels as byte[];
                Marshal.Copy(sourceData.Scan0, bytes, 0, length);
            }

            Bitmap.UnlockBits(sourceData);
        }

        protected override Freezable CreateInstanceCore() => (Freezable)Activator.CreateInstance(GetType());

        #endregion

        #region Public Methods

        public BitmapSource Resize(int newWidth, int newHeight)
        {
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(Bitmap, 0, 0, newWidth, newHeight);
            }

            return new SharedBitmapSource(newImage as Bitmap);
        }

        public new BitmapSource Clone() => new SharedBitmapSource(new Bitmap(Bitmap));

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected/Private Methods

        private static PixelFormat ConvertPixelFormat(
            System.Drawing.Imaging.PixelFormat sourceFormat)
        {
            switch (sourceFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;

                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormats.Pbgra32;

                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;
            }

            return new PixelFormat();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
            }
        }

        #endregion
    }
}