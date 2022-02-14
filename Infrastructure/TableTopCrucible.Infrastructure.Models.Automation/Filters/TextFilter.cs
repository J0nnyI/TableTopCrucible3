using System;
using System.Collections.Generic;
using TableTopCrucible.Infrastructure.Models.Automation.Enums;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Filters;

public class TextFilter : FilterBase
{
    public string FilterText { get; set; }
    public TextMatchType TextMatch { get; set; }
    public CaseMatchType CaseMatch { get; set; }

    public TextFilter(string filterText,
        TextMatchType textMatch = TextMatchType.Contains,
        CaseMatchType caseMatch = CaseMatchType.IgnoreCase)
    {
        FilterText = filterText;
        TextMatch = textMatch;
        CaseMatch = caseMatch;
    }

    public override bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item)
    {
        if (string.IsNullOrEmpty(FilterText))
            return true;

        string itemName;
        string filterText;
        switch (CaseMatch)
        {
            case CaseMatchType.RespectCase:
                itemName = item.Value.Name.Value;
                filterText = FilterText;
                break;
            case CaseMatchType.IgnoreCase:
                itemName = item.Value.Name.Value.ToLower();
                filterText = FilterText.ToLower();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return TextMatch switch
        {
            TextMatchType.StartsWith => itemName.StartsWith(filterText),
            TextMatchType.EndsWith => itemName.EndsWith(filterText),
            TextMatchType.Contains => itemName.Contains(filterText),
            TextMatchType.Equals => itemName == filterText,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}