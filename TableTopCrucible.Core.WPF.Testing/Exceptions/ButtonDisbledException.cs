using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.WPF.Testing.Exceptions
{
    public class ButtonDisbledException : Exception
    {
        public ButtonDisbledException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
