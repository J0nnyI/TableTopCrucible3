using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.DataAccess.ValueTypes
{
    public class TableSaveId
    {
        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public static TableSaveId New(Guid? id = null, DateTime? timestamp = null)
        => new TableSaveId
        {
            Id = id ?? Guid.NewGuid(),
            Timestamp = timestamp ?? DateTime.Now
        };
        public override string ToString()
            => Timestamp.ToString("yyyy-MM-dd-HH-mm-ss-ffff--" + Id.ToString());
        public BareFileName GetBareFilename()
            => BareFileName.From(this.ToString());
    }
}
