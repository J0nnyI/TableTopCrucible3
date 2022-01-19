using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace TableTopCrucible.Backend.Domain.Library
{
    public interface ILibraryController
    {
        TestItem getItem();
    }
    public class LibraryController : ILibraryController
    {
        private TestItem item = new TestItem();

        public TestItem getItem()
            => item;
    }


}
