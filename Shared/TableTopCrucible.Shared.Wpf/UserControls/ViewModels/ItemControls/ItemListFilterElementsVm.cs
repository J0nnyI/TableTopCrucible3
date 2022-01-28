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
    public class ItemListFilterElementsVm : ReactiveObject, IActivatableViewModel, IItemListFilterElements
    {
        public ITagEditor TagEditor { get; }
        public ViewModelActivator Activator { get; } = new();

        public ItemListFilterElementsVm(ITagEditor tagEditor)
        {
            // starts with  FormatLetterSTartsWith      ContainStart
            // ends with    FormatLetterEndsWith        ContainEnd
            // Contains                                 Contain
            // Equals       FormatLetterMatches         Equal
            // Ignore Case
            // Consider Case                            FormatLetterCase
            TagEditor = tagEditor;
            tagEditor.TagSource = TagCollection;
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

                        Func<string, bool> nameFilter =
                            string.IsNullOrEmpty(nameFilterText)
                            ? ( _) => true
                            : propertyFilter.textMatchType switch
                            {
                                TextMatchType.StartsWith =>  name => name.StartsWith(nameFilterText) ^ invertNameResult,
                                TextMatchType.EndsWith =>  name => name.EndsWith(nameFilterText) ^ invertNameResult,
                                TextMatchType.Contains =>  name => name.Contains(nameFilterText) ^ invertNameResult,
                                TextMatchType.Equals =>  name => name== nameFilterText ^ invertNameResult,
                                _ => throw new ArgumentOutOfRangeException()
                            };

                        Func<IEnumerable<Tag>, bool> tagFilter
                            = tags.None()
                            ? _ => true
                            : propertyFilter.filterMode switch
                            {
                                FilterMode.Include => b => tags.All(b.Contains),
                                FilterMode.Exclude => b => tags.None(b.Contains),
                                _ => throw new ArgumentOutOfRangeException()
                            };
                        // move as much logic as possible to the upper part since it is executed for each filter change, the func is executed for each item
                        return new Func<Item, bool>(item =>
                        {
                            var itemName = propertyFilter.caseMatchType == CaseMatchType.IgnoreCase
                                ? item.Name.Value.ToLower()
                                : item.Name.Value;

                            return nameFilter(itemName) && tagFilter(item.Tags.Items);
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
