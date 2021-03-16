using System;

namespace TableTopCrucible.Core.Data
{
    public interface ITypedId
    {
        Guid ToGuid();
    }
}
