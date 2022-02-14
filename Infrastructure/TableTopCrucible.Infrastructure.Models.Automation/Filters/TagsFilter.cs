using System;
using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Automation.Enums;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Filters
{
    public class TagsFilter : FilterBase
    {
        public IEnumerable<Tag> Tags { get; set; }
        public ListMatchType MatchType { get; set; }

        public TagsFilter(IEnumerable<Tag> tags, ListMatchType matchType)
        {
            Tags = tags;
            MatchType = matchType;
        }
        public override bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item)
        {
            if ((Tags?.Count() ?? 0) is 0)
                return false;

            return MatchType switch
            {
                ListMatchType.ContainsAll => item.Value.Tags.Items.ContainsAll(Tags),
                ListMatchType.ContainsAny => item.Value.Tags.Items.ContainsAny(Tags),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
