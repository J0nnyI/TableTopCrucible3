using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Helper.Exceptions
{
    public class UniqueConstraintFailedException:Exception
    {
        public UniqueConstraintFailedException(string message):base(message)
        {
            
        }
    }
    public class ExceptionObjectInfo<TObject, TId>
    {
        public TId Id { get; init; }
        public TObject OldObject { get; init; }
        public IEnumerable<TObject> NewObjects { get; init; }
        public override string ToString()
        {
            return string.Join(Environment.NewLine,
                       $"Id: {Id}",
                       $"old object: {OldObject}",
                       $"new objects: "
                   ) +
                   string.Join(Environment.NewLine + "---------------------" + Environment.NewLine,
                       NewObjects.Select(x => x.ToString()).ToArray());
        }
    }
    public class UniqueConstraintFailedException<TId,TObject> : UniqueConstraintFailedException
    {
        public UniqueConstraintFailedException(IEnumerable<ExceptionObjectInfo<TObject, TId>> objects) 
            : base(
                "Unique Constraint failed" + Environment.NewLine+
                string.Join(
                    Environment.NewLine,
                        objects.Select(obj=>obj.ToString())
                        .ToArray()
                    )
            )
        {

        }
    }
}
