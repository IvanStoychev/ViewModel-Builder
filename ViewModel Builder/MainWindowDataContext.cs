using Frontend;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ViewModel_Builder
{
    class MainWindowDataContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Amount of properties to generate.
        /// </summary>
        public int PropertiesCount
        {
            get => PropertyViewsCollection.Count;
            set
            {
                if (PropertyViewsCollection.Count != value)
                    PopulatePropertiesControls(value);
            }
        }

        /// <summary>
        /// Holds all the PropertyViews on the MainWindow.
        /// </summary>
        public ObservableCollection<PropertyView> PropertyViewsCollection { get; }        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public RelayCommand AddPropertyCommand { get; }
        public RelayCommand RemovePropertyCommand { get; }
        public RelayCommand ExecuteCommand { get; }

        public MainWindowDataContext()
        {
            PropertyViewsCollection = new ObservableCollection<PropertyView>();
            AddPropertyCommand = new RelayCommand(AddProperty);
            RemovePropertyCommand = new RelayCommand(RemoveProperty);
            ExecuteCommand = new RelayCommand(GenerateViewModelText);
        }

        /// <summary>
        /// Adds a new PropertyView to the PropertyViewsCollection.
        /// </summary>
        void AddProperty()
        {
            PropertyViewsCollection.Add(new PropertyView());
            OnPropertyChanged("PropertiesCount");
        }

        /// <summary>
        /// Removes the last PropertyView from the PropertyViewsCollection.
        /// </summary>
        void RemoveProperty()
        {
            PropertyViewsCollection.RemoveAt(PropertyViewsCollection.Count - 1);
            OnPropertyChanged("PropertiesCount");
        }

        /// <summary>
        /// Generates a set of properties and event handler for an MVVM viewmodel.
        /// </summary>
        private void GenerateViewModelText()
        {
            string propertyTypeMergeField = "{propertyType}";
            string privatePropertyNameMergeField = "{privatePropertyName}";
            string publicPropertyNameMergeField = "{publicPropertyName}";
            string privatePropertyTemplate = File.ReadAllText(@"Resources\Templates\Properties\Private property template.txt");
            string publicPropertyTemplate = File.ReadAllText(@"Resources\Templates\Properties\Public property template.txt");
            string propertyChangedTemplate = File.ReadAllText(@"Resources\Templates\Properties\Property changed template.txt");
            StringBuilder privatePropertiesSB = new StringBuilder();
            StringBuilder publicPropertiesSB = new StringBuilder();

            foreach (PropertyView propertyView in PropertyViewsCollection)
            {
                string propertyType = propertyView.tboxPropertyType.Text;
                string propertyName = propertyView.tboxPropertyName.Text;
                privatePropertiesSB.AppendLine(privatePropertyTemplate.Replace(propertyTypeMergeField, propertyType).Replace(privatePropertyNameMergeField, propertyName.FirstLetterToLower()));
                publicPropertiesSB.AppendLine(publicPropertyTemplate.Replace(propertyTypeMergeField, propertyType).Replace(publicPropertyNameMergeField, propertyName.FirstLetterToUpper()).Replace(privatePropertyNameMergeField, propertyName.FirstLetterToLower()) + Environment.NewLine);
            }

            privatePropertiesSB.AppendLine();
            publicPropertiesSB.AppendLine(propertyChangedTemplate);

            string result = privatePropertiesSB.ToString() + publicPropertiesSB.ToString();

            ExportToNotepad(result);
        }

        /// <summary>
        /// Creates the given amount of PropertyViews for property generation.
        /// </summary>
        /// <param name="count">The total number of PropertyViews that should exist.</param>
        void PopulatePropertiesControls(int count)
        {
            int difference = count - PropertyViewsCollection.Count;

            if (difference > 0)
                for (int i = 0; i < difference; i++)
                    AddProperty();
            else if (difference < 0)
                for (int i = 0; i > difference; i--)
                    RemoveProperty();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        /// <summary>
        /// Opens notepad.exe and send the given text to it.
        /// </summary>
        /// <param name="text">The text to send to notepad.exe.</param>
        void ExportToNotepad(string text)
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
