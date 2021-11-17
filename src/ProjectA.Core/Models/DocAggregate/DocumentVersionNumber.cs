using System;
using System.Text.RegularExpressions;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate
{
    public class DocumentVersionNumber : ValueObject, IComparable<DocumentVersionNumber>
    {
        public DocumentVersionNumber(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public int Major { get; }
        public int Minor { get; }

        public int CompareTo(DocumentVersionNumber other)
        {
            return this < other ? -1 : this == other ? 0 : 1;
        }

        public bool IsMajorVersion()
        {
            return Minor == 0;
        }

        public static DocumentVersionNumber FromFileCurVerNumStr(string fileCurVerNumStr)
        {
            if (!Regex.IsMatch(fileCurVerNumStr, @"^(\d+).(\d+)$"))
                throw new ArgumentException("{0} format not match", nameof(fileCurVerNumStr));

            var subStrings = fileCurVerNumStr.Split(".");
            var major = int.Parse(subStrings[0]);
            var minor = int.Parse(subStrings[1]);
            return new DocumentVersionNumber(major, minor);
        }

        public static bool operator >(DocumentVersionNumber a, DocumentVersionNumber b)
        {
            return a.Major > b.Major || a.Major == b.Major && a.Minor > b.Minor;
        }

        public static bool operator <(DocumentVersionNumber a, DocumentVersionNumber b)
        {
            return b.Major > a.Major || b.Major == a.Major && b.Minor > a.Minor;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }
}