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
        /// Path to the file containing the template used for declaring fields.
        /// </summary>
        public string FieldDeclarationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring properties.
        /// </summary>
        public string PropertyDeclarationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for the "OnPropertyChanged" method,
        /// which is usually used to invoke the "PropertyChanged" event.
        /// </summary>
        public string PropertyChangedTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for creating the viewmodel's constructor.
        /// </summary>
        public string ConstructorTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring an "Execute" method for each ICommand property.
        /// </summary>
        public string ICommandExecuteTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring a "Can execute" method for each ICommand property that needs one.
        /// </summary>
        public string ICommandCanExecuteTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring an "Execute" method for each ICommand&lt;T&gt; property.
        /// </summary>
        public string ICommandTExecuteTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for declaring a "Can execute" method for each ICommand&lt;T&gt; property that needs one.
        /// </summary>
        public string ICommandTCanExecuteTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a property of type: <code>ICommand</code>
        /// </summary>
        public string ICommandPropertyDeclarationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a property of type: <code>ICommand&lt;T&gt;</code>
        /// </summary>
        public string ICommandTPropertyDeclarationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing an ICommand property with only an "Execute" method.
        /// </summary>
        public string ICommandActionInitializationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing an ICommand property with both an "Execute" and "CanExecute" methods.
        /// </summary>
        public string ICommandActionFuncBoolInitializationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a generic ICommand property with only an "Execute" method.
        /// </summary>
        public string ICommandTActionInitializationTemplatePath { get; set; }

        /// <summary>
        /// Path to the file containing the template used for initializing a generic ICommand property with both an "Execute" and "CanExecute" methods.
        /// </summary>
        public string ICommandTActionFuncBoolInitializationTemplatePath { get; set; }
    }
}
