using System;

using ValueOf;

namespace TableTopCrucible.Data.Library.ValueTypes.IDs
{
    public abstract class IdBase<T> : ValueOf<Guid, T> where T : IdBase<T>, new()
    {
        public IdBase()
        {

        }
        public static T New()
        {
            return new T()
            {
                Value = new Guid()
            };
        }
    }
}
