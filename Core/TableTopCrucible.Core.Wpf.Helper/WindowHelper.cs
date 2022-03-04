using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class WindowHelper
    {
        public static Rect GetBounds(this Window window)
            => new(window.Left, window.Top, window.Width, window.Height);
    }
}
