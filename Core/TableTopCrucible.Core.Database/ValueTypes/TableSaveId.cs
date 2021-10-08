using System;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Database.ValueTypes
{
    public class TableSaveId
    {
        public Guid Id { get; private init; }
        public DateTime Timestamp { get; private init; }
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
