using System.IO;

namespace PAC.Extensions
{
    public static class TextReaderExtensions
    {
        /// <summary>
        /// Reads, checking if it reads the given string (starting at the current point).
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static bool ReadMatch(this TextReader reader, string text)
        {
            if (reader.Peek() == -1)
            {
                throw new EndOfStreamException("The given TextReader has nothing left to read.");
            }

            int index = 0;
            while (index < text.Length)
            {
                int read = reader.Read();
                if (read == -1)
                {
                    return false;
                }

                char chr = (char)read;
                if (chr != text[index])
                {
                    return false;
                }

                index++;
            }

            return true;
        }

        /// <summary>
        /// If the TextReader can read another character, it will read it into chr and return true. Otherwise, it will return false without changing chr.
        /// </summary>
        public static bool ReadChar(this TextReader reader, ref char chr)
        {
            int read = reader.Read();
            if (read == -1)
            {
                return false;
            }
            chr = (char)read;
            return true;
        }
    }
}
