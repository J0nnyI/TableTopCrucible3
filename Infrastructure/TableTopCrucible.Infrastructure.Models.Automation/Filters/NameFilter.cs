using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.Automation.Filters
{
    public class NameFilter:TextFilter
    {
        public new Name FilterText
        {
            get => (Name)base.FilterText;
            set => base.FilterText = value?.Value;
        }

        public NameFilter(Name filterText,
            TextMatchType textMatch = TextMatchType.Contains,
            CaseMatchType caseMatch = CaseMatchType.IgnoreCase) 
            : base(filterText.Value, textMatch, caseMatch)
        {
        }
    }
}