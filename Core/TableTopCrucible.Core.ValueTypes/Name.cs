﻿using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using TableTopCrucible.Core.ValueTypes.Exceptions;
using ValueOf;

using static TableTopCrucible.Core.Helper.FileSystemHelper;

namespace TableTopCrucible.Core.ValueTypes
{
    public class Name : ValueOf<string, Name>
    {
        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new InvalidNameException("The name must not be empty");
        }
        public static IDisposable RegisterValidator<T>(T vm, Expression<Func<T, string>> propertyName, bool includeExists = true) where T : ReactiveObject, IValidatableViewModel
        {
            CompositeDisposable disposables = new();

            vm.ValidationRule(propertyName, 
                    value => !string.IsNullOrWhiteSpace(value),
                    "The name must not be empty")
                .DisposeWith(disposables);

            return disposables;
        }
    }
}
