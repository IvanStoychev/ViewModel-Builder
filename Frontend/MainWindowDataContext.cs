using Frontend.Utility;
using Frontend.Views;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Backend;

namespace Frontend
{
    class MainWindowDataContext : INotifyPropertyChanged
    {
        readonly IConfigurationRoot configuration;

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
        public RelayCommand ExecuteCommand { get; }

        public MainWindowDataContext()
        {
            PropertyTypes = new List<string>() { "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "short", "ushort" };
            AddPropertyControlsCollection = new ObservableCollection<AddPropertyControl>();
            AddPropertyCommand = new RelayCommand(OnAddProperty);
            RemovePropertyCommand = new RelayCommand(OnRemoveProperty);
            ExecuteCommand = new RelayCommand(OnExecute);

            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
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
        /// Generates a set of properties and event handler for an MVVM viewmodel.
        /// </summary>
        private void OnExecute()
        {
            List<(string Name, string Type)> propertyNameAndTypeList = new();

            foreach (AddPropertyControl AddPropertyControl in AddPropertyControlsCollection)
                propertyNameAndTypeList.Add((AddPropertyControl.tboxPropertyName.Text, AddPropertyControl.cbPropertyType.Text));

            ViewModelBuilder.GenerateViewModelText(configuration, propertyNameAndTypeList);
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
