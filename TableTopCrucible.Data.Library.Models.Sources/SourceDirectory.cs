using System;

using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct SourceDirectory
    {
        public string Path { get; }
        /// <summary>
        /// the path where thumbnails are stored
        /// </summary>
        public string ThumbnailSubDir { get; }
        public string ThumbnailPath => System.IO.Path.Combine(Path, ThumbnailSubDir);
        public DirectorySetupName Name { get; }
        public Description Description { get; }


        public Guid Identity { get; }

        public SourceDirectoryId Id { get; }
        public DateTime Created { get; }
        public DateTime LastChange { get; }


        public SourceDirectory(string path, DirectorySetupName name, Description description)
            : this(path, name, description, SourceDirectoryId.New(), DateTime.Now)
        { }
        public SourceDirectory(SourceDirectory origin, string path, DirectorySetupName name, Description description)
            : this(path, name, description, origin.Id, origin.Created)
        { }

        public SourceDirectory(string path, DirectorySetupName name, Description description, SourceDirectoryId id, DateTime created, DateTime? lastChange = null)
        {
            Path = path;
            Name = name;
            Description = description;
            ThumbnailSubDir = @"\Thumbnails";

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
