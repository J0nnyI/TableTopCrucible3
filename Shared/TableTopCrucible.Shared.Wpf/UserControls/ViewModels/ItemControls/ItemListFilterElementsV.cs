using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using Accessibility;

using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    public enum TextMatchType
    {
        StartsWith,
        EndsWith,
        Contains,
        Equals
    }

    public enum CaseMatchType
    {
        RespectCase,
        IgnoreCase
    }

    public enum FilterMode
    {
        Include,
        Exclude
    }
    [Transient]
    public interface IItemListFilterElements
    {
        public IObservable<Func<Item, bool>> FilterChanges { get; }
        public FilterMode FilterMode { get; set; }
    }
    public class ItemListFilterElementsV : ReactiveObject, IActivatableViewModel, IItemListFilterElements
    {
        public ITagEditor TagEditor { get; }
        public ViewModelActivator Activator { get; } = new();

        public ItemListFilterElementsV(ITagEditor tagEditor)
        {
            // starts with  FormatLetterSTartsWith      ContainStart
            // ends with    FormatLetterEndsWith        ContainEnd
            // Contains                                 Contain
            // Equals       FormatLetterMatches         Equal
            // Ignore Case
            // Consider Case                            FormatLetterCase
            TagEditor = tagEditor;
            FilterChanges = this.WhenAnyValue(
                    vm => vm.NameTextMatchType,
                    vm => vm.NameCaseMatchType,
                    vm => vm.NameFilter,
                    vm => vm.FilterMode,
                    (textMatchType, caseMatchType, filterText, filterMode) =>
                        new { textMatchType, caseMatchType, filterText, filterMode })
                .CombineLatest(
                    TagCollection.Connect().StartWithEmpty().ToCollection(),
                    (propertyFilter, tags) =>
                    {
                        var nameFilterText = propertyFilter.filterText;
                        if (propertyFilter.caseMatchType == CaseMatchType.IgnoreCase)
                            nameFilterText = nameFilterText?.ToLower() ?? string.Empty;

                        var invertNameResult = propertyFilter.filterMode == FilterMode.Exclude;

                        Func<string, string, bool> nameFilter =
                            string.IsNullOrEmpty(nameFilterText)
                            ? (_, _) => true
                            : propertyFilter.textMatchType switch
                            {
                                TextMatchType.StartsWith => (filter, name) => name.StartsWith(filter) ^ invertNameResult,
                                TextMatchType.EndsWith => (filter, name) => name.EndsWith(filter) ^ invertNameResult,
                                TextMatchType.Contains => (filter, name) => name.Contains(filter) ^ invertNameResult,
                                TextMatchType.Equals => (filter, name) => name== filter ^ invertNameResult,
                                _ => throw new ArgumentOutOfRangeException()
                            };

                        Func<IEnumerable<Tag>, IEnumerable<Tag>, bool> tagFilter
                            = tags.None()
                            ? (_, _) => true
                            : propertyFilter.filterMode switch
                            {
                                FilterMode.Include => (a, b) => a.All(b.Contains),
                                FilterMode.Exclude => (a, b) => a.None(b.Contains),
                                _ => throw new ArgumentOutOfRangeException()
                            };
                        // move as much logic as possible to the upper part since it is executed for each filter change, the func is executed for each item
                        return new Func<Item, bool>(item =>
                        {
                            var itemName = propertyFilter.caseMatchType == CaseMatchType.IgnoreCase
                                ? item.Name.Value.ToLower()
                                : item.Name.Value;

                            return nameFilter(nameFilterText, itemName) && tagFilter(tags, item.Tags.Items);
                        });
                    });
        }

        [Reactive]
        public TextMatchType NameTextMatchType { get; set; } = TextMatchType.Contains;
        [Reactive]
        public CaseMatchType NameCaseMatchType { get; set; } = CaseMatchType.IgnoreCase;
        [Reactive]
        public string NameFilter { get; set; }

        public ITagCollection TagCollection { get; } = new TagCollection();
        [Reactive]
        public FilterMode FilterMode { get; set; } = FilterMode.Include;


        public IObservable<Func<Item, bool>> FilterChanges { get; }
    }
}
