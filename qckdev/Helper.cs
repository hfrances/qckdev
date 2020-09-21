using System;

namespace qckdev
{

    /// <summary>
    /// Provides a set of miscelaneous static (Shared in Visual Basic) methods.
    /// </summary>
    static class Helper
    {

        public static T Choose<T>(T val1, T val2, Func<T, T, T> conflictAction) where T : class
        {
            T rdo;

            if (val1 != null && val2 != null)
                rdo = conflictAction.Invoke(val1, val2);
            else if (val1 == null)
                rdo = val2;
            else
                rdo = val1;
            return rdo;
        }

        /// <remarks>
        /// Based on answer in https://stackoverflow.com/questions/5417070/c-sharp-version-of-sql-like
        /// </remarks>
        public static bool Like(string text, string pattern, StringLikeOptions options)
        {
            bool isMatch = true,
                isWildCardOn = false,
                isCharWildCardOn = false,
                isCharSetOn = false,
                isNotCharSetOn = false,
                endOfPattern = false;
            int lastWildCard = -1;
            int patternIndex = 0;
            var set = new System.Collections.Generic.List<char>();
            char p = '\0';

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                endOfPattern = (patternIndex >= pattern.Length);
                if (!endOfPattern)
                {
                    p = pattern[patternIndex];

                    if (!isWildCardOn && p == options.WildCard)
                    {
                        lastWildCard = patternIndex;
                        isWildCardOn = true;
                        while (patternIndex < pattern.Length &&
                            pattern[patternIndex] == options.WildCard)
                        {
                            patternIndex++;
                        }
                        if (patternIndex >= pattern.Length) p = '\0';
                        else p = pattern[patternIndex];
                    }
                    else if (p == options.CharWildCard)
                    {
                        isCharWildCardOn = true;
                        patternIndex++;
                    }
                    else if (p == '[')
                    {
                        if (pattern[++patternIndex] == '^')
                        {
                            isNotCharSetOn = true;
                            patternIndex++;
                        }
                        else isCharSetOn = true;

                        set.Clear();
                        if (pattern[patternIndex + 1] == '-' && pattern[patternIndex + 3] == ']')
                        {
                            char start, end;

                            start = (options.IgnoreCase ? char.ToUpper(pattern[patternIndex]) : pattern[patternIndex]);
                            patternIndex += 2;
                            end = (options.IgnoreCase ? char.ToUpper(pattern[patternIndex]) : pattern[patternIndex]);
                            if (start <= end)
                            {
                                for (char ci = start; ci <= end; ci++)
                                {
                                    set.Add(ci);
                                }
                            }
                            patternIndex++;
                        }

                        while (patternIndex < pattern.Length &&
                            pattern[patternIndex] != ']')
                        {
                            set.Add(pattern[patternIndex]);
                            patternIndex++;
                        }
                        patternIndex++;
                    }
                }

                if (isWildCardOn)
                {
                    var equals = (options.IgnoreCase ? char.ToUpper(c) == char.ToUpper(p) : c == p);
                    if (equals)
                    {
                        isWildCardOn = false;
                        patternIndex++;
                    }
                }
                else if (isCharWildCardOn)
                {
                    isCharWildCardOn = false;
                }
                else if (isCharSetOn || isNotCharSetOn)
                {
                    bool charMatch = (set.Contains(options.IgnoreCase ? char.ToUpper(c) : c));

                    if ((isNotCharSetOn && charMatch) || (isCharSetOn && !charMatch))
                    {
                        if (lastWildCard >= 0) patternIndex = lastWildCard;
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    isNotCharSetOn = isCharSetOn = false;
                }
                else
                {
                    var equals = (options.IgnoreCase ? char.ToUpper(c) == char.ToUpper(p) : c == p);

                    if (equals)
                    {
                        patternIndex++;
                    }
                    else
                    {
                        if (lastWildCard >= 0) patternIndex = lastWildCard;
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
            }
            endOfPattern = (patternIndex >= pattern.Length);

            if (isMatch && !endOfPattern)
            {
                bool isOnlyWildCards = true;
                for (int i = patternIndex; i < pattern.Length; i++)
                {
                    if (pattern[i] != options.WildCard)
                    {
                        isOnlyWildCards = false;
                        break;
                    }
                }
                if (isOnlyWildCards) endOfPattern = true;
            }
            return isMatch && endOfPattern;
        }

    }
}
