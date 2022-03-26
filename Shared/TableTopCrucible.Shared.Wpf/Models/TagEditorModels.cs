using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Shared.Wpf.Models
{
    namespace TagEditor
    {
        public enum DisplayMode
        {
            /// <summary>
            /// the + button to add a tag
            /// </summary>
            New,
            /// <summary>
            /// the simple view without progress bar, i.e. for single items
            /// </summary>
            Simple,
            /// <summary>
            /// the complex view with progress bar, i.e. for multiSelection
            /// </summary>
            Fraction
        }
        public enum WorkMode
        {
            Edit,
            View
        }
    }
}
