using System;

namespace TableTopCrucible.Core.Data
{
    public struct Validator<T>
    {
        public Func<T, bool> IsValid { get; }
        public string Message { get; }

        public Validator(Func<T, bool> isValid, string error)
        {
            IsValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
            Message = error ?? throw new ArgumentNullException(nameof(error));
        }
    }
}
