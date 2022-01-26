#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicData;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.Controller
{
    public interface ITagController : ISourceList<Tag>
    {
        public void Clear();
    }
    internal class TagController : ITagController
    {
        private readonly SourceList<Tag> _tags = new();
        public void Edit(Action<IExtendedList<Tag>> updateAction)
        {
            _tags.Edit(updater =>
            {
                updateAction(updater);
                var distinctTags = updater.Distinct().ToArray();
                for (var i = 0; i < updater.Count - 1; i++)
                {
                    var tag = distinctTags.ElementAtOrDefault(i);
                    if (tag is not null)
                        updater[i] = tag;
                    else
                        updater.RemoveAt(i);
                }

                if (updater.Count < distinctTags.Length)
                    updater.AddRange(distinctTags.Skip(updater.Count - 1));
                else// updater.Count > distinctTags.Length
                    updater.RemoveRange(distinctTags.Length - 1, updater.Count - distinctTags.Length);
            });
        }

        public void Dispose()
            => _tags.Dispose();

        public IObservable<IChangeSet<Tag>> Connect(Func<Tag, bool>? predicate = null)
            => _tags.Connect(predicate);
        public IObservable<IChangeSet<Tag>> Preview(Func<Tag, bool>? predicate = null)
            => _tags.Preview(predicate);
        public int Count
            => _tags.Count;
        public IObservable<int> CountChanged
            => _tags.CountChanged;
        public IEnumerable<Tag> Items
            => _tags.Items;

        public void Clear()
            => _tags.Clear();
    }
    public class TagSourceListJsonConverter : JsonConverter<ITagController>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, ITagController? value, JsonSerializer serializer)
        {
            if (value is null)
                return;

            var token = JToken.FromObject(value.Items.Select(tag => tag.Value));

            if (token.Type != JTokenType.Object)
                token.WriteTo(writer);
            else
            {
                var property = (JProperty)token;
                property.AddRange(value.Items.Select(tag => new JValue(tag.Value)));
                property.WriteTo(writer);
            }
        }

        public override ITagController? ReadJson(JsonReader reader, Type objectType, ITagController? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (objectType != typeof(TagController) && objectType != typeof(ITagController))
                return new TagController();


            var obj = JArray.Load(reader);
            var tags = obj
                .Values<string>()
                .Select(tag => (Tag)tag);

            if (existingValue is null)
            {
                var res = new TagController();
                res.AddRange(tags);
                return res;
            }

            existingValue.Clear();
            existingValue.AddRange(tags);
            return existingValue;
        }
    }
}
