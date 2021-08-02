using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;
using TableTopCrucible.Core.Jobs.Managers;

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
        public static IEnumerable<IProgressionViewer> CreateMany<T>(
            T[] fileModel,
            Func<T, string> pathReader,
            Action<T, FileHash> hashWriter,
            int threadCount,
            Func<int, string, string, IProgressionHandler> trackerGenerator)
        {
            using var totalProgress = 
                trackerGenerator(fileModel.Count(), "Hashing", $"with {threadCount} threads");

            using HashAlgorithm hashAlgorithm = SHA512.Create();
            
            fileModel
                .AsParallel()
                .WithDegreeOfParallelism(threadCount)
                .ForAll((file) =>
                {
                    hashWriter(file, Create(pathReader(file)));
                    totalProgress.Increase();
                });


        }
    }
}
