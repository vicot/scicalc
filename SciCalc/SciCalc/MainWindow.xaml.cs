using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using SciCalc.Tokens;
using SciCalc.Tokens.Values;

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
        private ObservableCollection<HistoryEntry> history = new ObservableCollection<HistoryEntry>();

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
            this.SolveExpression();
        }

        private void SolveExpression()
        {
            try
            {
                this.parser.LoadToPostfix(this.Expression);
                this.ResultsBox.Foreground = Brushes.Black;
                double result = this.parser.Solve();
                this.SaveHistory(result);
                this.ResultsBox.Text = result.ToString(CultureInfo.InvariantCulture);
            }
            catch (ParseException ex)
            {
                this.ResultsBox.Foreground = Brushes.DarkRed;
                this.ResultsBox.Text = ex.Message;
            }
            catch (Exception ex)
            {
                this.ResultsBox.Foreground = Brushes.DarkRed;
                this.ResultsBox.Text = ex.Message;

            }
        }

        private void SaveHistory(double result)
        {
            this.parser.SetConstant("ANS", result);
            this.history.Add(new HistoryEntry(this.Expression, result));
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
                        if (this.caretSelection.Text.Length > 0)
                        {
                            Clipboard.SetText(this.caretSelection.Text, TextDataFormat.Text);
                        }
                        else
                        {
                            Clipboard.SetText(this.ResultsBox.Text, TextDataFormat.Text);
                        }
                        return;

                    case Key.R:
                        this.ProcessKeyInput('√');
                        return;

                    case Key.Space:
                        this.LoadHistoryEntry();
                        return;

                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        this.ExpressionParagraph.Inlines.Clear();
                        this.ResultsBox.Text = "";
                        this.SetCursorPosition(0);
                        return;

                    case Key.Back:
                        this.ProcessKeyInput((char)8);
                        return;

                    case Key.Delete:
                        this.ProcessKeyInput((char)9);
                        return;

                    case Key.Enter:
                        this.SolveExpression();
                        return;

                    case Key.Home:
                        this.ClearSelection();
                        this.SetCursorPosition(0);
                        return;

                    case Key.End:
                        this.ClearSelection();
                        //go as far as possible
                        this.SetCursorPosition(int.MaxValue - 1);
                        return;

                    case Key.Left:
                        this.ClearSelection();
                        this.SetCursorPosition(this.caretOffset - 1);
                        return;

                    case Key.Right:
                        this.ClearSelection();
                        this.SetCursorPosition(this.caretOffset + 1);
                        return;

                    case Key.Up:
                        {
                            if (this.history.Count > 0)
                            {
                                int index = this.HistoryListBox.SelectedIndex - 1;
                                if (index < 0) index = 0;
                                this.HistoryListBox.SelectedIndex = index;
                            }

                            return;
                        }

                    case Key.Down:
                        {
                            if (this.history.Count > 0)
                            {
                                int index = this.HistoryListBox.SelectedIndex + 1;
                                if (index >= this.history.Count) index = this.history.Count-1;
                                this.HistoryListBox.SelectedIndex = index;
                            }

                            return;
                        }

                    case Key.Space:
                        //intercept and cancel space keypress
                        return;
                }
            }

            e.Handled = false;
        }

        private void ClearSelection()
        {
            if (this.caretSelection.Text.Length > 0)
            {
                this.ExpressionRichBox.CaretPosition = this.caretPointer;
            }
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
            if (char.IsControl(key))
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
            bool shiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

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

        private void ReformatExpression(char addedCharacter = (char)0)
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

                if (addedCharacter != 0)
                {
                    //move offset after the just added character, character 0 means there was no change
                    offset++;
                }
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

            var rect = rtb.Selection.End.GetCharacterRect(LogicalDirection.Forward);
            this.CaretLine.X1 = rect.TopLeft.X;
            this.CaretLine.Y1 = rect.TopLeft.Y;
            this.CaretLine.X2 = rect.BottomLeft.X;
            this.CaretLine.Y2 = rect.BottomLeft.Y;

            //scroll the view
            double scrollOffset = this.CaretLine.X1 - this.ExpressionScrollViewer.ActualWidth + 15;
            this.ExpressionScrollViewer.ScrollToHorizontalOffset(scrollOffset);

            //show left ellipsis if content is scrolled to the right
            this.LeftEllipsis.Visibility = scrollOffset < 0 ? Visibility.Hidden : Visibility.Visible;

            //show right ellipsis if remaining content is longer than scrollview's width
            rect = this.ExpressionParagraph.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            this.RightEllipsis.Visibility = (rect.Right - scrollOffset - this.ExpressionScrollViewer.ActualWidth) < 0 ? Visibility.Hidden : Visibility.Visible;

            Console.WriteLine($"### selection CHANGED: new position {this.caretOffset}");
        }

        private readonly ObservableCollection<Constant> constants = new ObservableCollection<Constant>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ExpressionRichBox.CaretPosition = this.ExpressionParagraph.ContentStart;

            foreach (var kp in this.parser.Constants)
            {
                this.constants.Add(new Constant(kp.Key, kp.Value));
            }

            this.ConstantsListBox.ItemsSource = this.constants;
            this.HistoryListBox.ItemsSource = this.history;

            //start timer for blinking cursor
            var timer = new DispatcherTimer();
            timer.Tick += (o, args) => {
                this.CaretLine.Visibility = this.CaretLine.Visibility == Visibility.Visible
                    ? Visibility.Hidden
                    : Visibility.Visible;
            };
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }


        private void UiButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                this.ProcessKeyInput(button.DataContext as string ?? "?");
            }
        }

        private void ConstantsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Constant c = this.ConstantsListBox.SelectedItem as Constant;
            if (c != null) this.ProcessKeyInput(c.Symbol);
        }

        private void RemoveValueButton_Click(object sender, RoutedEventArgs e)
        {
            Constant c = this.ConstantsListBox.SelectedItem as Constant;
            if (c != null)
            {
                this.parser.UnsetConstant(c.Symbol);
                this.constants.Remove(c);
                this.ReformatExpression();
            }
        }

        private void AddValueButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditConstDialog { Owner = this };
            bool result = dialog.ShowDialog() ?? false;
            if (result)
            {
                Constant c = new Constant(dialog.ValueName, dialog.Value);
                this.constants.Add(c);

                this.parser.SetConstant(c.Symbol, c.Value);

                this.ReformatExpression();
            }
        }

        private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            Constant c = this.ConstantsListBox.SelectedItem as Constant;
            if (c != null)
            {
                var dialog = new EditConstDialog(c.Symbol, c.Value) { Owner = this };
                bool result = dialog.ShowDialog() ?? false;
                if (result)
                {
                    //value was edited. Remove old one and insert new
                    this.parser.UnsetConstant(c.Symbol);

                    c.Value = dialog.Value;
                    c.Symbol = dialog.ValueName;

                    this.parser.SetConstant(c.Symbol, c.Value);
                    this.ReformatExpression();
                }
            }
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            //press backspace key
            this.ProcessKeyInput((char)8);
        }

        private void AcButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExpressionParagraph.Inlines.Clear();
            this.ResultsBox.Text = "";
            this.parser.UnsetConstant("ANS");
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            this.LoadHistoryEntry();
        }

        private void LoadHistoryEntry()
        {
            var entry = this.HistoryListBox.SelectedItem as HistoryEntry;
            if (entry == null)
            {
                return;
            }

            this.ExpressionParagraph.Inlines.Clear();
            this.ExpressionParagraph.Inlines.Add(new Run(entry.Expression));

            this.ResultsBox.Text = entry.Result.ToString(CultureInfo.InvariantCulture);

            //scroll to end
            this.SetCursorPosition(int.MaxValue - 1);
            this.ReformatExpression();
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            var entry = this.HistoryListBox.SelectedItem as HistoryEntry;
            this.history.Remove(entry);
        }

        private void MenuItemClearHistory_Click(object sender, RoutedEventArgs e)
        {
            this.history.Clear();
        }
    }
}
