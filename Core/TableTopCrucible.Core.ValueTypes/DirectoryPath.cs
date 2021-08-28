using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;

using Splat;

using System;
using System.IO.Abstractions;
using System.Linq;
using System.Linq.Expressions;

using TableTopCrucible.Core.ValueTypes.Exceptions;

using ValueOf;

using static TableTopCrucible.Core.Helper.FileSystemHelper;

using SearchOption = System.IO.SearchOption;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    /// the path of a directory
    /// </summary>
    public class DirectoryPath<Tthis> : ValueOf<string, Tthis> where Tthis : DirectoryPath<Tthis>, new()
    {

        public static FilePath operator +(DirectoryPath<Tthis> directory, FileName fileName)
            => FilePath.From(Path.Combine(directory.Value, fileName.Value));
        public static Tthis operator +(DirectoryPath<Tthis> directory, DirectoryName subDirectory)
            => From(Path.Combine(directory.Value, subDirectory.Value));
        public static Tthis operator +(DirectoryPath<Tthis> directory, RelativeDirectoryPath relativeDirectory)
            => From(Path.Combine(directory.Value, relativeDirectory.Value));

        protected override void Validate()
        {
            // throws an exception if the path is invalid or relative
            try
            {
                Path.IsPathRooted(Value);
            }
            catch (Exception ex)
            {
                throw new InvalidPathException($"The path '{Path}' is either not relative or invalid", ex);
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
        public void Create()
        {
            try
            {
                Directory.CreateDirectory(Value);

            }
            catch (Exception ex)
            {

                throw new DirectoryCreationFailedException<Tthis>(this as Tthis, ex);
            }
        }
        public void Delete(bool recursive = true)
        {
            try
            {
                Directory.Delete(Value, recursive);
            }
            catch (Exception ex)
            {
                throw new DirectoryDeletionFailedException<Tthis>(this as Tthis, ex);
            }
        }

        public IDirectoryInfo GetParent()
            => Directory.GetParent(Value);
        public void Rename(Tthis newLocation)
        {
            try
            {
                if (!this.GetParent().Exists)
                    return;
                var newParent = newLocation.GetParent();
                if (!newParent.Exists)
                    Directory.CreateDirectory(newParent.FullName);
                Directory.Move(Value, newLocation.Value);

            }
            catch (Exception ex)
            {
                throw new DirectoryMoveFailedException<Tthis>(this as Tthis, newLocation, ex);
            }
        }

        public void Move(Tthis newLocation) => Directory.Move(Value, newLocation.Value);
        public DirectoryName GetDirectoryName() =>
            DirectoryName.From(Value.Split(Path.DirectorySeparatorChar).Last());
        public string[] GetFiles(string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
            => Locator.Current.GetService<IFileSystem>().Directory.GetFiles(Value, searchPattern, searchOption);

    }
    public class DirectoryPath : DirectoryPath<DirectoryPath>
    {

        public static FilePath operator +(DirectoryPath directory, FileName fileName)
            => FilePath.From(Path.Combine(directory.Value, fileName.Value));
        public static DirectoryPath operator +(DirectoryPath directory, DirectoryName subDirectory)
            => From(Path.Combine(directory.Value, subDirectory.Value));

        public static DirectoryPath AppData => DirectoryPath.From(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + DirectoryName.From("TableTopCrucible");

        public static DirectoryPath GetTemporaryPath()
            => From(Path.GetTempPath());
    }

}
