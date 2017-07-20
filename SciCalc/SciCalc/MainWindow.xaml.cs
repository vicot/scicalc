using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SciCalc
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StringBuilder currentExpression;
        private readonly StringBuilder currentToken;
        private readonly Parser parser;

        public MainWindow()
        {
            this.InitializeComponent();
            this.parser = new Parser();
            this.currentToken = new StringBuilder();
            this.currentExpression = new StringBuilder();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            var expression = this.EquationBox.Text.Replace(" ", "");

            
            this.parser.Parse(expression);
            this.ResultBox.Text = this.parser.Solve().ToString(CultureInfo.InvariantCulture);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
  

        }

        private void ProcessKeyInput(string keys)
        {
            foreach (char key in keys)
            {
                this.ProcessKeyInput(key);
            }
        }

        private void ProcessKeyInput(char key)
        {
            this.currentToken.Append(key);
            this.parser.Parse(this.currentToken.ToString());
            string token = this.parser.PostfixNotation.OrderBy(t => t.Index).First().Symbol;
            if (this.parser.PostfixNotation.Count > 1)
            {
                //token finished, apply layout
                this.currentExpression.Append(token);
                this.currentToken.Clear().Append(key);
                token = this.parser.PostfixNotation.OrderBy(t => t.Index).Last().Symbol;
            }

            this.EquationBox.Text = this.currentExpression + token;
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            Console.WriteLine($"[KeyDown]: {Keyboard.Modifiers} - {e.Text}");

            bool shiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

            //handle C-v and C-c keys for paste/copy
            //if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            //{
            //    switch (e.Key)
            //    {
            //        case Key.V:
            //            this.ProcessKeyInput(Clipboard.GetText(TextDataFormat.Text));
            //            return;

            //        case Key.C:
            //            Clipboard.SetText("Hello world", TextDataFormat.Text);
            //            return;
            //    }
            //}

            //normal keys
            if (e.Text.Length == 1)
            {
                char key = e.Text.ToString()[0];

                //special case for upper and lowercase letters
                if (char.IsLetter(key))
                {
                    if (shiftPressed)
                    {
                        key = char.ToUpperInvariant(key);
                    }
                    this.ProcessKeyInput(key);
                    return;
                }

                //if not letter, process as-is
                this.ProcessKeyInput(key);
                return;
            }

        }
    }
}
