using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileExtension : ValueType<string, FileExtension>
    {
        public static readonly FileExtension JSonLibrary = From(".ttcjl");
        public static readonly FileExtension TemporaryJSonLibrary = From(".ttcjlt");
        public static readonly FileExtension JSON = From(".json");
        public static readonly IEnumerable<FileExtension> LibraryFile = new[] { JSonLibrary, TemporaryJSonLibrary };
        public static readonly IEnumerable<FileExtension> SlicerProject = FromList(".3mf");
        public static readonly IEnumerable<FileExtension> SlicedFile = FromList(".photon", ".gcode");
        public static readonly IEnumerable<FileExtension> Archive = FromList(".zip");

        public static readonly IEnumerable<FileExtension> Model = FromList(".stl", ".obj", ".off", ".objz", ".lwo",
            ".3ds");

        public static readonly FileExtension UncompressedImage = (FileExtension)".bmp";

        public static readonly IEnumerable<FileExtension> Image = FromList(".png", ".jpg", ".jpeg",
            UncompressedImage.Value, ".gif",
            ".hdp", ".jp2", ".pbm", ".psd", ".tga", ".tiff", ".img");

        public static readonly FileExtension Library = From(".ttcl");
        public static readonly FileExtension Table = From(".ttct");

        public static IEnumerable<FileExtension> FromList(params string[] values) => values.Select(From);

        protected override void Validate(string value)
        {
            base.Validate(value);
            if (!value.StartsWith('.'))
                throw new InvalidValueException("The extension has to start a '.'");
        }

        protected override string Sanitize(string value) => value.ToLower();

        public FileExtension ToLower() => From(Value.ToLower());

        public bool IsModel() => Model.Contains(ToLower());

        public bool IsImage() => Image.Contains(ToLower());

        public bool IsSlicerProject() => SlicerProject.Contains(ToLower());

        public bool IsSlicedFile() => SlicedFile.Contains(ToLower());

        public bool IsArchive() => Archive.Contains(ToLower());

        public bool IsLibrary() => LibraryFile.Contains(ToLower());

        public bool IsTable() => Table == ToLower();

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
                return FileType.SlicedFile;
            if (IsArchive())
                return FileType.Archive;

            return FileType.Other;
        }
    }
}