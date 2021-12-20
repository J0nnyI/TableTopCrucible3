using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using FluentAssertions;
using NUnit.Framework;

namespace TableTopCrucible.Core.Test.Packages
{
    /// <summary>
    ///     testFixture to check if 3rd party code behaves as expected
    /// </summary>
    [TestFixture]
    internal class DynamicDataTests
    {
        private void CheckCount(SourceList<Tuple<string, IObservable<bool>>> list, int count)
        {
            var filteredList = list
                .Connect()
                .FilterOnObservable(x => x.Item2)
                .AsObservableList();
            var bindedList = new ObservableCollectionExtended<Tuple<string, IObservable<bool>>>();
            filteredList.Connect().Transform(x => new Tuple<string, IObservable<bool>>(x.Item1, x.Item2))
                .Bind(bindedList).Subscribe();
            filteredList.Items.Should().HaveCount(count);
            bindedList.Should().HaveCount(count);
        }

        [Test]
        public void FilterOnObservable()
        {
            var item1 = new Subject<bool>();
            var item2 = new Subject<bool>();
            var item3 = new Subject<bool>();
            var list = new SourceList<Tuple<string, IObservable<bool>>>();
            list.AddRange(new Tuple<string, IObservable<bool>>[]
            {
                new("first", item1.Replay().RefCount()),
                new("second", item2.Replay().RefCount()),
                new("third", item3.Replay().RefCount())
            });
            CheckCount(list, 3);
            CheckCount(list, 3);

            item1.OnNext(false);
            item2.OnNext(true);
            item3.OnNext(false);
            CheckCount(list, 1);


            item1.OnNext(true);
            item2.OnNext(false);
            item3.OnNext(true);
            CheckCount(list, 2);
        }

        [Test]
        public void Bind_Transform_and_FilterOnObservable()
        {
            var count = 3;
            var list = new SourceList<string>();
            var bindedList = new ObservableCollectionExtended<string>();

            list.AddRange(Enumerable.Range(1, count).Select(c => $"item {c}"));

            list.Connect()
                .FilterOnObservable(_ => Observable.Return(true))
                .Transform(str => str)
                .Bind(bindedList)
                .Subscribe(
                    _ => { },
                    ex => Assert.Fail("exception thrown: {0}", ex) // causes the test to fail
                );
        }
    }
}