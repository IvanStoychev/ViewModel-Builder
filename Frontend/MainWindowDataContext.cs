using Frontend.Utility;
using Frontend.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Backend;
using Backend.Options;
using System.Windows;

namespace Frontend
{
    class MainWindowDataContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Name to use for the ViewModel class.
        /// </summary>
        public string ViewModelName { get; set; }

        /// <summary>
        /// Amount of properties to generate.
        /// </summary>
        public int PropertiesCount
        {
            get => AddPropertyControlsCollection.Count;
            set
            {
                if (AddPropertyControlsCollection.Count != value)
                    PopulatePropertiesControls(value);
            }
        }

        public List<string> PropertyTypes { get; }

        /// <summary>
        /// Holds all the PropertyViews on the MainWindow.
        /// </summary>
        public ObservableCollection<AddPropertyControl> AddPropertyControlsCollection { get; }  
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public RelayCommand AddPropertyCommand { get; }
        public RelayCommand RemovePropertyCommand { get; }
        public RelayCommand BuildFromPropertiesCommand { get; }
        public RelayCommand BuildFromClipboardCommand { get; }

        public MainWindowDataContext()
        {
            Startup.InitConfig();
            PropertyTypes = TypeData.instance.PropertyTypes;
            AddPropertyControlsCollection = new ObservableCollection<AddPropertyControl>();
            AddPropertyCommand = new RelayCommand(OnAddProperty);
            RemovePropertyCommand = new RelayCommand(OnRemoveProperty);
            BuildFromPropertiesCommand = new RelayCommand(OnBuildFromProperties);
            BuildFromClipboardCommand = new RelayCommand(OnBuildFromClipboard);
        }

        /// <summary>
        /// Adds a new <see cref="AddPropertyControl"/> to the AddPropertyControlsCollection.
        /// </summary>
        void OnAddProperty()
        {
            AddPropertyControlsCollection.Add(new AddPropertyControl() { PropertyTypes = PropertyTypes });
            OnPropertyChanged("PropertiesCount");
        }

        /// <summary>
        /// Removes the last AddPropertyControl from the AddPropertyControlsCollection.
        /// </summary>
        void OnRemoveProperty()
        {
            AddPropertyControlsCollection.RemoveAt(AddPropertyControlsCollection.Count - 1);
            OnPropertyChanged("PropertiesCount");
        }

        /// <summary>
        /// Gives the collection of properties and ViewModel name to the backend to generate the MVVM ViewModel.
        /// </summary>
        private void OnBuildFromProperties()
        {
            List<(string Name, string Type)> propertyNameAndTypeList = new();

            foreach (AddPropertyControl AddPropertyControl in AddPropertyControlsCollection)
                propertyNameAndTypeList.Add((AddPropertyControl.tboxPropertyName.Text, AddPropertyControl.cbPropertyType.Text));

            ViewModelBuilder.GenerateViewModelText(propertyNameAndTypeList, ViewModelName);
        }

        /// <summary>
        /// Gives the collection of properties and ViewModel name to the backend to generate the MVVM ViewModel.
        /// </summary>
        private void OnBuildFromClipboard()
        {
            string clipboardText = Clipboard.GetText();
            ViewModelBuilder.ExtractFromXAML(clipboardText);
        }

        /// <summary>
        /// Creates the given amount of PropertyViews for property generation.
        /// </summary>
        /// <param name="count">The total number of PropertyViews that should exist.</param>
        void PopulatePropertiesControls(int count)
        {
            int difference = count - AddPropertyControlsCollection.Count;

            if (difference > 0)
                for (int i = 0; i < difference; i++)
                    OnAddProperty();
            else if (difference < 0)
                for (int i = 0; i > difference; i--)
                    OnRemoveProperty();
        }
    }
}
