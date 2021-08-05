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
        /// The text to replace with the ViewModel's constructor.
        /// <code>Ex. "{ViewModel constructor}"</code>
        /// </summary>
        public string ClassConstructorMergeField { get; set; }

        /// <summary>
        /// The text to replace with the fields of the ViewModel.
        /// <code>Ex. "{ViewModel fields}"</code>
        /// </summary>
        public string ClassFieldsMergeField { get; set; }

        /// <summary>
        /// The text to replace with the ViewModel's ICommand declarations.
        /// <code>Ex. "{ICommand declarations}"</code>
        /// </summary>
        public string ClassICommandDeclarationsMergeField { get; set; }

        /// <summary>
        /// The text to replace with the ViewModel's ICommand methods ("OnExecute" and "CanExecute").
        /// <code>Ex. "{ICommand methods}"</code>
        /// </summary>
        public string ClassICommandMethodsMergeField { get; set; }

        /// <summary>
        /// The text to replace with the "OnChanged" event handler.
        /// <code>Ex. "{OnChanged event handler}"</code>
        /// </summary>
        public string ClassOnPropertyChangedMergeField { get; set; }

        /// <summary>
        /// The text to replace with the ViewModel's properties.
        /// <code>Ex. "{ViewModel properties}"</code>
        /// </summary>
        public string ClassPropertiesMergeField { get; set; }

        /// <summary>
        /// The text to replace with a field type.
        /// <code>Ex. "{Field name}"</code>
        /// </summary>
        public string FieldNameMergeField { get; set; }

        /// <summary>
        /// The text to replace with all command initializations.
        /// <code>Ex. "{ICommand initializations}"</code>
        /// </summary>
        public string ICommandInitializationsMergeField { get; set; }

        /// <summary>
        /// The text to replace with the name of the type that implements ICommand.
        /// <code>Ex. "{ICommand implementation type}"</code>
        /// </summary>
        public string ICommandImplementationTypeMergeField { get; set; }

        /// <summary>
        /// The text to replace with a command property's name.
        /// <code>Ex. "{ICommand name}"</code>
        /// </summary>
        public string ICommandNameMergeField { get; set; }

        /// <summary>
        /// The text to replace with a property's name.
        /// <code>Ex. "{Property name}"</code>
        /// </summary>
        public string PropertyNameMergeField { get; set; }

        /// <summary>
        /// The text to replace with a property type.
        /// <code>Ex. "{Property type}"</code>
        /// </summary>
        public string PropertyTypeMergeField { get; set; }

        /// <summary>
        /// The text to replace with a generic type parameter.
        /// <code>Ex. "{Type parameter}"</code>
        /// </summary>
        public string TypeParameterMergeField { get; set; }

        /// <summary>
        /// The text to replace with the name of the viewmodel.
        /// <code>Ex. "{ViewModel name}"</code>
        /// </summary>
        public string ViewModelNameMergeField { get; set; }
    }
}
