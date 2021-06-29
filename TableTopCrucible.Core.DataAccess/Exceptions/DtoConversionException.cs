using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.DataAccess.Exceptions
{
    public class DtoConversionException:Exception
    {
        public DtoConversionException(Exception innerException):base(null, innerException)
        {

        }
    }
}
