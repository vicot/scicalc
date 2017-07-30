using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SciCalc
{
    /// <summary>
    /// Interaction logic for EditConstDialog.xaml
    /// </summary>
    public partial class EditConstDialog : Window
    {
        public EditConstDialog()
        {
            this.InitializeComponent();
        }

        public EditConstDialog(string name, double value) : this()
        {
            this.NameBox.Text = name;
            this.ValueBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        public double Value { get; set; }
        public string ValueName { get; set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(this.ValueBox.Text, out double result))
            {
                this.Value = result;
            }
            else
            {
                MessageBox.Show($"'{this.ValueBox.Text}' is not a valid number.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(this.NameBox.Text))
            {
                MessageBox.Show($"Must provide name for the value", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            this.ValueName = this.NameBox.Text.ToUpper();
            this.DialogResult = true;
            this.Close();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.NameBox.Focus();
        }
    }
}
