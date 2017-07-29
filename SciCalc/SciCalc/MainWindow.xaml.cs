using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using SciCalc.Tokens;

namespace SciCalc
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Parser parser;

        private int caretOffset;
        private TextPointer caretPointer;
        private TextRange caretSelection;

        private bool dontChangeRTB = false;
        private string oldExpression = "";

        public MainWindow()
        {
            this.InitializeComponent();
            this.parser = new Parser();

        }

        private string Expression => this.ExpressionParagraph != null
            ? new TextRange(this.ExpressionParagraph.ContentStart, this.ExpressionParagraph.ContentEnd).Text
            : "";

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            //this.parser.LoadToPostfix(this.EquationBox.Text);
            this.parser.LoadToPostfix(this.Expression);
            this.ResultParagraph.Inlines.Clear();
            this.ResultParagraph.Inlines.Add(new Run(this.parser.Solve().ToString(CultureInfo.InvariantCulture)));

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // e.Handled will be set back to false if no special keys are captured here in this function
            e.Handled = true;

            //handle C-v and C-c keys for paste/copy
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                switch (e.Key)
                {
                    case Key.V:
                        this.ProcessKeyInput(Clipboard.GetText(TextDataFormat.Text));
                        return;

                    case Key.C:
                        Clipboard.SetText("Hello world", TextDataFormat.Text);
                        return;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Back:
                        this.ProcessKeyInput((char)8);
                        return;

                    case Key.Delete:
                        this.ProcessKeyInput((char)9);
                        return;
                }
            }

            e.Handled = false;
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
            if (this.caretPointer == null)
            {
                this.caretPointer = this.ExpressionParagraph.ContentStart;
            }
            var range = new TextRange(this.caretSelection.Start, this.caretSelection.End);

            //special keys
            if (char.IsControl(key) || key == 'd')
            {
                switch ((int)key)
                {
                    case 8: //backspace
                        {
                            if (range.IsEmpty)
                            {
                                TextPointer previousPosition =
                                    this.caretSelection.End.GetNextInsertionPosition(LogicalDirection.Backward);
                                if (previousPosition != null)
                                {
                                    range = new TextRange(previousPosition, this.caretSelection.End);

                                    //when deleting space, delete the character before it
                                    if (range.Text == " ")
                                    {
                                        previousPosition =
                                            previousPosition.GetNextInsertionPosition(LogicalDirection.Backward);
                                        if (previousPosition != null)
                                        {
                                            range = new TextRange(previousPosition, this.caretSelection.End);

                                        }
                                    }

                                }
                            }

                            //add space before the range if needed

                            string charactersbefore =
                                range.Start.GetNextInsertionPosition(LogicalDirection.Backward)?
                                    .GetTextInRun(LogicalDirection.Forward) ?? "";
                            if (charactersbefore.Length > 0 && charactersbefore[0] == ' ')
                            {
                                range = new TextRange(range.Start.GetNextInsertionPosition(LogicalDirection.Backward),
                                                      range.End);
                            }

                            //nothing to remove, cancel operation
                            if (range.Text.Length == 0) return;

                            range.Text = "";
                            break;
                        }

                    case 9: //delete
                        {
                            if (range.IsEmpty)
                            {
                                TextPointer nextPosition =
                                    this.caretSelection.End.GetNextInsertionPosition(LogicalDirection.Forward);
                                if (nextPosition != null)
                                {
                                    range = new TextRange(this.caretSelection.End, nextPosition);

                                    //when deleting space, delete the character after it
                                    if (range.Text == " ")
                                    {
                                        nextPosition =
                                            nextPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                                        if (nextPosition != null)
                                        {
                                            range = new TextRange(this.caretSelection.End, nextPosition);

                                        }
                                    }

                                }
                            }

                            //add space after the range if needed

                            string charactersafter =
                                range.End.GetNextInsertionPosition(LogicalDirection.Forward)?
                                    .GetTextInRun(LogicalDirection.Backward) ?? "";
                            if (charactersafter.Length > 0 && charactersafter.Last() == ' ')
                            {
                                range = new TextRange(range.Start, range.End.GetNextInsertionPosition(LogicalDirection.Forward));
                            }

                            //nothing to remove, cancel operation
                            if (range.Text.Length == 0) return;

                            range.Text = "";
                            break;
                        }

                }
            }
            else
            {

                switch (key)
                {
                    case '(': //add closing )
                        range.Text = "()";
                        break;

                    case ')': // ) are autogenerated, skip if was writing next ) just before an existing )
                        {
                            string expr = this.Expression;
                            int offset = this.caretOffset;
                            if (offset >= expr.Length || (expr[offset] != ')' &&
                                                         (expr.Length <= offset || expr[offset] != ' ' ||
                                                          expr[offset + 1] != ')')))
                            {
                                range.Text = ")";
                            }

                            break;
                        }

                    default:
                        range.Text = key.ToString();
                        break;
                }

            }

            this.ReformatExpression(key);
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            //     Console.WriteLine($"[TextInput]: {Keyboard.Modifiers} - {e.Text}");

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
                char key = e.Text[0];

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
            }

            e.Handled = true;
        }

        private void ReformatExpression(char addedCharacter)
        {
            string staryexpression = this.Expression;
            List<Token> tokens = this.parser.ParseTokens(this.Expression);


            this.ExpressionParagraph.Inlines.Clear();

            if (tokens.Count == 0)
            {
                this.oldExpression = "";
                this.SetCursorPosition(0);
                return;
            }

            var run = new TokenRun(tokens.First());
            this.ExpressionParagraph.Inlines.Add(run);
            foreach (Token token in tokens.Skip(1))
            {
                this.ExpressionParagraph.Inlines.AddRange(run.ConnectToken(token));
                run = (TokenRun)this.ExpressionParagraph.Inlines.LastInline;
            }

            string expression = this.Expression; //updated layout


            Console.WriteLine($"[RTB] {this.caretOffset} diff {expression.Length - this.oldExpression.Length}");
            Console.WriteLine($"   {this.oldExpression}[{this.oldExpression.Length}");
            Console.WriteLine($"   {staryexpression}[{staryexpression.Length}");
            Console.WriteLine($"   {expression}[{expression.Length}");

            //int offset = this.caretOffset  + (staryexpression.Length - this.oldExpression.Length);
            int offset;
            if (addedCharacter == 8)
            {
                //backspace, find first difference from previous string
                for (offset = 0; offset < expression.Length; ++offset)
                {
                    if (offset >= this.oldExpression.Length) break;
                    if (this.caretOffset > 0 && offset >= this.caretOffset) break;
                    if (this.caretOffset > 1 && this.oldExpression[this.caretOffset - 1] == ' ' &&
                        offset >= this.caretOffset - 1)
                        break;

                    if (this.oldExpression[offset] != expression[offset]) break;
                }
            }
            else if (addedCharacter == 9)
            {
                //delete, find first difference from previous string
                for (offset = 0; offset < expression.Length; ++offset)
                {
                    if (offset >= this.oldExpression.Length) break;
                    if (this.caretOffset > 0 && offset >= this.caretOffset) break;
                    
                    if (this.oldExpression[offset] != expression[offset]) break;
                }
            }
            else
            {
                offset = this.caretOffset;
                while (offset < expression.Length && expression[offset] != addedCharacter) offset++;

                //move offset after the just added character
                offset++;
            }

            this.SetCursorPosition(offset);

            this.oldExpression = expression;
        }

        private void SetCursorPosition(int offset)
        {
            TextPointer tp = this.ExpressionParagraph.ContentStart;
            Console.WriteLine($"     setting offset {offset}");
            for (int o = 0; o < offset + 1; ++o)
            {
                Console.WriteLine($"        {o}");
                TextPointer nextTp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                if (nextTp != null)
                {
                    tp = nextTp;
                }
                else
                {
                    Console.WriteLine("         EOL?");
                    break; //EOL?
                }
            }

            this.ExpressionRichBox.CaretPosition = tp;
        }

        private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var rtb = (RichTextBox)sender;
            TextPointer selectionStart = rtb.Selection.Start;
            TextPointer selectionEnd = rtb.Selection.End;

            //trim selection to paragraph
            if (selectionStart.CompareTo(this.ExpressionParagraph.ContentStart) < 0)
            {
                selectionStart = this.ExpressionParagraph.ContentStart;
            }

            if (selectionEnd.CompareTo(this.ExpressionParagraph.ContentEnd) > 0)
            {
                selectionEnd = this.ExpressionParagraph.ContentEnd;
            }

            this.caretSelection = new TextRange(selectionStart, selectionEnd);

            this.caretPointer = rtb.Selection.Start;
            this.caretOffset = new TextRange(rtb.Selection.Start, this.ExpressionParagraph.ContentStart).Text.Length;
            Console.WriteLine($"### selection CHANGED: new position {this.caretOffset}");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ExpressionRichBox.CaretPosition = this.ExpressionParagraph.ContentStart;
        }


        private void UiButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                this.ProcessKeyInput(button.DataContext as string ?? "NIEMA");
            }
        }
    }
}
