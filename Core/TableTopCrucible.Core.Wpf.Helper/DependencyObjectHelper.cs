using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class DependencyObjectHelper
    {
        public static Window GetWindow(this DependencyObject obj)
            => Window.GetWindow(obj);
        public class VisualTreeHierarchyEntry
        {
            [JsonInclude]
            public string Type => Reference.GetType().Name;
            [JsonIgnore]
            public DependencyObject Reference { get; }
            [JsonInclude]
            public IEnumerable<VisualTreeHierarchyEntry> Children =>
                Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(Reference))
                    .Select(i => new VisualTreeHierarchyEntry(VisualTreeHelper.GetChild(Reference, i)))
                    .Concat(new[]
                        {
                            (Reference as Popup )?.Child
                        }
                        .Where(x=>x is not null)
                        .Select(x => new VisualTreeHierarchyEntry(x))
                    )
                    .ToArray();
            public VisualTreeHierarchyEntry(DependencyObject reference)
            {
                Reference = reference;
            }
            public override string ToString()
                => Reference.GetType().Name;

            private string printTree(string indent = "-")
            {
                return string.Join(Environment.NewLine,
                    Children
                        .Select(x => x.printTree("|" + indent))
                        .Prepend(indent + ToString())
                    );
            }
            public string PrintTree()
                => printTree();

            public VisualTreeHierarchyEntry this[int index]
                => Children.ElementAt(index);
            public VisualTreeHierarchyEntry getChild<T>()
                => Children.FirstOrDefault(c => c.Reference is T);

            [JsonIgnore]
            public IEnumerable<VisualTreeHierarchyEntry> Entries
                => Children.SelectMany(x => x.Entries).Prepend(this);

            public IEnumerable<VisualTreeHierarchyEntry> getChildrenRecusively<T>()
                => this
                    .Entries
                    .Where(x => x.Reference is T)
                    .ToArray();
            public VisualTreeHierarchyEntry getChildRecusively<T>()
                => this
                    .Entries
                    .SingleOrDefault(x => x.Reference is T);

            public Hierarchy ToDictionary()
            {
                return new Hierarchy(
                    this.Children
                        .Select((control, index) => new {control,index})
                        .ToDictionary(
                            x => $"{x.index} - {x.control.Type}", 
                            x => x.control.ToDictionary()));
            }
            public class Hierarchy:Dictionary<string, Hierarchy>
            {
                public Hierarchy(Dictionary<string, Hierarchy> items):base(items)
                {

                }
            }
        }
        public static string PrintChildren(this DependencyObject obj)
            => obj.GetChildren().PrintTree();
        public static string PrintChildrenJson(this DependencyObject obj)
            => JsonSerializer.Serialize(obj.GetChildren().ToDictionary());
        public static VisualTreeHierarchyEntry GetChildren(this DependencyObject obj)
        {
            if (obj is null)
                throw new NullReferenceException(nameof(obj));

            return new VisualTreeHierarchyEntry(obj);
        }
    }
}
