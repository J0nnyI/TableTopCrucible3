using System;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    ///     Filename without path or extension
    /// </summary>
    public class BareFileName : ValueType<string, BareFileName>
    {
        public static BareFileName TimeSuffix => (BareFileName)$"_{DateTime.Now:yyyyMMdd_HHmmssss}";

        public static BareFileName operator +(BareFileName bareFileA, BareFileName bareFileB) =>
            From(bareFileA.Value + bareFileB.Value);

        public static FileName operator +(BareFileName fileName, FileExtension extension) =>
            FileName.From(fileName.Value + extension.Value);

        public Name ToName()
            => (Name)Value;
    }
}