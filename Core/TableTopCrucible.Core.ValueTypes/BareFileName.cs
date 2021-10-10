using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    ///     Filename without path or extension
    /// </summary>
    public class BareFileName : ValueOf<string, BareFileName>
    {
        public static BareFileName operator +(BareFileName bareFileA, BareFileName bareFileB) =>
            From(bareFileA.Value + bareFileB.Value);

        public static FileName operator +(BareFileName fileName, FileExtension extension) =>
            FileName.From(fileName.Value + extension.Value);

        public Name ToName()
            => (Name) Value;
    }
}