using System;
using System.Linq;
using DynamicData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class Tag : ValueType<string, Tag>
    {
        protected override void Validate(string value)
        {
            base.Validate(value);
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidValueException("a tag must not be empty");
        }
    }

    public class TagSourceListConverter : JsonConverter<SourceList<Tag>>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, SourceList<Tag>? value, JsonSerializer serializer)
        {
            if (value is null)
                return;

            var token = JToken.FromObject(value.Items.Select(tag=>tag.Value));

            if (token.Type != JTokenType.Object)
                token.WriteTo(writer);
            else
            {
                var property = (JProperty)token;
                property.AddRange(value.Items.Select(tag => new JValue(tag.Value)));
                property.WriteTo(writer);
            }
        }

        public override SourceList<Tag>? ReadJson(JsonReader reader, Type objectType, SourceList<Tag>? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (objectType != typeof(SourceList<Tag>))
                return new();


            var obj = JArray.Load(reader);
            var tags = obj
                .Values<string>()
                .Select(tag => (Tag)tag);

            if (existingValue is null)
            {
                var res = new SourceList<Tag>();
                res.AddRange(tags);
                return res;
            }

            existingValue.Clear();
            existingValue.AddRange(tags);
            return existingValue;
        }
    }
}