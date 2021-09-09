using System;

namespace TableTopCrucible.Core.WPF.Testing.Exceptions
{
    public class ButtonDisbledException : Exception
    {
        public ButtonDisbledException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
