using System.Linq;

namespace ViewModel_Builder.Utility
{
    /// <summary>
    /// Contains all custom extension methods for this project.
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Returns the input string with its first letter in upper case.
        /// </summary>
        /// <param name="str">The string whose first letter to set in upper case.</param>
        /// <returns>The input string with its first letter in upper case.</returns>
        public static string FirstLetterToUpper(this string str)
        {
            string firstLetterToUpper = str.First().ToString().ToUpper() + str.Substring(1);

            return firstLetterToUpper;
        }

        /// <summary>
        /// Returns the input string with its first letter in lower case.
        /// </summary>
        /// <param name="str">The string whose first letter to set in lower case.</param>
        /// <returns>The input string with its first letter in lower case.</returns>
        public static string FirstLetterToLower(this string str)
        {
            string firstLetterToLower = str.First().ToString().ToLower() + str.Substring(1);

            return firstLetterToLower;
        }
    }
}
