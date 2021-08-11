using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Backend.Options
{
    /// <summary>
    /// Contains information about the types the program supports for building a viewmodel.
    /// </summary>
    public class TypeData
    {
        static readonly Lazy<TypeData> lazy = new Lazy<TypeData>(() => new TypeData());

        /// <summary>
        /// Reference to the singleton instance of this class.
        /// </summary>
        public static readonly TypeData instance = lazy.Value;

        /// <summary>
        /// The data type used for the <see cref="ICommand"/> implementation.
        /// </summary>
        public string ICommand_ImplementationType { get; set; }

        /// <summary>
        /// List of all data types the program can work with.
        /// </summary>
        public List<string> PropertyTypes { get; set; }

        TypeData() { }

        /// <summary>
        /// Replaces the default "ICommand" type text in the "PropertyTypes" list with the implementation defined in appsettings.json.
        /// </summary>
        public static void ReplaceICommandType()
        {
            for (int i = 0; i < instance.PropertyTypes.Count; i++)
                if (instance.PropertyTypes[i].StartsWith("ICommand"))
                    instance.PropertyTypes[i] = instance.PropertyTypes[i].Replace("ICommand", instance.ICommand_ImplementationType);
        }
    }
}
