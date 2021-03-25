using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ViewModel_Builder.Utility;

namespace Backend
{
    /// <summary>
    /// Builds the contents of the view model from given data.
    /// </summary>
    public static class ViewModelBuilder
    {
        /// <summary>
        /// Replaces merge fields in property templates, defined in the appsettings file, to construct the code of the view model.
        /// Then opens a Notepad and send it there.
        /// </summary>
        /// <param name="configuration">The configuration to take settings from.</param>
        /// <param name="propertyNameAndTypeDict">A list of property names and their data types.</param>
        public static void GenerateViewModelText(IConfigurationRoot configuration, List<(string Name, string Type)> propertyNameAndTypeDict)
        {
            string propertyTypeMergeField = configuration["PropertyTypeMergeField"];
            string privatePropertyNameMergeField = configuration["PrivatePropertyNameMergeField"];
            string publicPropertyNameMergeField = configuration["PublicPropertyNameMergeField"];
            string privatePropertyTemplateFilepath = configuration["PrivatePropertyTemplate"];
            string publicPropertyTemplateFilepath = configuration["PublicPropertyTemplate"];
            string propertyChangedTemplateFilepath = configuration["PropertyChangedTemplate"];
            string privatePropertyTemplate = File.ReadAllText(privatePropertyTemplateFilepath);
            string publicPropertyTemplate = File.ReadAllText(publicPropertyTemplateFilepath);
            string propertyChangedTemplate = File.ReadAllText(propertyChangedTemplateFilepath);
            StringBuilder privatePropertiesSB = new StringBuilder();
            StringBuilder publicPropertiesSB = new StringBuilder();

            foreach (var NameAndType in propertyNameAndTypeDict)
            {
                string propertyType = NameAndType.Type;
                string propertyName = NameAndType.Name;
                string privatePropertyText = privatePropertyTemplate.Replace(propertyTypeMergeField, propertyType)
                                                                    .Replace(privatePropertyNameMergeField, propertyName.FirstLetterToLower());

                string publicPropertyText = publicPropertyTemplate.Replace(propertyTypeMergeField, propertyType)
                                                                  .Replace(publicPropertyNameMergeField, propertyName.FirstLetterToUpper())
                                                                  .Replace(privatePropertyNameMergeField, propertyName.FirstLetterToLower()) + Environment.NewLine;

                privatePropertiesSB.AppendLine(privatePropertyText);
                publicPropertiesSB.AppendLine(publicPropertyText);
            }

            privatePropertiesSB.AppendLine();
            publicPropertiesSB.AppendLine(propertyChangedTemplate);

            string result = privatePropertiesSB.ToString() + publicPropertiesSB.ToString();

            ExportToNotepad(result);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        /// <summary>
        /// Opens notepad.exe and send the given text to it.
        /// </summary>
        /// <param name="text">The text to send to notepad.exe.</param>
        static void ExportToNotepad(string text)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("notepad");
            startInfo.UseShellExecute = false;
            Process notepad = Process.Start(startInfo);
            notepad.WaitForInputIdle();
            IntPtr child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), null, null);
            SendMessage(child, 0x000c, 0, text);
        }
    }
}
