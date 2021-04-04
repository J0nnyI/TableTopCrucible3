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
      
    }
}
