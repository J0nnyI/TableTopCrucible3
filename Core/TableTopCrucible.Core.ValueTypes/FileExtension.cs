using System;
using System.Collections.Generic;
using System.Linq;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileExtension : ValueOf<string, FileExtension>
    {
        public static readonly FileExtension JSON = From(".json");
        public static readonly IEnumerable<FileExtension> SlicerProject = FromList(".3mf");
        public static readonly IEnumerable<FileExtension> SlicedFile = FromList(".photon", ".stl");
        public static readonly IEnumerable<FileExtension> Archive = FromList(".zip");
        public static readonly IEnumerable<FileExtension> Model = FromList(".stl", ".obj", ".off", ".objz", ".lwo", ".3ds");
        public static readonly IEnumerable<FileExtension> Image = FromList(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".hdp", ".jp2", ".pbm", ".psd", ".tga", ".tiff", ".img");
        public static readonly FileExtension Library = From(".ttcl");
        public static readonly FileExtension Table = From(".ttct");
        public static IEnumerable<FileExtension> FromList(params string[] values)
            => values.Select(FileExtension.From);
        public FileExtension ToLower()
            => From(this.Value.ToLower());
        public bool IsModel()
             => Model.Contains(this.ToLower());
        public bool IsImage()
             => Image.Contains(this.ToLower());
        public bool IsSlicerProject()
            => SlicerProject.Contains(this.ToLower());

        public bool IsSlicedFile()
            => SlicedFile.Contains(this.ToLower());
        public bool IsArchive()
            => Archive.Contains(this.ToLower());

        public bool IsLibrary()
             => Library == this.ToLower();
        public bool IsTable()
             => Table == this.ToLower();
        public FileType GetFileType()
        {
            if (IsModel())
                return FileType.Model;
            if (IsImage())
                return FileType.Image;
            if (IsTable())
                return FileType.Table;
            if (IsLibrary())
                return FileType.Library;
            if (IsSlicerProject())
                return FileType.SlicerProject;
            if (IsSlicedFile())
                return FileType.SlicerProject;
            if (IsArchive())
                return FileType.Archive;

            return FileType.Other;
        }


    }
}
