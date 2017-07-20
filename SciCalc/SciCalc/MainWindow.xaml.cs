using System.Globalization;
using System.Windows;

namespace SciCalc
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            this.EquationBox.Text = this.EquationBox.Text.Replace(" ", "");

            var p = new Parser();
            p.Parse(this.EquationBox.Text);
            this.ResultBox.Text = p.Solve().ToString(CultureInfo.InvariantCulture);
        }
    }
}
