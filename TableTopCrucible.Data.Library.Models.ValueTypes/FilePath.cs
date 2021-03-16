using System;
using System.Collections.Generic;
using System.Linq;

using TableTopCrucible.Core.Data;

namespace TableTopCrucible.Data.Library.Models.ValueTypes
{
    public struct FilePath
    {
        public Uri RootPath { get; }
        public Uri Path { get; }

        public FilePath(Uri rootPath, Uri relativePath)
        {
            RootPath = rootPath;
            Path = relativePath.IsAbsoluteUri
                ? rootPath.MakeRelativeUri(relativePath)
                : relativePath;
            _asAbsolute = null;
        }
        private string _asAbsolute;
        public string AsAbsolute
        {
            get
            {
                if (_asAbsolute == null)
                    _asAbsolute = RootPath.MakeRelativeUri(Path).LocalPath;
                return _asAbsolute;
            }
        }
        public override string ToString()
            => Path.ToString();
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case string str:
                    return Path == new Uri(str);
                case Uri uri:
                    return Path == uri;
                case FilePath path:
                    return Path == path.Path;
                default:
                    return false;
            }
        }
        public override int GetHashCode()
            => Path.GetHashCode();

        public static IEnumerable<string> Validate(string tag)
        {
            return Validators
                .Where(x => !x.IsValid(tag))
                .Select(x => x.Message)
                .ToArray();
        }
        public static IEnumerable<Validator<string>> Validators { get; } = new Validator<string>[] {
            new Validator<string>(directoryPath=>!string.IsNullOrWhiteSpace(directoryPath),"The path must not be empty")
        };
    }
}
