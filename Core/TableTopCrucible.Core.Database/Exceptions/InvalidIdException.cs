using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Database.Exceptions
{
    public class InvalidIdException:Exception
    {
        public InvalidIdException(string message):base(message)
        {
                
        }
    }
}
