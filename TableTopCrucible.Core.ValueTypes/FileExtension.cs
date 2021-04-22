using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileExtension:ValueOf<string, FileExtension>
    {

        public static readonly IEnumerable<FileExtension> ModelExtensions = FileExtension.FromList(".stl", ".obj", ".off", ".objz", ".lwo", ".3ds");
        public static readonly IEnumerable<FileExtension> ImageExtensions = FileExtension.FromList(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".hdp", ".jp2", ".pbm", ".psd", ".tga", ".tiff", ".img");

        public static IEnumerable<FileExtension> FromList(params string[] values)
            => values.Select(FileExtension.From);
        public FileExtension ToLower()
            => From(this.Value.ToLower());
        public bool IsModel()
             => ModelExtensions.Contains(this.ToLower());
        public bool IsImage()
             => ImageExtensions.Contains(this.ToLower());
        public FileType GetFileType()
        {
            if (IsModel())
                return FileType.Model;
            if (IsImage())
                return FileType.Image;
            return FileType.Other;
        }
    }
}
