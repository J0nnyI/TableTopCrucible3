using TableTopCrucible.Shared.Wpf.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Wpf.Filters.Operators;

namespace TableTopCrucible.Shared.Wpf.Filters.Tests
{
    [TestFixture]
    public class ItemNameFilterTests
    {
        [Test]
        public void ApplyTest()
        {
            var includeName = (Name)"test";
            var includeTags = new[] { (Tag)"include 1", (Tag)"include 2"};
            var excludeName = (Name)"not";
            var excludeTags = new[] { (Tag)"exclude 1", (Tag)"exclude 2" };


            IEnumerable<Item> testItems = new Item[]
            {
                new(includeName,new FileHashKey(), includeTags),
                new(includeName,new FileHashKey(), includeTags),


                //exclude
                new(includeName,new FileHashKey(), includeTags.First().AsArray()),
                new(includeName,new FileHashKey(), includeTags.Last().AsArray()),

                new(excludeName,new FileHashKey(), includeTags),
                new(includeName,new FileHashKey(), includeTags),
                new(includeName,new FileHashKey(), includeTags),
                new(includeName,new FileHashKey(), includeTags),
                new(includeName,new FileHashKey(), includeTags),
            }


            var filter = new AndOperator(
                // whitelist
             new NameFilter(includeName),
                new TagsFilter(includeTags, ListMatchType.ContainsAll),
                //blacklist
                new NotOperator(
                    new OrOperator(
                        new NameFilter(excludeName),
                        new TagsFilter(excludeTags,ListMatchType.ContainsAny)
                    )
                )
            );

        }
    }
}