using System;
using System.Collections.Generic;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct SourceDirectory
    {
        public SourceDirectory(DirectoryPath dir)
            : this(SourceDirectoryId.New(), 
                  dir, dir + DirectoryName.From("Thumbnails"), 
                  DirectorySetupName.From(dir.GetDirectoryName().Value))
        {
        }

        public SourceDirectory(SourceDirectoryId id, DirectoryPath filePath, DirectoryPath thumbnailSubDir, DirectorySetupName name)
        {
            Directory = filePath ?? throw new ArgumentNullException(nameof(filePath));
            ThumbnailPath = thumbnailSubDir;//todo ?? throw new ArgumentNullException(nameof(thumbnailSubDir));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public DirectoryPath Directory { get; }
        /// <summary>
        /// the path where thumbnails are stored
        /// </summary>
        public DirectoryPath ThumbnailPath { get; }
        public DirectorySetupName Name { get; }
        public SourceDirectoryId Id { get; }

        public override bool Equals(object obj)
        {
            return obj is SourceDirectory directory &&
                   EqualityComparer<DirectoryPath>.Default.Equals(Directory, directory.Directory) &&
                   EqualityComparer<DirectoryPath>.Default.Equals(ThumbnailPath, directory.ThumbnailPath) &&
                   EqualityComparer<DirectorySetupName>.Default.Equals(Name, directory.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Directory, ThumbnailPath, Name);
        }
    }
}
