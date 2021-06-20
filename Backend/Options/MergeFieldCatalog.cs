using System;

namespace Backend.Options
{
    /// <summary>
    /// Contains the locations of all text files being used as templates by the program.
    /// </summary>
    class MergeFieldCatalog
    {
        static readonly Lazy<MergeFieldCatalog> lazy = new Lazy<MergeFieldCatalog>(() => new MergeFieldCatalog());

        /// <summary>
        /// Reference to the singleton instance of this class.
        /// </summary>
        internal static readonly MergeFieldCatalog instance = lazy.Value;

        MergeFieldCatalog() { }

        /// <summary>
        /// The text to replace with a field type.
        /// </summary>
        public string FieldNameMergeField { get; set; }

        /// <summary>
        /// The text to replace with all command initializations.
        /// </summary>
        public string ICommandInitializationsMergeField { get; set; }

        /// <summary>
        /// The text to replace with the name of the type that implements ICommand.
        /// </summary>
        public string ICommandImplementationTypeMergeField { get; set; }

        /// <summary>
        /// The text to replace with a command property's name.
        /// </summary>
        public string ICommandNameMergeField { get; set; }

        /// <summary>
        /// The text to replace with a property's name.
        /// </summary>
        public string PropertyNameMergeField { get; set; }

        /// <summary>
        /// The text to replace with a property type.
        /// </summary>
        public string PropertyTypeMergeField { get; set; }

        /// <summary>
        /// The text to replace with a generic type parameter.
        /// </summary>
        public string TypeParameterMergeField { get; set; }

        /// <summary>
        /// The text to replace with the code of the viewmodel.
        /// </summary>
        public string ViewModelCodeMergeField { get; set; }

        /// <summary>
        /// The text to replace with the name of the viewmodel.
        /// </summary>
        public string ViewModelNameMergeField { get; set; }
    }
}
