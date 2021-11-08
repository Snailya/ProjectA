using System;
using System.Text.RegularExpressions;

namespace ProjectA.Core.Models
{
    public record VersionNumber(int Major, int Minor) : IComparable<VersionNumber>
    {
        public int CompareTo(VersionNumber other)
        {
            return this < other ? -1 : this == other ? 0 : 1;
        }

        public bool IsMajorVersion()
        {
            return Minor == 0;
        }

        public static VersionNumber FromFileCurVerNumStr(string fileCurVerNumStr)
        {
            if (!Regex.IsMatch(fileCurVerNumStr, @"^(\d+).(\d+)$"))
                throw new ArgumentException("{0} format not match", nameof(fileCurVerNumStr));

            var subStrings = fileCurVerNumStr.Split(".");
            var major = int.Parse(subStrings[0]);
            var minor = int.Parse(subStrings[1]);
            return new VersionNumber(major, minor);
        }

        public static bool operator >(VersionNumber a, VersionNumber b)
        {
            return a.Major > b.Major || a.Major == b.Major && a.Minor > b.Minor;
        }

        public static bool operator <(VersionNumber a, VersionNumber b)
        {
            return b.Major > a.Major || b.Major == a.Major && b.Minor > a.Minor;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }
}