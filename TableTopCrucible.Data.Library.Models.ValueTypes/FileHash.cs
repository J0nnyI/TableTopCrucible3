using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;

namespace TableTopCrucible.Data.Library.Models.ValueTypes
{
    public struct FileHash
    {
        public FileHash(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public byte[] Data { get; }

        public override bool Equals(object obj)
            => obj is FileHash hash &&
            (Data != null && hash.Data != null && Data.SequenceEqual(hash.Data) ||
            Data == null && hash.Data == null);
        public override int GetHashCode()
            => ToString().GetHashCode();
        public override string ToString()
            => BitConverter.ToString(Data);

        public static bool operator !=(FileHash hashA, FileHash hashB)
            => !hashA.Equals(hashB);
        public static bool operator ==(FileHash hashA, FileHash hashB)
            => hashA.Equals(hashB);
        public static FileHash Create(string path, HashAlgorithm hashAlgorithm)
        {
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
        public static IObservable<T> CreateMany<T>(
            IEnumerable<T> fileModels,
            Func<T, string> pathReader,
            Action<T, FileHash> hashWriter,
            Func<int, IProgressionController> progressionGenerator,
            int threadcount)
        {
            var prog = progressionGenerator(fileModels.Count());
            return Observable.Return(fileModels)
                .SelectMany(list =>
                    list.SplitEvenly(threadcount)
                        .Select(fileGroup =>
                        {

                            // start 1 observable/thread
                            return Observable.Start(() =>
                            {
                                using var algorithm = SHA512.Create();
                                using var sub = new ReplaySubject<T>();
                                foreach (var fileModel in fileGroup)
                                {
                                    hashWriter(fileModel, Create(pathReader(fileModel), algorithm));
                                    sub.OnNext(fileModel);
                                    prog.CurrentProgress++;
                                }
                                return sub;
                            }, RxApp.TaskpoolScheduler).Switch();
                        })
            .Merge())
            .Replay();
        }
    }
}
