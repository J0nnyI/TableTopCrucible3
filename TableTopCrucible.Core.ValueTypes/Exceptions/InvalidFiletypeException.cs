using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class InvalidFiletypeException:Exception
    {
        public InvalidFiletypeException():base()
        {

        }
        public InvalidFiletypeException(string message) : base(message)
        {

        }
    }
}
