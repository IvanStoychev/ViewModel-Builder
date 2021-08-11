using Backend.Options;
using Backend.Utility;
using IvanStoychev.StringExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ViewModel_Builder.Utility;

namespace Backend
{
    /// <summary>
    /// Builds the contents of the view model from given data.
    /// </summary>
    public static class ViewModelBuilder
    {
        // This project breaks all social norms and expectations by using capitalized field names.
        // In this case I prefer it this way.

        #region Templates

        /// <summary>
        /// Template used for creating the ViewModel's class.
        /// </summary>
        static readonly string Class_Template = File.ReadAllText(TemplateCatalog.instance.Class_TemplatePath);

        /// <summary>
        /// Template used for creating the ViewModel's constructor.
        /// </summary>
        static readonly string Constructor_Template = File.ReadAllText(TemplateCatalog.instance.Constructor_TemplatePath);

        /// <summary>
        /// Template used for creating a field declaration.
        /// <code>ex. "string {FieldName};"</code>
        /// </summary>
        static readonly string Field_Declaration_Template = File.ReadAllText(TemplateCatalog.instance.Field_Declaration_TemplatePath);
        
        /// <summary>
        /// Template used to declare a property of the type, implementing the ICommand interface.
        /// <code>ex. "public {ICommandType} {CommandName} { get; }"</code>
        /// </summary>
        static readonly string ICommand_PropDeclaration_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_PropertyDeclaration_TemplatePath);

