using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using TableTopCrucible.Core.Helper;

namespace TableTopCurcible.Data.Library.Models.ValueTypes.General
{
    public struct Version : IComparable<Version>, IComparable
    {
        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        public int CompareTo([AllowNull] Version other)
        {
            if (other == default)
                return -1;

            if (Major < other.Major)
                return -1;
            if (Major > other.Major)
                return 1;

            if (Minor < other.Minor)
                return -1;
            if (Minor > other.Minor)
                return 1;

            if (Patch < other.Patch)
                return -1;
            if (Patch > other.Patch)
                return 1;

            return 0;
        }

        public override bool Equals(object obj) => obj is Version version && Major == version.Major && Minor == version.Minor && Patch == version.Patch;
        public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);
        public static bool operator ==(Version v1, Version v2)
        => v1.Equals(v2);
        public static bool operator !=(Version v1, Version v2)
        => !v1.Equals(v2);
        public static bool operator >=(Version v1, Version v2)
            => v1.CompareTo(v2) >= 0;
        public static bool operator >(Version v1, Version v2)
            => v1.CompareTo(v2) > 0;
        public static bool operator <=(Version v1, Version v2)
            => v1.CompareTo(v2) <= 0;
        public static bool operator <(Version v1, Version v2)
            => v1.CompareTo(v2) > 0;
        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        public int CompareTo(object obj)
        {
            if (obj is Version ver)
                return CompareTo(ver);
            return -1;
        }
        public static Version GetNextVersion(Version previousVersion, IEnumerable<Version> takenVersions)
        {
            var nextMajor = previousVersion.Major + 1;
            var nextMinor = previousVersion.Minor + 1;
            var nextPatch = previousVersion.Patch + 10000;

            if (!takenVersions.Any(x =>
                    x.Major == nextMajor))
                return new Version(nextMajor, 0, 0);

            if (!takenVersions.Any(x =>
                    x.Major == previousVersion.Major &&
                    x.Minor == nextMinor))
                return new Version(previousVersion.Major, nextMinor, 0);



            var patchRange = takenVersions.Where(x =>
                    x.Major == previousVersion.Major &&
                    x.Minor == previousVersion.Minor &&
                    x.Patch.Between(previousVersion.Patch + 1, nextPatch));

            if (!patchRange.Any())
                return new Version(previousVersion.Major, previousVersion.Minor, nextPatch);

            var nextTakenVersion = patchRange.Min();

            var fallbackPatch = (nextTakenVersion.Patch - previousVersion.Patch) / 2 + previousVersion.Patch;

            return new Version(previousVersion.Major, previousVersion.Minor, fallbackPatch);
        }
    }
}
