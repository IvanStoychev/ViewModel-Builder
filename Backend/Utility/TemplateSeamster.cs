using Backend.Options;
using System.Collections.Generic;
using System.Reflection;

namespace Backend.Utility
{
    /// <summary>
    /// Prepares templates for use.
    /// </summary>
    static class TemplateSeamster
    {
        /// <summary>
        /// A collection of all merge fields used by the application and the values they should be replaced with.
        /// </summary>
        internal static Dictionary<string, string> MergeFieldsAndValues = new();

        static TemplateSeamster()
        {
            var allMergeFields = MergeFieldCatalog.instance.GetType().GetProperties(BindingFlags.Public);
            
            foreach (var mergeField in allMergeFields)
            {
                MergeFieldsAndValues.Add(mergeField.GetValue(MergeFieldCatalog.instance).ToString(), "");
            }
        }

        /// <summary>
        /// Returns the given <paramref name="template"/> with all merge fields replaced.
        /// </summary>
        /// <param name="template">A template for use in the building of a view model with merge fields.</param>
        /// <returns>A string representing the given template, filled with actual values.</returns>
        internal static string PrepareTemplate(string template)
        {
            foreach (var pair in MergeFieldsAndValues)
                template = template.Replace(pair.Key, pair.Value);

            return template;
        }
    }
}
