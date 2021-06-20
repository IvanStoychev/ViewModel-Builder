using Backend.Options;
using Backend.Utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ViewModel_Builder.Utility;
using IvanStoychev.StringExtensions;

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
        static readonly string ClassTemplate = File.ReadAllText(TemplateCatalog.instance.ClassTemplatePath);

        /// <summary>
        /// Template used for creating the ViewModel's constructor.
        /// </summary>
        static readonly string ConstructorTemplate = File.ReadAllText(TemplateCatalog.instance.ConstructorTemplatePath);

        /// <summary>
        /// Template used to declare a property of the type, implementing the ICommand interface.
        /// <code>ex. "public {ICommandType} {CommandName} { get; }"</code>
        /// </summary>
        static readonly string CommandPropDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandPropertyDeclarationTemplatePath);

        /// <summary>
        /// Template used to declare a generic property of the type, implementing the ICommand interface.
        /// <code>ex. "public {ICommandType}&lt;{TypeParam}&gt; {CommandName} { get; }"</code>
        /// </summary>
        static readonly string CommandTPropDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandTPropertyDeclarationTemplatePath);

        /// <summary>
        /// Template used to initialize an ICommand property with only an "Execute" method.
        /// <code>ex. "{CommandName} = new {ICommandType}(On{CommandName});"</code>
        /// </summary>
        static readonly string CommandActionInitTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandActionInitializationTemplatePath);

        /// <summary>
        /// Template used to initialize an ICommand property with both an "Execute" and "CanExecute" methods.
        /// <code>ex. "{CommandName} = new {ICommandType}(On{CommandName}, Can{CommandName});"</code>
        /// </summary>
        static readonly string CommandActionFuncBoolInitTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandActionFuncBoolInitializationTemplatePath);

        /// <summary>
        /// Template used to initialize a ICommand&lt;T&gt; property with only an "Execute" method.
        /// <code>ex. "{CommandName} = new {ICommandType}&lt;{TypeParameter}&gt;(On{CommandName});"</code>
        /// </summary>
        static readonly string CommandTActionInitTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandTActionInitializationTemplatePath);

        /// <summary>
        /// Template used to initialize a ICommand&lt;T&gt; property with both an "Execute" and "CanExecute" methods.
        /// <code>ex. "{CommandName} = new {ICommandType}&lt;{TypeParameter}&gt;(On{CommandName}, Can{CommandName});"</code>
        /// </summary>
        static readonly string CommandTActionFuncBoolInitTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandTActionFuncBoolInitializationTemplatePath);

        /// <summary>
        /// Template used for an ICommand "Execute" method.
        /// <code>ex. "void On{CommandName}()"</code>
        /// </summary>
        static readonly string CommandExecDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandExecuteTemplatePath);

        /// <summary>
        /// Template used for an ICommand "CanExecute" method.
        /// <code>ex. "bool Can{CommandName}()"</code>
        /// </summary>
        static readonly string CommandCanExecDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandCanExecuteTemplatePath);

        /// <summary>
        /// Template used for a ICommand&lt;T&gt; "Execute" method.
        /// <code>ex. "void On{CommandName}({TypeParameter} obj)"</code>
        /// </summary>
        static readonly string CommandTExecDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandTExecuteTemplatePath);

        /// <summary>
        /// Template used for a ICommand&lt;T&gt; "CanExecute" method.
        /// <code>ex. "bool Can{CommandName}({TypeParameter} obj)"</code>
        /// </summary>
        static readonly string CommandTCanExecDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.ICommandTCanExecuteTemplatePath);

        /// <summary>
        /// Template used for creating a field declaration.
        /// <code>ex. "string {FieldName};"</code>
        /// </summary>
        static readonly string FieldDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.FieldDeclarationTemplatePath);

        /// <summary>
        /// Template used for creating a property declaration.
        /// <code>ex. "public string {PropName} { get => {FieldName}; set => {FieldName} = value; }"</code>
        /// </summary>
        static readonly string PropertyDeclarationTemplate = File.ReadAllText(TemplateCatalog.instance.PropertyDeclarationTemplatePath);

        /// <summary>
        /// Template used for the "OnPropertyChanged" method.
        /// <code>ex. "void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(...);"</code>
        /// </summary>
        static readonly string PropertyChangedTemplate = File.ReadAllText(TemplateCatalog.instance.PropertyChangedTemplatePath);
        #endregion Templates

        #region Merge fields
        /// <summary>
        /// Text to replace with a field's name.
        /// <code>ex. "{FieldName}"</code>
        /// </summary>
        static readonly string FieldNameMergeField = MergeFieldCatalog.instance.FieldNameMergeField;

        /// <summary>
        /// Text to replace with a property's name.
        /// <code>ex. "{PropertyName}"</code>
        /// </summary>
        static readonly string PropertyNameMergeField = MergeFieldCatalog.instance.PropertyNameMergeField;

        /// <summary>
        /// Text to replace with a property's type.
        /// <code>ex. "{PropertyType}"</code>
        /// </summary>
        static readonly string PropertyTypeMergeField = MergeFieldCatalog.instance.PropertyTypeMergeField;

        /// <summary>
        /// Text to replace with a ViewModel's code.
        /// <code>ex. "{ViewModelCode}"</code>
        /// </summary>
        static readonly string ViewModelCodeMergeField = MergeFieldCatalog.instance.ViewModelCodeMergeField;

        /// <summary>
        /// Text to replace with a ViewModel's name.
        /// <code>ex. "{ViewModelName}"</code>
        /// </summary>
        static readonly string ViewModelNameMergeField = MergeFieldCatalog.instance.ViewModelNameMergeField;
        #endregion Merge fields

        #region ICommand data
        /// <summary>
        /// Text to replace with a command's name.
        /// <code>ex. "{CommandName}"</code>
        /// </summary>
        static readonly string CommandNameMergeField = MergeFieldCatalog.instance.ICommandNameMergeField;

        /// <summary>
        /// Text to replace with an ICommand's implementing type (like "RelayCommand").
        /// <code>ex. "{CommandType}"</code>
        /// </summary>
        static readonly string CommandImplementationTypeMergeField = MergeFieldCatalog.instance.ICommandImplementationTypeMergeField;

        /// <summary>
        /// The data type used for the <see cref="System.Windows.Input.ICommand"/> implementation.
        /// <code>ex. "RelayCommand"</code>
        /// </summary>
        static readonly string CommandImplementationType = TypeData.instance.ICommandImplementationType;
        #endregion ICommand data

        /// <summary>
        /// Replaces merge fields in property templates, defined in the appsettings file, to construct the code of the view model.
        /// Then opens a Notepad and send it there.
        /// </summary>
        /// <param name="propertyNameAndTypeList">A list of property names and their data types.</param>
        /// <param name="viewModelName">What to name the ViewModel class.</param>
        public static void GenerateViewModelText(List<(string Name, string Type)> propertyNameAndTypeList, string viewModelName)
        {
            TemplateSeamster.MergeFieldsAndValues[ViewModelCodeMergeField] = ViewModelCodeMergeField;
            TemplateSeamster.MergeFieldsAndValues[CommandImplementationTypeMergeField] = CommandImplementationType;

            // Regexes to capture the generic type.
            // For some reason Visual Studio does not recognize the "\(" escape sequence and throws a CS1009 error,
            // which is why the escaping is handled the following way.
            string escapedActionCommandRegex = Regex.Escape("(Action<T>)");
            string escapedActionFuncCommandRegex = Regex.Escape("(Action<T>, Func<T, bool>)");
            Regex genericActionCommandRegex = new Regex($"{CommandImplementationType}<(.*)>{escapedActionCommandRegex}", RegexOptions.Compiled);
            Regex genericActionFuncCommandRegex = new Regex($"{CommandImplementationType}<(.*)>{escapedActionFuncCommandRegex}", RegexOptions.Compiled);

            StringBuilder fieldDeclarationsSB = new();
            StringBuilder propertyDeclarationsSB = new();
            StringBuilder iCommandPropertiesSB = new();
            StringBuilder iCommandInitialisationsSB = new();
            StringBuilder iCommandMethodsSB = new();

            Regex regexEndsWithViewModel = new Regex("ViewModel$", RegexOptions.IgnoreCase);
            Regex regexEndsWithView = new Regex("View$", RegexOptions.IgnoreCase);

            if (!regexEndsWithViewModel.IsMatch(viewModelName))
            {
                if (regexEndsWithView.IsMatch(viewModelName))
                    viewModelName += "Model";
                else
                    viewModelName += "ViewModel";
            }

            viewModelName = viewModelName.FirstLetterToUpper();
            TemplateSeamster.MergeFieldsAndValues[ViewModelNameMergeField] = viewModelName;

            foreach (var property in propertyNameAndTypeList)
            {
                string propertyType = property.Type;
                string fieldName = property.Name.FirstLetterToLower();
                string propertyName = property.Name.FirstLetterToUpper();

                // ICommand builder.
                if (propertyType.StartsWith(CommandImplementationType))
                {
                    TemplateSeamster.MergeFieldsAndValues[CommandNameMergeField] = propertyName;

                    string commandInitializationText = "";
                    string commandPropertyDeclarationText;
                    string commandExecuteMethodDeclaration;
                    string commandCanExecuteMethodDeclaration;

                    if (propertyType == $"{CommandImplementationType}(Action)")
                    {
                        commandPropertyDeclarationText = TemplateSeamster.PrepareTemplate(CommandPropDeclarationTemplate);
                        iCommandPropertiesSB.AppendLine(commandPropertyDeclarationText);
                        commandInitializationText = TemplateSeamster.PrepareTemplate(CommandActionInitTemplate);
                        commandExecuteMethodDeclaration = TemplateSeamster.PrepareTemplate(CommandExecDeclarationTemplate);
                        iCommandMethodsSB.AppendLine(commandExecuteMethodDeclaration);
                        iCommandMethodsSB.AppendLine();
                    }
                    else if (propertyType == $"{CommandImplementationType}(Action, Func<bool>)")
                    {
                        commandPropertyDeclarationText = TemplateSeamster.PrepareTemplate(CommandPropDeclarationTemplate);
                        iCommandPropertiesSB.AppendLine(commandPropertyDeclarationText);
                        commandInitializationText = TemplateSeamster.PrepareTemplate(CommandActionFuncBoolInitTemplate);
                        commandExecuteMethodDeclaration = TemplateSeamster.PrepareTemplate(CommandExecDeclarationTemplate);
                        iCommandMethodsSB.AppendLine(commandExecuteMethodDeclaration);
                        iCommandMethodsSB.AppendLine();
                        commandCanExecuteMethodDeclaration = TemplateSeamster.PrepareTemplate(CommandCanExecDeclarationTemplate);
                        iCommandMethodsSB.AppendLine(commandCanExecuteMethodDeclaration);
                        iCommandMethodsSB.AppendLine();
                    }
                    else if (genericActionCommandRegex.IsMatch(propertyType))
                    {
                        string typeParameterMergeField = MergeFieldCatalog.instance.TypeParameterMergeField;
                        TemplateSeamster.MergeFieldsAndValues[typeParameterMergeField] = genericActionCommandRegex.Matches(propertyType)[0].Groups[1].Value;
                        commandPropertyDeclarationText = TemplateSeamster.PrepareTemplate(CommandTPropDeclarationTemplate);
                        iCommandPropertiesSB.AppendLine(commandPropertyDeclarationText);
                        commandInitializationText = TemplateSeamster.PrepareTemplate(CommandTActionInitTemplate);
                        commandExecuteMethodDeclaration = TemplateSeamster.PrepareTemplate(CommandTExecDeclarationTemplate);
                        iCommandMethodsSB.AppendLine(commandExecuteMethodDeclaration);
                        iCommandMethodsSB.AppendLine();
                    }
                    else if (genericActionFuncCommandRegex.IsMatch(propertyType))
                    {
                        string typeParameterMergeField = MergeFieldCatalog.instance.TypeParameterMergeField;
                        TemplateSeamster.MergeFieldsAndValues[typeParameterMergeField] = genericActionFuncCommandRegex.Matches(propertyType)[0].Groups[1].Value;
                        commandPropertyDeclarationText = TemplateSeamster.PrepareTemplate(CommandTPropDeclarationTemplate);
                        iCommandPropertiesSB.AppendLine(commandPropertyDeclarationText);
                        commandInitializationText = TemplateSeamster.PrepareTemplate(CommandTActionFuncBoolInitTemplate);
                        commandExecuteMethodDeclaration = TemplateSeamster.PrepareTemplate(CommandTExecDeclarationTemplate);
                        iCommandMethodsSB.AppendLine(commandExecuteMethodDeclaration);
                        iCommandMethodsSB.AppendLine();
                        commandCanExecuteMethodDeclaration = TemplateSeamster.PrepareTemplate(CommandTCanExecDeclarationTemplate);
                        iCommandMethodsSB.AppendLine(commandCanExecuteMethodDeclaration);
                        iCommandMethodsSB.AppendLine();
                    }

                    iCommandInitialisationsSB.AppendLine(commandInitializationText);
                }
                else
                {
                    TemplateSeamster.MergeFieldsAndValues[PropertyNameMergeField] = propertyName;
                    TemplateSeamster.MergeFieldsAndValues[PropertyTypeMergeField] = propertyType;
                    TemplateSeamster.MergeFieldsAndValues[FieldNameMergeField] = fieldName;
                    string fieldDeclarationText = TemplateSeamster.PrepareTemplate(FieldDeclarationTemplate);
                    string propertyDeclarationText = TemplateSeamster.PrepareTemplate(PropertyDeclarationTemplate);

                    fieldDeclarationsSB.AppendLine(fieldDeclarationText);
                    propertyDeclarationsSB.AppendLine(propertyDeclarationText);
                    propertyDeclarationsSB.AppendLine();
                }
            }

            fieldDeclarationsSB.AppendLine();
            iCommandPropertiesSB.AppendLine();
            propertyDeclarationsSB.AppendLine(PropertyChangedTemplate);
            propertyDeclarationsSB.AppendLine();

            string iCommandInitialisationsString = iCommandInitialisationsSB.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            string iCommandInitialisationsMergeField = MergeFieldCatalog.instance.ICommandInitializationsMergeField;
            TemplateSeamster.MergeFieldsAndValues[iCommandInitialisationsMergeField] = iCommandInitialisationsString;
            string constructorString = TemplateSeamster.PrepareTemplate(ConstructorTemplate) + Environment.NewLine + Environment.NewLine;

            string fieldDeclarationsString = fieldDeclarationsSB.ToString();
            string propertyDeclarationsString = propertyDeclarationsSB.ToString();
            string iCommandPropertyDeclarationsString = iCommandPropertiesSB.ToString();
            string iCommandMethodsString = iCommandMethodsSB.ToString();


            StringBuilder result = new();
            result.AppendLine($"public class {viewModelName}");
            result.AppendLine("{");
            string temp = fieldDeclarationsString + propertyDeclarationsString + iCommandPropertyDeclarationsString + constructorString + iCommandMethodsString.TrimEnd(Environment.NewLine.ToCharArray());
            result.AppendLine(temp);
            result.AppendLine("}");

            //string result = result.ToString();// fieldDeclarationsString + propertyDeclarationsString + iCommandPropertyDeclarationsString + constructorString + iCommandMethodsString;
            //result = result.TrimEnd(Environment.NewLine.ToCharArray());

            //Interop.ExportToNotepad(result);
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
            string commandImplementationType = TypeData.instance.ICommandImplementationType;
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
    }
}
