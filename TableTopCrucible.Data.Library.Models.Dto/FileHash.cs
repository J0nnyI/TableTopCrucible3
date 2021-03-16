using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;

using TableTopCrucible.Core.Enums;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;
using TableTopCrucible.Core.Models.Sources;

namespace TableTopCrucible.Domain.Models.ValueTypes
{
    public struct FileHash
    {
        public FileHash(byte[] data)
        {
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public byte[] Data { get; }

        public override bool Equals(object obj)
            => obj is FileHash hash &&
            (this.Data != null && hash.Data != null && Enumerable.SequenceEqual(this.Data, hash.Data) ||
            (this.Data == null && hash.Data == null));
        public override int GetHashCode()
            => this.ToString().GetHashCode();
        public override string ToString()
            => BitConverter.ToString(this.Data);

        public static bool operator !=(FileHash hashA, FileHash hashB)
            => !hashA.Equals(hashB);
        public static bool operator ==(FileHash hashA, FileHash hashB)
            => hashA.Equals(hashB);
        public static FileHash Create(string path, HashAlgorithm hashAlgorithm)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"could not find file {path}");

            using FileStream stream = File.OpenRead(path);
            byte[] data = hashAlgorithm.ComputeHash(stream);
            return new FileHash(data);
        }
        public static FileHash Create(string path)
        {
            using var hashAlgorithm = SHA512.Create();
            return Create(path, hashAlgorithm);
        }
        /**
         * each thread fires once
         */
        public static IEnumerable<IProgression> CreateMany<T>(
            IEnumerable<T> fileModel,
            Func<T, string> pathReader,
            Action<T, FileHash> hashWriter,
            int threadcount,
            Func<int, string, string, IProgressionController> trackerGenerator)
        {
            using var totalProg = trackerGenerator(fileModel.Count(), "Hashing", $"with {threadcount} threads");
            using HashAlgorithm hashAlgorithm = SHA512.Create();
            return fileModel.ToList()
                .SplitEvenly(threadcount)
                .Select(group =>
                {
                    using var subProg = trackerGenerator(group.Count(), "", "");

                    Observable.Start(() =>
                    {
                        trackerGenerator.
                        group.ToList().ForEach(file =>
                        {
                            hashWriter(file, Create(pathReader(file)));
                        });
                    }, RxApp.TaskpoolScheduler);
                    return subProg;
                })
                .ToArray();
        }
    }
}
