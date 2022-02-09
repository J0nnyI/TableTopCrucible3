using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using HelixToolkit.Wpf;

namespace TableTopCrucible.Core.Wpf.Helper
{
    // https://stackoverflow.com/questions/1070685/hiding-the-arrows-for-the-wpf-expander-control
    public class ExpanderAssist
    {
        #region HideExpanderArrow AttachedProperty

        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static bool GetHideExpanderArrow(DependencyObject obj)
        {
            return (bool)obj.GetValue(HideExpanderArrowProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHideExpanderArrow(DependencyObject obj, bool value)
        {
            obj.SetValue(HideExpanderArrowProperty, value);
        }

        // Using a DependencyProperty as the backing store for HideExpanderArrow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideExpanderArrowProperty =
            DependencyProperty.RegisterAttached("HideExpanderArrow", typeof(bool), typeof(AttachedProperties), new UIPropertyMetadata(false, OnHideExpanderArrowChanged));

        private static void OnHideExpanderArrowChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Expander expander = (Expander)o;

            if (expander.IsLoaded)
            {
                UpdateExpanderArrow(expander, (bool)e.NewValue);
            }
            else
            {
                expander.Loaded += new RoutedEventHandler((x, y) => UpdateExpanderArrow(expander, (bool)e.NewValue));
            }
        }

        private static void UpdateExpanderArrow(Expander expander, bool visible)
        {
            if (VisualTreeHelper.GetChild(expander, 0) is not Border border
                || VisualTreeHelper.GetChild(border, 0) is not DockPanel dock
                || VisualTreeHelper.GetChild(dock, 0) is not ToggleButton headerButton
                || VisualTreeHelper.GetChild(headerButton, 0) is not Border headerBorder
                || VisualTreeHelper.GetChild(headerBorder, 0) is not Grid headerGrid)
                throw new ArgumentException("incompatible template");

            if (VisualTreeHelper.GetChild(headerGrid, 0) is ContentPresenter
                && VisualTreeHelper.GetChild(headerGrid, 1) is ToggleButton arrowButton)
                // Material Design Expander Template
                arrowButton.Visibility = Visibility.Collapsed;
            else
            {
                // default expander template
                headerGrid.Children[0].Visibility = visible
                    ? Visibility.Collapsed
                    : Visibility.Visible; // Hide or show the Ellipse
                headerGrid.Children[1].Visibility = visible
                    ? Visibility.Collapsed
                    : Visibility.Visible; // Hide or show the Arrow
                headerGrid.Children[2].SetValue(Grid.ColumnProperty, visible
                    ? 0
                    : 1); // If the Arrow is not visible, then shift the Header Content to the first column.
                headerGrid.Children[2].SetValue(Grid.ColumnSpanProperty, visible
                    ? 2
                    : 1); // If the Arrow is not visible, then set the Header Content to span both rows.
                headerGrid.Children[2].SetValue(ContentPresenter.MarginProperty, visible
                    ? new Thickness(0)
                    : new Thickness(4, 0, 0,
                        0)); // If the Arrow is not visible, then remove the margin from the Content.
            }
        }

        #endregion
    }
}
