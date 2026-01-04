namespace CIG
{
    public class CIGGameVersion : IStorable
    {
        private const string MajorKey = "Major";

        private const string MinorKey = "Minor";

        private const string RevisionKey = "Revision";

        public int Major
        {
            get;
        }

        public int Minor
        {
            get;
        }

        public int Revision
        {
            get;
        }

        public CIGGameVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public CIGGameVersion(StorageDictionary storage)
        {
            Major = storage.Get("Major", 0);
            Minor = storage.Get("Minor", 0);
            Revision = storage.Get("Revision", 0);
        }

        public static CIGGameVersion GetCurrentVersion()
        {
            string[] array = "1.11.8".Split('.');
            if (array.Length == 3 && int.TryParse(array[0], out int result) && int.TryParse(array[1], out int result2) && int.TryParse(array[2], out int result3))
            {
                return new CIGGameVersion(result, result2, result3);
            }
            return new CIGGameVersion(0, 0, 0);
        }

        public override int GetHashCode()
        {
            return (int)(((long)-327234472 * -1521134295 + Major.GetHashCode()) * -1521134295 + Minor.GetHashCode()) * -1521134295 + Revision.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            CIGGameVersion cIGGameVersion = obj as CIGGameVersion;
            if (cIGGameVersion != null && Major == cIGGameVersion.Major && Minor == cIGGameVersion.Minor)
            {
                return Revision == cIGGameVersion.Revision;
            }
            return false;
        }

        private bool GreaterThan(CIGGameVersion other)
        {
            if (other != null)
            {
                if (Major <= other.Major)
                {
                    if (Major == other.Major)
                    {
                        if (Minor <= other.Minor)
                        {
                            if (Minor == other.Minor)
                            {
                                return Revision > other.Revision;
                            }
                            return false;
                        }
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool LessThan(CIGGameVersion other)
        {
            if (other != null)
            {
                if (Major >= other.Major)
                {
                    if (Major == other.Major)
                    {
                        if (Minor >= other.Minor)
                        {
                            if (Minor == other.Minor)
                            {
                                return Revision < other.Revision;
                            }
                            return false;
                        }
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool operator ==(CIGGameVersion a, CIGGameVersion b)
        {
            return a?.Equals(b) ?? ((object)b == null);
        }

        public static bool operator !=(CIGGameVersion a, CIGGameVersion b)
        {
            return !(a == b);
        }

        public static bool operator >=(CIGGameVersion a, CIGGameVersion b)
        {
            if (!(a == b))
            {
                return a > b;
            }
            return true;
        }

        public static bool operator <=(CIGGameVersion a, CIGGameVersion b)
        {
            if (!(a == b))
            {
                return a < b;
            }
            return true;
        }

        public static bool operator >(CIGGameVersion a, CIGGameVersion b)
        {
            return a?.GreaterThan(b) ?? ((object)b == null);
        }

        public static bool operator <(CIGGameVersion a, CIGGameVersion b)
        {
            return a?.LessThan(b) ?? ((object)b == null);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}";
        }

        StorageDictionary IStorable.Serialize()
        {
            StorageDictionary storageDictionary = new StorageDictionary();
            storageDictionary.Set("Major", Major);
            storageDictionary.Set("Minor", Minor);
            storageDictionary.Set("Revision", Revision);
            return storageDictionary;
        }
    }
}
