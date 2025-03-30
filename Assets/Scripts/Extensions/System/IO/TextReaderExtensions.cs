using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PAC.Extensions.System.IO
{
    public static class TextReaderExtensions
    {
        /// <summary>
        /// Reads, checking if it reads the given string (starting at the current point). Returns the characters it matched before it found a non-match. Doesn't reach the non-match
        /// character / the next character after all have characters been matched.
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static string ReadMatch(this TextReader reader, string text) => reader.ReadMatch((IEnumerable<char>)text);
        /// <summary>
        /// Reads, checking if it reads the given sequence of characters (starting at the current point). Returns the characters it matched before it found a non-match. Doesn't reach the
        /// non-match character / the next character after all have characters been matched.
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static string ReadMatch(this TextReader reader, IEnumerable<char> text)
        {
            if (reader.Peek() == -1)
            {
                throw new EndOfStreamException("The given TextReader has nothing left to read.");
            }

            StringBuilder matched = new StringBuilder();
            foreach (char textChr in text)
            {
                int peek = reader.Peek();
                if (peek == -1)
                {
                    return matched.ToString();
                }

                if (peek != textChr)
                {
                    return matched.ToString();
                }

                matched.Append((char)reader.Read());
            }

            return matched.ToString();
        }
        /// <summary>
        /// Reads, checking if the first read character is in the first collection of characters, then if the second is in the second collection, etc. until all character collections have
        /// been matched or until a character doesn't match. Returns the characters it matched before it found a non-match. Doesn't reach the non-match character / the next character after
        /// all have characters been matched.
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static string ReadMatch(this TextReader reader, IEnumerable<ICollection<char>> charSets)
        {
            if (reader.Peek() == -1)
            {
                throw new EndOfStreamException("The given TextReader has nothing left to read.");
            }

            StringBuilder matched = new StringBuilder();
            foreach (ICollection<char> charSet in charSets)
            {
                int peek = reader.Peek();
                if (peek == -1)
                {
                    return matched.ToString();
                }

                if (!charSet.Contains((char)peek))
                {
                    return matched.ToString();
                }

                matched.Append((char)reader.Read());
            }

            return matched.ToString();
        }
        /// <summary>
        /// Reads, checking if the first read character satisfies the first match condition, then if the second matches the second condition, etc. until all character collections have
        /// been matched or until a character doesn't match. Returns the characters it matched before it found a non-match. Doesn't reach the non-match character / the next character after
        /// all have characters been matched.
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static string ReadMatch(this TextReader reader, IEnumerable<Func<char, bool>> matchConditions)
        {
            if (reader.Peek() == -1)
            {
                throw new EndOfStreamException("The given TextReader has nothing left to read.");
            }

            StringBuilder matched = new StringBuilder();
            foreach (Func<char, bool> matchCondition in matchConditions)
            {
                int peek = reader.Peek();
                if (peek == -1)
                {
                    return matched.ToString();
                }

                if (!matchCondition.Invoke((char)peek))
                {
                    return matched.ToString();
                }

                matched.Append((char)reader.Read());
            }

            return matched.ToString();
        }

        /// <summary>
        /// Reads, checking if it reads the given string (starting at the current point). Doesn't reach the non-match character / the next character after all have characters been matched.
        /// Doesn't reach the non-match character / the next character after all have characters been matched.
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static bool ReadMatchAll(this TextReader reader, string text) => reader.ReadMatchAll((IEnumerable<char>)text);
        /// <summary>
        /// Reads, checking if it reads the given sequence of characters (starting at the current point).
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static bool ReadMatchAll(this TextReader reader, IEnumerable<char> text)
        {
            string matched = reader.ReadMatch(text);

            if (matched.Length == text.Count())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Reads, checking if the first read character is in the first collection of characters, then if the second is in the second collection, etc. until all character collections have
        /// been matched. Doesn't reach the non-match character / the next character after all have characters been matched.
        /// </summary>
        /// <exception cref="EndOfStreamException">If there is nothing left for the reader to read at the start of the method.</exception>
        public static bool ReadMatchAll(this TextReader reader, IEnumerable<ICollection<char>> charSets)
        {
            string matched = reader.ReadMatch(charSets);

            if (matched.Length == charSets.Count())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// If the TextReader can read another character, it will read it into chr and return true. Otherwise, it will return false without changing chr and without reading.
        /// </summary>
        public static bool TryReadIntoChar(this TextReader reader, ref char chr)
        {
            int read = reader.Peek();
            if (read == -1)
            {
                return false;
            }
            chr = (char)reader.Read();
            return true;
        }
    }
}
