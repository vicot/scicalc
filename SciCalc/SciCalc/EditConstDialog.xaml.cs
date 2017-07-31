// EditConstDialog.xaml.cs Copyright (c) 2017 vicot
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Globalization;
using System.Windows;

namespace SciCalc
{
    /// <summary>
    ///     Interaction logic for EditConstDialog.xaml
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
