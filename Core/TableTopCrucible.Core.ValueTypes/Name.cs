using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using TableTopCrucible.Core.ValueTypes.Exceptions;
using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class Name : ValueOf<string, Name>, IComparable<Name>
    {
        public int CompareTo(Name other) => Value?.CompareTo(other?.Value) ?? 0;

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new InvalidNameException("The name must not be empty");
        }

        public static IDisposable RegisterValidator<T>(T vm, Expression<Func<T, string>> propertyName)
            where T : ReactiveObject, IValidatableViewModel
        {
            CompositeDisposable disposables = new();

            vm.ValidationRule(propertyName,
                    value => !string.IsNullOrWhiteSpace(value),
                    "The name must not be empty")
                .DisposeWith(disposables);

            return disposables;
        }

        public static explicit operator Name(string value) => From(value);
        public Name()
        {
            
        }
    }
}