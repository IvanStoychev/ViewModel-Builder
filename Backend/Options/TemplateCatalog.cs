using System;

namespace Backend.Options
{
    /// <summary>
    /// Contains the locations of all text files being used as templates by the program.
    /// </summary>
    class TemplateCatalog
    {
        static readonly Lazy<TemplateCatalog> lazy = new Lazy<TemplateCatalog>(() => new TemplateCatalog());

        /// <summary>
        /// Reference to the singleton instance of this class.
        /// </summary>
        internal static readonly TemplateCatalog instance = lazy.Value;

        TemplateCatalog() { }

        /// <summary>
        /// Path to the file containing the template used for creating the viewmodel's class.
        /// </summary>
        public string Class_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for creating the viewmodel's constructor.
        /// </summary>
        public string Constructor_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring fields.
        /// </summary>
        public string Field_Declaration_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring an "Execute" method for each ICommand property.
        /// </summary>
        public string ICommand_OnExecute_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring a "Can execute" method for each ICommand property that needs one.
        /// </summary>
        public string ICommand_CanExecute_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring an "Execute" method for each ICommand&lt;T&gt; property.
        /// </summary>
        public string ICommand_T_OnExecute_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring a "Can execute" method for each ICommand&lt;T&gt; property that needs one.
        /// </summary>
        public string ICommand_T_CanExecute_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a property of type: <code>ICommand</code>
        /// </summary>
        public string ICommand_PropertyDeclaration_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a property of type: <code>ICommand&lt;T&gt;</code>
        /// </summary>
        public string ICommand_T_PropertyDeclaration_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing an ICommand property with only an "Execute" method.
        /// </summary>
        public string ICommand_Action_Initialization_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing an ICommand property with both an "Execute" and "CanExecute" methods.
        /// </summary>
        public string ICommand_ActionFuncBool_Initialization_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a generic ICommand property with only an "Execute" method.
        /// </summary>
        public string ICommand_T_Action_Initialization_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a generic ICommand property with both an "Execute" and "CanExecute" methods.
        /// </summary>
        public string ICommand_T_ActionFuncBool_Initialization_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring properties.
        /// </summary>
        public string Property_Declaration_TemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for the "OnPropertyChanged" method,
        /// which is usually used to invoke the "PropertyChanged" event.
        /// </summary>
        public string PropertyChanged_TemplatePath { get; set; }
    }
}
