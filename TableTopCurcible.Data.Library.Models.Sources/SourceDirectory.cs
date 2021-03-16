using System;
using System.IO;

using TableTopCrucible.Core.Data;

using TableTopCurcible.Data.Library.Models.ValueTypes.General;
using TableTopCurcible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct SourceDirectory : IEntity<SourceDirectoryId>
    {
        public Uri Path { get; }
        /// <summary>
        /// the path where thumbnails are stored
        /// </summary>
        public Uri ThumbnailUri { get; }
        public Uri AbsoluteThumbnailUri => new Uri(Path.LocalPath + ThumbnailUri);
        public DirectorySetupName Name { get; }
        public Description Description { get; }

        public bool IsValid
            => Path != null && Directory.Exists(Path.LocalPath);

        public Guid Identity { get; }

        public SourceDirectoryId Id { get; }
        public DateTime Created { get; }
        public DateTime LastChange { get; }


        public SourceDirectory(Uri path, DirectorySetupName name, Description description)
            : this(path, name, description, (SourceDirectoryId)Guid.NewGuid(), DateTime.Now)
        { }
        public SourceDirectory(SourceDirectory origin, Uri path, DirectorySetupName name, Description description)
            : this(path, name, description, origin.Id, origin.Created)
        { }

        public SourceDirectory(Uri path, DirectorySetupName name, Description description, SourceDirectoryId id, DateTime created, DateTime? lastChange = null)
        {
            Path = path;
            Name = name;
            Description = description;
            ThumbnailUri = new Uri(@"\Thumbnails", UriKind.Relative);

            Id = id;
            Identity = Guid.NewGuid();
            LastChange = lastChange ?? DateTime.Now;
            Created = created;
        }

        public override string ToString() => $"directory setup {Id} ({Name})";

        public static bool operator ==(SourceDirectory directorySetupA, SourceDirectory directorySetupB)
            => directorySetupA.Identity == directorySetupB.Identity;
        public static bool operator !=(SourceDirectory directorySetupA, SourceDirectory directorySetupB)
            => directorySetupA.Identity != directorySetupB.Identity;

        public override bool Equals(object obj) => obj is SourceDirectory dirSetup && this == dirSetup;
        public override int GetHashCode() => HashCode.Combine(Identity);

    }
}
