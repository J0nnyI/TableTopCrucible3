using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using ValueOf;

namespace TableTopCrucible.Core.Data
{
    /// <summary>
    /// implements <see cref="IValueConverter{, }"/> in both directions, IValueConverter(<typeparamref name="TValue"/>, <typeparamref name="TThis"/>) has to be added manually
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TThis"></typeparam>
    public abstract class MappedValueOf<TValue, TThis>
        :
        ValueOf<TValue, TThis>,
        IValueConverter<TThis, TValue>
        where
        TThis : MappedValueOf<TValue, TThis>,
        new()
    {
        public virtual TThis Convert(TValue sourceMember, ResolutionContext context)
        {
            return new TThis()
            {
                Value = sourceMember
            };
        }
        public virtual TValue Convert(TThis sourceMember, ResolutionContext context)
            => sourceMember.Value;
    }
}
