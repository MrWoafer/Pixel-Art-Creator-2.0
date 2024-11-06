using System;

namespace PAC.Files
{
    public struct SemanticVersion
    {
        private int _major;
        public int major
        {
            get => _major;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Major number must be non-negative, but the value given is: " + value, "value");
                }
                _major = value;
            }
        }

        private int _minor;
        public int minor
        {
            get => _minor;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Minor number must be non-negative, but the value given is: " + value, "value");
                }
                _minor = value;
            }
        }

        private int _patch;
        public int patch
        {
            get => _patch;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Patch number must be non-negative, but the value given is: " + value, "value");
                }
                _patch = value;
            }
        }

        public SemanticVersion(int major, int minor, int patch)
        {
            // We set these to 0 to get around the fact that you must declare all fields before you can use 'this'
            _major = 0;
            _minor = 0;
            _patch = 0;
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public static bool operator !=(SemanticVersion version1, SemanticVersion version2) => !(version1 == version2);
        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            return version1.major == version2.major && version1.minor == version2.minor && version1.patch == version2.patch;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj.GetType() == typeof(SemanticVersion))
            {
                return this == (SemanticVersion)obj;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(SemanticVersion version1, SemanticVersion version2) => version1 == version2 || version1 < version2;
        public static bool operator >=(SemanticVersion version1, SemanticVersion version2) => version1 == version2 || version1 > version2;
        public static bool operator <(SemanticVersion version1, SemanticVersion version2) => version2 > version1;
        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1.major > version2.major)
            {
                return true;
            }
            if (version1.major < version2.major)
            {
                return false;
            }

            if (version1.minor > version2.minor)
            {
                return true;
            }
            if (version1.minor < version2.minor)
            {
                return false;
            }

            if (version1.patch > version2.patch)
            {
                return true;
            }
            if (version1.patch < version2.patch)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// <para>
        /// Returns a Version such that the most important field in which the operands differ (in the order major, minor, patch) is set to the positive difference of them in this field,
        /// and the other two fields are set to 0.
        /// </para>
        /// <para>
        /// NOTE: This operation is commutative (swapping the order of the operands doesn't affect the result).
        /// </para>
        /// <example>
        /// Example:
        /// <code>
        /// Version(1, 4, 3) - Version(1, 6, 10) = Version(0, 2, 0)
        /// </code>
        /// </example>
        /// </summary>
        public static SemanticVersion operator -(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1.major != version2.major)
            {
                return new SemanticVersion(Math.Abs(version1.major - version2.major), 0, 0);
            }
            if (version1.minor != version2.minor)
            {
                return new SemanticVersion(0, Math.Abs(version1.minor - version2.minor), 0);
            }
            if (version1.patch != version2.patch)
            {
                return new SemanticVersion(0, 0, Math.Abs(version1.patch - version2.patch));
            }
            return new SemanticVersion(0, 0, 0);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(major, minor, patch);
        }

        public override string ToString()
        {
            return major + "." + minor + "." + patch;
        }

        /// <summary>
        /// Adds 1 to the major and resets the minor and patch to 0.
        /// </summary>
        public void IncrementMajor()
        {
            _major += 1;
            _minor = 0;
            _patch = 0;
        }
        /// <summary>
        /// Adds 1 to the minor and resets the patch to 0. (The major is unchanged.)
        /// </summary>
        public void IncrementMinor()
        {
            _minor += 1;
            _patch = 0;
        }
        /// <summary>
        /// Adds 1 to the patch. (The major and minor are unchanged.)
        /// </summary>
        public void IncrementPatch()
        {
            _patch += 1;
        }
    }
}
