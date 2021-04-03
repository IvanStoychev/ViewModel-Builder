using System.Collections.Generic;

namespace Backend.Utility
{
    /// <summary>
    /// Prepares templates for use.
    /// </summary>
    static class TemplateSeamster
    {
        /// <summary>
        /// Replaces all occurrences in the given <paramref name="template"/> of all keys in the given <paramref name="mergeFieldsAndValues"/> dictionary with their values.
        /// </summary>
        /// <param name="template">A template for use in the building of a view model with merge fields.</param>
        /// <param name="mergeFieldsAndValues">A dictionary with merge fields as keys and the values that should replace them as values.</param>
        /// <returns></returns>
        internal static string PrepareTemplate(string template, Dictionary<string, string> mergeFieldsAndValues)
        {
            foreach (var pair in mergeFieldsAndValues)
                template = template.Replace(pair.Key, pair.Value);

            return template;
        }
    }
}
