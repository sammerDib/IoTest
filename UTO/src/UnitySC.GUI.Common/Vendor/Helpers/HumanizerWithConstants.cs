using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Humanizer;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    /// <summary>
    /// Allows to humanize string by omitting constants.
    /// </summary>
    public static class HumanizerWithConstants
    {
        #region DescendingLengthComparer

        private sealed class DescendingLengthComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                if (x.Length > y.Length)
                {
                    return -1;
                }

                return x.Length == y.Length ? 1 : 0;
            }
        }

        #endregion

        #region Properties

        private static readonly SortedSet<string> PrivateConstStrings = new SortedSet<string>(new DescendingLengthComparer());

        /// <summary>
        /// Gets the list of constant tokens.
        /// </summary>
        public static ReadOnlyCollection<string> ConstStrings => PrivateConstStrings.ToList().AsReadOnly();

        #endregion

        #region Methods

        /// <summary>
        /// Create a list where all even indexes are string to be humanized and other are constants (defined in <see cref="PrivateConstStrings"/>).
        /// For example, an execution with the input "[CONST]Word[CONST]" will return ["", "[CONST]", "Word", "[CONST]", ""]
        /// </summary>
        /// <param name="input">The string to be tokenized.</param>
        /// <returns>The list of string where each string at an even index will be a string to be humanized. Others are constants.</returns>
        private static LinkedList<string> Tokenize(string input)
        {
            var splitTokenList = new LinkedList<string>();
            splitTokenList.AddFirst(input);

            // For each constant string, split each string to be humanized.
            foreach (var constString in ConstStrings)
            {
                var listIterator = splitTokenList.First;

                // Split each element of the list with an even index.
                do
                {
                    var oldString = listIterator;
                    var splitString = listIterator.Value.Split(new[] { constString }, StringSplitOptions.None);

                    // Add split elements to the token list (separated by the current constant string).
                    for (var i = 0; i < splitString.Length; ++i)
                    {
                        splitTokenList.AddBefore(listIterator, splitString[i]);

                        if (i < splitString.Length - 1)
                        {
                            splitTokenList.AddBefore(listIterator, constString);
                        }
                    }

                    // Remove from the list the string that just split and go to the following string.
                    listIterator = listIterator.Previous;
                    splitTokenList.Remove(oldString);
                    listIterator = listIterator?.Next?.Next;

                } while (listIterator != null);
            }

            return splitTokenList;
        }

        /// <summary>
        /// Humanize the <see cref="input"/> by considering constant tokens.
        /// </summary>
        /// <param name="input">The string to humanize.</param>
        /// <param name="letterCasing">The casing to apply.</param>
        /// <returns></returns>
        public static string Humanize(string input, LetterCasing letterCasing = LetterCasing.Sentence)
        {
            var stringList = Tokenize(input);
            var listIterator = stringList.First;
            var humanizedInput = new StringBuilder();

            var isFirstOfSentence = LetterCasing.Sentence == letterCasing;

            // Build the humanized string.
            do
            {
                var stringToHumanize = listIterator.Value;
                if (stringToHumanize != "")
                {
                    if (isFirstOfSentence || letterCasing != LetterCasing.Sentence)
                    {
                        humanizedInput.Append(listIterator.Value.Humanize(letterCasing)).Append(" ");
                    }
                    else
                    {
                        humanizedInput.Append(listIterator.Value.Humanize(LetterCasing.LowerCase)).Append(" ");
                    }

                }

                // Go to the next string to be added to the output.
                // If there is any, it is a constant because of the index parity in the list.
                listIterator = listIterator.Next;
                if (listIterator == null)
                {
                    continue;
                }

                humanizedInput.Append(listIterator.Value);
                isFirstOfSentence = false;

                // If there is any element after the iterator position, add a space (to humanize the expression).
                listIterator = listIterator.Next;
                if (listIterator != null && listIterator.Value != "")
                {
                    humanizedInput.Append(" ");
                }

            } while (listIterator != null);

            return humanizedInput.ToString();
        }

        /// <summary>
        /// Add the given string to the list of string to not humanize when calling the <see cref="Humanize"/> method.
        /// </summary>
        /// <param name="constString">The string to be declared as constant.</param>
        public static void AddConstString(string constString)
        {
            PrivateConstStrings.Add(constString);
        }

        #endregion
    }
}
