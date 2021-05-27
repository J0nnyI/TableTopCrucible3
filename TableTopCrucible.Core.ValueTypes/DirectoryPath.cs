using ReactiveUI.Validation.Abstractions;

using System;
using System.IO;
using System.Linq;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Core.ValueTypes.Exceptions;

using ValueOf;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Contexts;
using System.Linq.Expressions;
using ReactiveUI;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    /// the path of a directory
    /// </summary>
    public class DirectoryPath : ValueOf<string, DirectoryPath>
    {
        public static FilePath operator +(DirectoryPath directory, FileName fileName)
            => FilePath.From(Path.Combine(directory.Value, fileName.Value));
        public static DirectoryPath operator +(DirectoryPath directory, DirectoryName subDirectory)
            => DirectoryPath.From(Path.Combine(directory.Value, subDirectory.Value));

        protected override void Validate()
        {
            // throws an exception if the path is invalid or relative
            try
            {
                Path.IsPathRooted(Value);
            }
            catch (Exception ex)
            {
                throw new InvalidPathException("The path is either not relative or invalid", ex);
            }

            if (string.IsNullOrWhiteSpace(Value))
                throw new InvalidPathException("The path must not be empty");
        }
        public static void RegisterValidator<T>(T vm, Expression<Func<T, string>> propertyName, bool includeExists = true) where T : ReactiveObject, IValidatableViewModel
        {
            vm.ValidationRule(propertyName, value => !string.IsNullOrWhiteSpace(value), "The path must not be empty");

            if (includeExists)
                vm.ValidationRule(propertyName, value => Directory.Exists(value), "This directory does not exist");

            vm.ValidationRule(propertyName,
                value =>
                {
                    try
                    {
                        return Path.IsPathRooted(value);
                    }
                    catch (Exception) { return false; }
                }, "This is not a valid directory path");
        }

        public bool Exists() => Directory.Exists(Value);
        public void CreateDirectory() => Directory.CreateDirectory(Value);
        public DirectoryName GetDirectoryName() =>
            DirectoryName.From(Value.Split(Path.DirectorySeparatorChar).Last());
        public string[] GetFiles(string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
            => Directory.GetFiles(Value, searchPattern, searchOption);

        public static DirectoryPath GetTemporaryPath()
            => From(Path.GetTempPath());
    }

}
