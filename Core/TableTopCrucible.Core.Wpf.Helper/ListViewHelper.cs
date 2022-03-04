using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TableTopCrucible.Core.Wpf.Helper;

public static class ListViewHelper
{
    public static void ScrollIndexIntoView(this ListView list, int index)
    {
        var item = list.Items.GetItemAt(index);
        list.ScrollIntoView(item);
    }
    public static void ScrollToSelection(this ListView list)
        => list.ScrollIntoView(list.SelectedItem);
    public static void SelectLastItem(this ListView list)
    {
        list.SelectedIndex = list.Items.Count - 1;
        list.ScrollToSelection();
    }
    public static void SelectNext(this ListView list)
    {
        if (list.SelectedIndex < list.Items.Count - 1)
        {
            list.SelectedIndex++;
            list.ScrollToSelection();
        }
    }
    public static void SelectPrevious(this ListView list)
    {
        if (list.SelectedIndex == -1)
            list.SelectLastItem();
        if (list.SelectedIndex > 0)
        {
            list.SelectedIndex--;
            list.ScrollToSelection();
        }
    }
}
