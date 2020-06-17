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
        public int PropertiesCount
        {
            get => PropertyViewsCollection.Count;
            set
            {
                if (PropertyViewsCollection.Count != value)
                {
                    AdjustPropertiesCount(value);
                }
            }
        }

        public ObservableCollection<PropertyView> PropertyViewsCollection { get; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public RelayCommand AddPropertyCommand { get; }
        public RelayCommand RemovePropertyCommand { get; }
        public RelayCommand ExecuteCommand { get; }

        public MainWindowDataContext()
        {
            PropertyViewsCollection = new ObservableCollection<PropertyView>();
            AddPropertyCommand = new RelayCommand(OnAddProperty);
            RemovePropertyCommand = new RelayCommand(OnRemoveProperty);
            ExecuteCommand = new RelayCommand(OnExecute);
        }

        void OnAddProperty()
        {
            PropertyViewsCollection.Add(new PropertyView());
            OnPropertyChanged("PropertiesCount");
        }

        void OnRemoveProperty()
        {
            PropertyViewsCollection.RemoveAt(PropertyViewsCollection.Count - 1);
            OnPropertyChanged("PropertiesCount");
        }

        private void OnExecute()
        {
            string propertyTypeMergeField = "{propertyType}";
            string privatePropertyNameMergeField = "{privatePropertyName}";
            string publicPropertyNameMergeField = "{publicPropertyName}";
            string privatePropertyTemplate = File.ReadAllText("Private property template.txt");
            string publicPropertyTemplate = File.ReadAllText("Public property template.txt");
            string propertyChangedTemplate = File.ReadAllText("Property changed template.txt");
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

        void AdjustPropertiesCount(int count)
        {
            int difference = count - PropertyViewsCollection.Count;

            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    OnAddProperty();
                }
            }
            else
            {
                difference = Math.Abs(difference);
                for (int i = 0; i < difference; i++)
                {
                    OnRemoveProperty();
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

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
