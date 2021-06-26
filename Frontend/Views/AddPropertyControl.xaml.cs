using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Views
{
    /// <summary>
    /// Interaction logic for AddPropertyControl.xaml
    /// </summary>
    public partial class AddPropertyControl : UserControl
    {
        public List<string> PropertyTypes
        {
            get { return (List<string>)GetValue(PropertyTypesProperty); }
            set { SetValue(PropertyTypesProperty, value); }
        }

        public static readonly DependencyProperty PropertyTypesProperty =
            DependencyProperty.Register("PropertyTypes", typeof(List<string>), typeof(AddPropertyControl));

        public AddPropertyControl()
        {
            InitializeComponent();
        }
    }
}
