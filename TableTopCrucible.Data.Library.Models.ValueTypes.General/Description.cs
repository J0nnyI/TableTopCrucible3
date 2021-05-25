using System;
using System.Collections.Generic;
using System.Linq;

using TableTopCrucible.Core.Data;

namespace TableTopCrucible.Data.Library.Models.ValueTypes.General
{
    public struct Description
    {
        private string _description { get; }
        public Description(string name)
        {
            var errors = Validate(name);
            if (errors.Any())
                throw new Exception($"could not create description {Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
            _description = name;
        }
        public override string ToString()
            => _description;
        public override bool Equals(object obj)
        {
            return obj switch
            {
                string str => _description == str,
                Description name => _description == name._description,
                _ => false,
            };
        }
        public override int GetHashCode()
            => _description.GetHashCode();
        public static explicit operator Description(string text)
            => new Description(text);
        public static explicit operator string(Description name)
            => name._description;

        public static IEnumerable<string> Validate(string tag)
        {
            return Validators
                .Where(x => !x.IsValid(tag))
                .Select(x => x.Message)
                .ToArray();
        }
        public static IEnumerable<Validator<string>> Validators { get; } = new Validator<string>[] {
            //new Validator<string>(description=>!string.IsNullOrWhiteSpace(description),"The name must not be empty")
        };
    }
}
