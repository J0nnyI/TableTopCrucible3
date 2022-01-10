using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    ///     the path of a directory
    /// </summary>
    public class DirectoryPath<TThis>
        : ValueType<string, TThis>
        where TThis : DirectoryPath<TThis>, new()
    {
        public static FilePath operator +(DirectoryPath<TThis> directory, FileName fileName) =>
            FilePath.From(FileSystemHelper.Path.Combine(directory.Value, fileName.Value));

        public static TThis operator +(DirectoryPath<TThis> directory, DirectoryName subDirectory) =>
            From(FileSystemHelper.Path.Combine(directory.Value, subDirectory.Value));

        public static TThis operator +(DirectoryPath<TThis> directory, RelativeDirectoryPath relativeDirectory) =>
            From(FileSystemHelper.Path.Combine(directory.Value, relativeDirectory.Value));

        public static Exception IsValid(string path, bool includeExists = false)
        {
            if (path == null)
                return new InvalidPathException("the path must not be null");
            // throws an exception if the path is invalid or relative

            if (includeExists && !FileSystemHelper.Directory.Exists(path))
                return new InvalidPathException("This directory does not exist");

            try
            {
                FileSystemHelper.Path.IsPathRooted(path);
            }
            catch (Exception ex)
            {
                return new InvalidPathException($"The path '{FileSystemHelper.Path}' is either not relative or invalid",
                    ex);
            }

            if (string.IsNullOrWhiteSpace(path))
                return new InvalidPathException("The path must not be empty");
            return null;
        }

        protected override void Validate(string value)
        {
            var ex = IsValid(value);
            if (ex != null) throw ex;
        }

        public static IDisposable RegisterValidator<T>(
            T vm,
            Expression<Func<T, string>> propertyName,
            bool includeExists = true,
            IObservable<IEnumerable<DirectoryPath<TThis>>> blacklistChanges = null
        ) where T : ReactiveObject, IValidatableViewModel
        {
            CompositeDisposable disposables = new();
            vm.ValidationRule(propertyName,
                    value => !string.IsNullOrWhiteSpace(value),
                    "The path must not be empty")
                .DisposeWith(disposables);

            if (includeExists)
                vm.ValidationRule(propertyName,
                        value => FileSystemHelper.Directory.Exists(value),
                        "This directory does not exist")
                    .DisposeWith(disposables);

            if (blacklistChanges != null)
                vm.ValidationRule(
                    propertyName,
                    // ReSharper disable once InvokeAsExtensionMethod
                    Observable.CombineLatest(
                        vm.WhenAnyValue(propertyName),
                        blacklistChanges.StartWith(Array.Empty<DirectoryPath<TThis>>()),
                        (prop, blacklist) =>
                            !blacklist.Select(vt => vt.Value).Contains(prop)
                    ),
                    "This directory has already been registered"
                ).DisposeWith(disposables);

            vm.ValidationRule(propertyName,
                    value =>
                    {
                        try
                        {
                            return FileSystemHelper.Path.IsPathRooted(value);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }, "This is not a valid directory path")
                .DisposeWith(disposables);
            return disposables;
        }

        public bool Exists() => FileSystemHelper.Directory.Exists(Value);

        public void Create()
        {
            try
            {
                FileSystemHelper.Directory.CreateDirectory(Value);
            }
            catch (Exception ex)
            {
                throw new DirectoryCreationFailedException<TThis>(this as TThis, ex);
            }
        }

        public void Delete(bool recursive = true)
        {
            try
            {
                FileSystemHelper.Directory.Delete(Value, recursive);
            }
            catch (Exception ex)
            {
                throw new DirectoryDeletionFailedException<TThis>(this as TThis, ex);
            }
        }

        public IDirectoryInfo GetParent() => FileSystemHelper.Directory.GetParent(Value);

        public void Rename(TThis newLocation)
        {
            try
            {
                if (!GetParent().Exists)
                    return;
                var newParent = newLocation.GetParent();
                if (!newParent.Exists)
                    FileSystemHelper.Directory.CreateDirectory(newParent.FullName);
                FileSystemHelper.Directory.Move(Value, newLocation.Value);
            }
            catch (Exception ex)
            {
                throw new DirectoryMoveFailedException<TThis>(this as TThis, newLocation, ex);
            }
        }

        public void Move(TThis newLocation)
        {
            FileSystemHelper.Directory.Move(Value, newLocation.Value);
        }

        public DirectoryName GetDirectoryName() =>
            DirectoryName.From(Value.Split(FileSystemHelper.Path.DirectorySeparatorChar).Last());

        public IEnumerable<FilePath> EnumerateFiles() =>
            FileSystemHelper.Directory.EnumerateFiles(Value, "*", SearchOption.AllDirectories)
                .Select(FilePath.From);

        public IEnumerable<FilePath> GetFiles(string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories) =>
            FileSystemHelper.Directory
                .GetFiles(Value, searchPattern, searchOption)
                .Select(FilePath.From)
                .ToImmutableArray();

        public IEnumerable<FilePath> GetFiles(params FileType[] types)
        {
            return EnumerateFiles()
                .Where(f => types.Contains(f.GetFileType()));
        }

        public bool ContainsFilepath<TFilePath>(TFilePath filePath)
            where TFilePath : FilePath<TFilePath>, new()
            => filePath.Value.ToLower().StartsWith(this.Value.ToLower());
    }

    public class DirectoryPath : DirectoryPath<DirectoryPath>
    {
        public static DirectoryPath AppData { get; }
            = From(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) +
            DirectoryName.From("TableTopCrucible");

        public static FilePath operator +(DirectoryPath directory, FileName fileName) =>
            FilePath.From(FileSystemHelper.Path.Combine(directory.Value, fileName.Value));

        public static DirectoryPath operator +(DirectoryPath directory, DirectoryName subDirectory) =>
            From(FileSystemHelper.Path.Combine(directory.Value, subDirectory.Value));

        public static DirectoryPath GetTemporaryPath() => From(FileSystemHelper.Path.GetTempPath());
        public static explicit operator DirectoryPath(string path) => From(path);
    }
}