        /// <summary>
        /// Template used to declare a generic property of the type, implementing the ICommand interface.
        /// <code>ex. "public {ICommandType}&lt;{TypeParam}&gt; {CommandName} { get; }"</code>
        /// </summary>
        static readonly string ICommand_T_PropDeclaration_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_T_PropertyDeclaration_TemplatePath);

        /// <summary>
        /// Template used to initialize an ICommand property with only an "Execute" method.
        /// <code>ex. "{CommandName} = new {ICommandType}(On{CommandName});"</code>
        /// </summary>
        static readonly string ICommand_Action_Init_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_Action_Initialization_TemplatePath);

        /// <summary>
        /// Template used to initialize an ICommand property with both an "Execute" and "CanExecute" methods.
        /// <code>ex. "{CommandName} = new {ICommandType}(On{CommandName}, Can{CommandName});"</code>
        /// </summary>
        static readonly string ICommand_ActionFuncBool_Init_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_ActionFuncBool_Initialization_TemplatePath);

        /// <summary>
        /// Template used to initialize a ICommand&lt;T&gt; property with only an "Execute" method.
        /// <code>ex. "{CommandName} = new {ICommandType}&lt;{Type parameter}&gt;(On{CommandName});"</code>
        /// </summary>
        static readonly string ICommand_T_Action_Init_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_T_Action_Initialization_TemplatePath);

        /// <summary>
        /// Template used to initialize a ICommand&lt;T&gt; property with both an "Execute" and "CanExecute" methods.
        /// <code>ex. "{CommandName} = new {ICommandType}&lt;{Type parameter}&gt;(On{CommandName}, Can{CommandName});"</code>
        /// </summary>
        static readonly string ICommand_T_ActionFuncBool_Init_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_T_ActionFuncBool_Initialization_TemplatePath);

        /// <summary>
        /// Template used for an ICommand "OnExecute" method.
        /// <code>ex. "void On{CommandName}()"</code>
        /// </summary>
        static readonly string ICommand_OnExecute_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_OnExecute_TemplatePath);

        /// <summary>
        /// Template used for an ICommand "CanExecute" method.
        /// <code>ex. "bool Can{CommandName}()"</code>
        /// </summary>
        static readonly string ICommand_CanExecute_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_CanExecute_TemplatePath);

        /// <summary>
        /// Template used for a ICommand&lt;T&gt; "Execute" method.
        /// <code>ex. "void On{CommandName}({Type parameter} obj)"</code>
        /// </summary>
        static readonly string ICommand_T_OnExecute_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_T_OnExecute_TemplatePath);

        /// <summary>
        /// Template used for a ICommand&lt;T&gt; "CanExecute" method.
        /// <code>ex. "bool Can{CommandName}({Type parameter} obj)"</code>
        /// </summary>
        static readonly string ICommand_T_CanExecute_Template = File.ReadAllText(TemplateCatalog.instance.ICommand_T_CanExecute_TemplatePath);

        /// <summary>
        /// Template used for creating a property declaration.
        /// <code>ex. "public string {PropName} { get => {FieldName}; set => {FieldName} = value; }"</code>
        /// </summary>
        static readonly string Property_Declaration_Template = File.ReadAllText(TemplateCatalog.instance.Property_Declaration_TemplatePath);

        /// <summary>
        /// Template used for the "OnPropertyChanged" method.
        /// <code>ex. "void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(...);"</code>
        /// </summary>
        static readonly string PropertyChanged_Template = File.ReadAllText(TemplateCatalog.instance.PropertyChanged_TemplatePath);

        #endregion Templates

        #region Merge fields

        /// <summary>
        /// Text to replace with the ViewModel's constructor.
        /// <code>Ex. "{ViewModel constructor}"</code>
        /// </summary>
        static readonly string Class_Constructor_MergeField = MergeFieldCatalog.instance.Class_Constructor_MergeField;

        /// <summary>
        /// Text to replace with the fields of the ViewModel.
        /// <code>Ex. "{ViewModel fields}"</code>
        /// </summary>
        static readonly string Class_Fields_MergeField = MergeFieldCatalog.instance.Class_Fields_MergeField;

        /// <summary>
        /// Text to replace with the ViewModel's ICommand declarations.
        /// <code>Ex. "{ICommand declarations}"</code>
        /// </summary>
        static readonly string Class_ICommand_Declarations_MergeField = MergeFieldCatalog.instance.Class_ICommand_Declarations_MergeField;

        /// <summary>
        /// Text to replace with the ViewModel's ICommand methods ("OnExecute" and "CanExecute").
        /// <code>Ex. "{ICommand methods}"</code>
        /// </summary>
        static readonly string Class_ICommand_Methods_MergeField = MergeFieldCatalog.instance.Class_ICommand_Methods_MergeField;

        /// <summary>
        /// Text to replace with the "OnChanged" event handler.
        /// <code>Ex. "{OnChanged EventHandler}"</code>
        /// </summary>
        static readonly string Class_OnPropertyChanged_MergeField = MergeFieldCatalog.instance.Class_OnPropertyChanged_MergeField;

        /// <summary>
        /// Text to replace with the ViewModel's properties.
        /// <code>Ex. "{ViewModel properties}"</code>
        /// </summary>
        static readonly string Class_Properties_MergeField = MergeFieldCatalog.instance.Class_Properties_MergeField;

        /// <summary>
        /// Text to replace with a field's name.
        /// <code>ex. "{FieldName}"</code>
        /// </summary>
        static readonly string Field_Name_MergeField = MergeFieldCatalog.instance.Field_Name_MergeField;

        /// <summary>
        /// Text to replace with a property's name.
        /// <code>ex. "{PropertyName}"</code>
        /// </summary>
        static readonly string Property_Name_MergeField = MergeFieldCatalog.instance.Property_Name_MergeField;

        /// <summary>
        /// Text to replace with a property's type.
        /// <code>ex. "{PropertyType}"</code>
        /// </summary>
        static readonly string Property_Type_MergeField = MergeFieldCatalog.instance.Property_Type_MergeField;

        /// <summary>
        /// Text to replace with a ViewModel's name.
        /// <code>ex. "{ViewModelName}"</code>
        /// </summary>
        static readonly string ViewModel_Name_MergeField = MergeFieldCatalog.instance.ViewModel_Name_MergeField;

        /// <summary>
        /// Text to replace with a generic type parameter.
        /// <code>Ex. "{Type parameter}"</code>
        /// </summary>
        static readonly string TypeParameter_MergeField = MergeFieldCatalog.instance.TypeParameter_MergeField;

        #endregion Merge fields

        #region ICommand data

        /// <summary>
        /// Text to replace with the initializations of all ICommand properties.
        /// <code>Ex. "{ICommand initializations}"</code>
        /// </summary>
        static readonly string Class_ICommand_Initializations_MergeField = MergeFieldCatalog.instance.ICommand_Initializations_MergeField;

        /// <summary>
        /// Text to replace with a command's name.
        /// <code>ex. "{CommandName}"</code>
        /// </summary>
        static readonly string ICommand_Name_MergeField = MergeFieldCatalog.instance.ICommand_Name_MergeField;

        /// <summary>
        /// Text to replace with an ICommand's implementing type (like "RelayCommand").
        /// <code>ex. "{CommandType}"</code>
        /// </summary>
        static readonly string ICommand_ImplementationType_MergeField = MergeFieldCatalog.instance.ICommand_ImplementationType_MergeField;

        /// <summary>
        /// The data type used for the <see cref="System.Windows.Input.ICommand"/> implementation.
        /// <code>ex. "RelayCommand"</code>
        /// </summary>
        static readonly string ICommand_ImplementationType = TypeData.instance.ICommand_ImplementationType;
        
        #endregion ICommand data

        /// <summary>
        /// Replaces merge fields in property templates, defined in the appsettings file, to construct the code of the view model.
        /// Then opens a Notepad and send it there.
        /// </summary>
        /// <param name="propertyNameAndTypeList">A list of property names and their data types.</param>
        /// <param name="viewModelName">What to name the ViewModel class.</param>
        public static void GenerateViewModelText(List<(string Name, string Type)> propertyNameAndTypeList, string viewModelName)
        {
            // Prepare the specific class used for any ICommand implementations for replacing in any templates.
            TemplateSeamster.MergeFieldsAndValues[ICommand_ImplementationType_MergeField] = ICommand_ImplementationType;
            // Prepare the "OnChanged" EventHandler for replacing in the template for the ViewModel class.
            TemplateSeamster.MergeFieldsAndValues[Class_OnPropertyChanged_MergeField] = PropertyChanged_Template;

            // Regexes to capture a generic ICommand.
            Regex genericActionCommandRegex = new Regex(@$"{ICommand_ImplementationType}<(.*)>\(Action<.*>\)", RegexOptions.Compiled);
            Regex genericActionFuncCommandRegex = new Regex(@$"{ICommand_ImplementationType}<(.*)>\(Action<.*>, Func<.*, bool>\)", RegexOptions.Compiled);

            #region StringBuilders initialization

            StringBuilder fieldDeclarationsSB = new();
            StringBuilder propertyDeclarationsSB = new();
            StringBuilder iCommandPropDeclarationSB = new();
            StringBuilder iCommandInitializationsSB = new();
            StringBuilder iCommandMethodsSB = new();
            
            #endregion StringBuilders initialization

            viewModelName = ValidateViewModelName(viewModelName);
            TemplateSeamster.MergeFieldsAndValues[ViewModel_Name_MergeField] = viewModelName;
            
            // Build all fields and properties (including commands).
            foreach (var property in propertyNameAndTypeList)
            {
                string propertyType = property.Type;
                string fieldName = property.Name.FirstLetterToLower();
                string propertyName = property.Name.FirstLetterToUpper();

                // If the property is an ICommand - build its declaration, initialization and methods.
                if (propertyType.StartsWith(ICommand_ImplementationType, StringComparison.InvariantCulture))
                {
                    TemplateSeamster.MergeFieldsAndValues[ICommand_Name_MergeField] = propertyName;

                    if (propertyType == $"{ICommand_ImplementationType}(Action)")
                    {
                        AddCommand_Action(iCommandPropDeclarationSB, iCommandInitializationsSB, iCommandMethodsSB);
                    }
                    else if (propertyType == $"{ICommand_ImplementationType}(Action, Func<bool>)")
                    {
                        AddCommand_ActionFunc(iCommandPropDeclarationSB, iCommandInitializationsSB, iCommandMethodsSB);
                    }
                    else if (genericActionCommandRegex.IsMatch(propertyType))
                    {
                        string genericType = genericActionCommandRegex.Matches(propertyType)[0].Groups[1].Value;
                        AddCommand_TAction(iCommandPropDeclarationSB, iCommandInitializationsSB, iCommandMethodsSB, genericType);
                    }
                    else if (genericActionFuncCommandRegex.IsMatch(propertyType))
                    {
                        string genericType = genericActionCommandRegex.Matches(propertyType)[0].Groups[1].Value;
                        AddCommand_TActionFunc(iCommandPropDeclarationSB, iCommandInitializationsSB, iCommandMethodsSB, genericType);
                    }
                }
                else
                {
                    TemplateSeamster.MergeFieldsAndValues[Property_Name_MergeField] = propertyName;
                    TemplateSeamster.MergeFieldsAndValues[Property_Type_MergeField] = propertyType;
                    TemplateSeamster.MergeFieldsAndValues[Field_Name_MergeField] = fieldName;
                    string fieldDeclarationText = TemplateSeamster.PrepareTemplate(Field_Declaration_Template);
                    string propertyDeclarationText = TemplateSeamster.PrepareTemplate(Property_Declaration_Template);

                    fieldDeclarationsSB.AppendLine(fieldDeclarationText);
                    propertyDeclarationsSB.AppendLine(propertyDeclarationText);
                    propertyDeclarationsSB.AppendLine();
                }
            }

            iCommandPropDeclarationSB.AppendLine();

            string fieldDeclarationsString = fieldDeclarationsSB.ToString();
            string iCommandInitializationsString = iCommandInitializationsSB.ToString();
            string iCommandMethodsString = iCommandMethodsSB.ToString();
            string iCommandPropDeclarationString = iCommandPropDeclarationSB.ToString();
            string propertyDeclarationsString = propertyDeclarationsSB.ToString();

            fieldDeclarationsString = fieldDeclarationsString.TrimEnd(Environment.NewLine);
            iCommandInitializationsString = iCommandInitializationsString.TrimEnd(Environment.NewLine);
            iCommandMethodsString = iCommandMethodsString.TrimEnd(Environment.NewLine).TrimEnd(Environment.NewLine);
            iCommandPropDeclarationString = iCommandPropDeclarationString.TrimEnd(Environment.NewLine).TrimEnd(Environment.NewLine);
            propertyDeclarationsString = propertyDeclarationsString.TrimEnd(Environment.NewLine).TrimEnd(Environment.NewLine);

            TemplateSeamster.MergeFieldsAndValues[Class_Fields_MergeField] = fieldDeclarationsString;
            TemplateSeamster.MergeFieldsAndValues[Class_ICommand_Initializations_MergeField] = iCommandInitializationsString;
            TemplateSeamster.MergeFieldsAndValues[Class_ICommand_Methods_MergeField] = iCommandMethodsString;
            TemplateSeamster.MergeFieldsAndValues[Class_ICommand_Declarations_MergeField] = iCommandPropDeclarationString;
            TemplateSeamster.MergeFieldsAndValues[Class_Properties_MergeField] = propertyDeclarationsString;

            string constructorString = TemplateSeamster.PrepareTemplate(Constructor_Template);
            TemplateSeamster.MergeFieldsAndValues[Class_Constructor_MergeField] = constructorString;

            string result = TemplateSeamster.PrepareTemplate(Class_Template);

            Interop.ExportToNotepad(result);
        }

        /// <summary>
        /// Extracts properties and a ViewModel name from the XAML in the clipboard.
        /// </summary>
        /// <param name="xamlText">XAML of an MVVM View to extract data from.</param>
        public static void ExtractFromXAML(string xamlText)
        {
            string viewModelName = "";
            string propertyName, propertyType;
            bool gotViewModelName = false;
            List<(string Name, string Type)> propertyNameAndTypeList = new();
            string commandImplementationType = TypeData.instance.ICommand_ImplementationType;
            string[] splitText = xamlText.Split('<', StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            foreach (var line in splitText)
            {
                if (!gotViewModelName && line.Contains("Class="))
                {
                    viewModelName = line.Substring("Class=\"", "\"");
                    viewModelName = viewModelName.SubstringEnd(".");
                    viewModelName = viewModelName.TrimEnd("View");
                    viewModelName += "ViewModel";
                    gotViewModelName = true;
                }

                if (!line.StartsWith("/") && line.Contains("Binding"))
                {
                    propertyName = line.Substring("Binding ", "}");
                    if (propertyName.Contains(","))
                        propertyName = propertyName.SubstringStart(",");

                    string controlType = line.SubstringStart(" ");
                    switch (controlType)
                    {
                        case "TextBox":
                        case "TextBlock":
                            propertyType = "string";
                            break;
                        case "Button":
                            propertyType = commandImplementationType + "(Action, Func<bool>)";
                            break;
                        case "ItemsControl":
                            propertyType = "IEnumerable<T>";
                            break;
                        default:
                            propertyType = "unknown";
                            break;
                    }

                    propertyNameAndTypeList.Add((propertyName, propertyType));
                }
            }

            GenerateViewModelText(propertyNameAndTypeList, viewModelName);
        }

        /// <summary>
        /// Verifies if the given <paramref name="viewModelName"/> follows MVVM naming conventions and ends in "ViewModel" and fixes it, if it doesn't.
        /// </summary>
        /// <param name="viewModelName">Name of a ViewModel class.</param>
        /// <returns>The given <paramref name="viewModelName"/>, affixed with "ViewModel", if it wasn't already.</returns>
        static string ValidateViewModelName(string viewModelName)
        {
            if (!viewModelName.EndsWith("ViewModel"))
            {
                if (viewModelName.EndsWith("View"))
                    viewModelName += "Model";
                else
                    viewModelName += "ViewModel";
            }

            viewModelName = viewModelName.FirstLetterToUpper();
            return viewModelName;
        }

        /// <summary>
        /// Builds a declaration and initialization for an ICommand property with only an "OnExecute" method and adds that method to the given <see cref="StringBuilder"/>s.
        /// </summary>
        /// <param name="iCommandPropDeclarationSB"><see cref="StringBuilder"/> used for ICommand property declarations.</param>
        /// <param name="iCommandInitializationsSB"><see cref="StringBuilder"/> used for ICommand initializations.</param>
        /// <param name="iCommandMethodsSB"><see cref="StringBuilder"/> used for ICommand "OnExecute" methods.</param>
        static void AddCommand_Action(StringBuilder iCommandPropDeclarationSB, StringBuilder iCommandInitializationsSB, StringBuilder iCommandMethodsSB)
        {
            // Add a declaration of an ICommand property.
            string commandPropertyDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_PropDeclaration_Template);
            iCommandPropDeclarationSB.AppendLine(commandPropertyDeclarationString);

            // Add initialization for the declared property.
            string commandInitializationString = TemplateSeamster.PrepareTemplate(ICommand_Action_Init_Template);
            iCommandInitializationsSB.AppendLine(commandInitializationString);

            // Add an "OnExecute" method for the ICommand.
            string commandExecuteMethodDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_OnExecute_Template);
            iCommandMethodsSB.AppendLine(commandExecuteMethodDeclarationString);
            iCommandMethodsSB.AppendLine();
        }

        /// <summary>
        /// Builds a declaration and initialization for an ICommand property with "OnExecute" and "CanExecute" methods and adds those methods to the given <see cref="StringBuilder"/>s.
        /// </summary>
        /// <param name="iCommandPropDeclarationSB"><see cref="StringBuilder"/> used for ICommand property declarations.</param>
        /// <param name="iCommandInitializationsSB"><see cref="StringBuilder"/> used for ICommand initializations.</param>
        /// <param name="iCommandMethodsSB"><see cref="StringBuilder"/> used for ICommand "OnExecute" methods.</param>
        static void AddCommand_ActionFunc(StringBuilder iCommandPropDeclarationSB, StringBuilder iCommandInitializationsSB, StringBuilder iCommandMethodsSB)
        {
            // Add a declaration of an ICommand property.
            string commandPropertyDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_PropDeclaration_Template);
            iCommandPropDeclarationSB.AppendLine(commandPropertyDeclarationString);

            // Add initialization for the declared property.
            string commandInitializationString = TemplateSeamster.PrepareTemplate(ICommand_ActionFuncBool_Init_Template);
            iCommandInitializationsSB.AppendLine(commandInitializationString);

            // Add an "OnExecute" method for the ICommand.
            string commandExecuteMethodDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_OnExecute_Template);
            iCommandMethodsSB.AppendLine(commandExecuteMethodDeclarationString);
            iCommandMethodsSB.AppendLine();

            // Add an "CanExecute" method for the ICommand.
            string commandCanExecuteMethodDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_CanExecute_Template);
            iCommandMethodsSB.AppendLine(commandCanExecuteMethodDeclarationString);
            iCommandMethodsSB.AppendLine();
        }

        /// <summary>
        /// Builds a declaration and initialization for a generic ICommand property with an "OnExecute" method and adds that method to the given <see cref="StringBuilder"/>s.
        /// </summary>
        /// <param name="iCommandPropDeclarationSB"><see cref="StringBuilder"/> used for ICommand property declarations.</param>
        /// <param name="iCommandInitializationsSB"><see cref="StringBuilder"/> used for ICommand initializations.</param>
        /// <param name="iCommandMethodsSB"><see cref="StringBuilder"/> used for ICommand "OnExecute" methods.</param>
        /// <param name="genericType">The type parameter for the generic ICommand.</param>
        static void AddCommand_TAction(StringBuilder iCommandPropDeclarationSB, StringBuilder iCommandInitializationsSB, StringBuilder iCommandMethodsSB, string genericType)
        {
            string typeParameterMergeField = MergeFieldCatalog.instance.TypeParameter_MergeField;
            TemplateSeamster.MergeFieldsAndValues[typeParameterMergeField] = genericType;

            // Add a declaration of a generic ICommand property.
            string commandPropertyDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_T_PropDeclaration_Template);
            iCommandPropDeclarationSB.AppendLine(commandPropertyDeclarationString);

            // Add initialization for the declared property.
            string commandInitializationString = TemplateSeamster.PrepareTemplate(ICommand_T_Action_Init_Template);
            iCommandInitializationsSB.AppendLine(commandInitializationString);

            // Add an "OnExecute" method for the generic ICommand.
            string commandExecuteMethodDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_T_OnExecute_Template);
            iCommandMethodsSB.AppendLine(commandExecuteMethodDeclarationString);
            iCommandMethodsSB.AppendLine();
        }

        /// <summary>
        /// Builds a declaration and initialization for a generic ICommand property with "OnExecute" and "CanExecute" methods and adds those methods to the given <see cref="StringBuilder"/>s.
        /// </summary>
        /// <param name="iCommandPropDeclarationSB"><see cref="StringBuilder"/> used for ICommand property declarations.</param>
        /// <param name="iCommandInitializationsSB"><see cref="StringBuilder"/> used for ICommand initializations.</param>
        /// <param name="iCommandMethodsSB"><see cref="StringBuilder"/> used for ICommand "OnExecute" methods.</param>
        /// <param name="genericType">The type parameter for the generic ICommand.</param>
        static void AddCommand_TActionFunc(StringBuilder iCommandPropDeclarationSB, StringBuilder iCommandInitializationsSB, StringBuilder iCommandMethodsSB, string genericType)
        {
            string typeParameterMergeField = MergeFieldCatalog.instance.TypeParameter_MergeField;
            TemplateSeamster.MergeFieldsAndValues[typeParameterMergeField] = genericType;

            // Add a declaration of a generic ICommand property.
            string commandPropertyDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_T_PropDeclaration_Template);
            iCommandPropDeclarationSB.AppendLine(commandPropertyDeclarationString);

            // Add initialization for the declared property.
            string commandInitializationString = TemplateSeamster.PrepareTemplate(ICommand_T_Action_Init_Template);
            iCommandInitializationsSB.AppendLine(commandInitializationString);

            // Add an "OnExecute" method for the generic ICommand.
            string commandExecuteMethodDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_T_OnExecute_Template);
            iCommandMethodsSB.AppendLine(commandExecuteMethodDeclarationString);
            iCommandMethodsSB.AppendLine();

            // Add an "CanExecute" method for the generic ICommand.
            string commandCanExecuteMethodDeclarationString = TemplateSeamster.PrepareTemplate(ICommand_T_CanExecute_Template);
            iCommandMethodsSB.AppendLine(commandCanExecuteMethodDeclarationString);
            iCommandMethodsSB.AppendLine();
        }
    }
}
