using System;

namespace TableTopCrucible.Core.Database.Exceptions
{
    public class DtoConversionException : Exception
    {
        public DtoConversionException(Type dtoType, Type entityType, Exception innerException) : base("", innerException)
        {
            DtoType = dtoType;
            EntityType = entityType;
        }

        public Type DtoType { get; }
        public Type EntityType { get; }
    }

    public class DtoConversionException<Tentity, Tdto> : DtoConversionException
    {
        public DtoConversionException(Exception innerException) : base(typeof(Tdto), typeof(Tentity), innerException)
        {

        }
    }
}