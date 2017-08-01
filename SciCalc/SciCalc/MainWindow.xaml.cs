// MainWindow.xaml.cs Copyright (c) 2017 vicot
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SciCalc.Tokens;
using SciCalc.Tokens.Values;

namespace SciCalc
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ObservableCollection<Constant> constants = new ObservableCollection<Constant>();
        private readonly ObservableCollection<HistoryEntry> history = new ObservableCollection<HistoryEntry>();
        private readonly Parser parser;
        private int caretOffset;
        private TextPointer caretPointer;
        private TextRange caretSelection;
        private string oldExpression = "";

        public MainWindow()
        {
            this.InitializeComponent();
            this.parser = new Parser();

            //set window min size
            this.SourceInitialized += (sender, args) =>
            {
                this.MinWidth = this.ActualWidth;
                this.MinHeight = this.ActualHeight;
            };
        }

        /// <summary>
        /// Gets the combined expression from all runs as a string.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        private string Expression => this.ExpressionParagraph != null
            ? new TextRange(this.ExpressionParagraph.ContentStart, this.ExpressionParagraph.ContentEnd).Text
            : "";

        /// <summary>
        /// Save the expression and it's result in history
        /// </summary>
        /// <param name="result">The result.</param>
        private void SaveHistory(double result)
        {
            this.parser.SetConstant("ANS", result);
            this.history.Add(new HistoryEntry(this.Expression, result));
        }

        /// <summary>
        /// Solves the expression using Parser class, saves and displays the result.
        /// </summary>
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

        /// <summary>
        /// Clears the selection in expression box.
        /// </summary>
        private void ClearSelection()
        {
            if (this.caretSelection.Text.Length > 0)
            {
                this.ExpressionRichBox.CaretPosition = this.caretPointer;
            }
        }

        /// <summary>
        /// Loads the history entry into expression and result boxes.
        /// </summary>
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

        /// <summary>
        /// Formats the expression by parsing it into tokens and generating a series of TokenRuns based on them
        /// </summary>
        /// <param name="addedCharacter">The added character.</param>
        private void ReformatExpression(char addedCharacter = (char) 0)
        {
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
                run = (TokenRun) this.ExpressionParagraph.Inlines.LastInline;
            }


            string expression = this.Expression; //updated layout

            //int offset = this.caretOffset  + (staryexpression.Length - this.oldExpression.Length);
            int offset;
            if (addedCharacter == 8)
            {
                //backspace, find first difference from previous string
                for (offset = 0; offset < expression.Length; ++offset)
                {
                    if (offset >= this.oldExpression.Length)
                    {
                        break;
                    }
                    if (this.caretOffset > 0 && offset >= this.caretOffset)
                    {
                        break;
                    }
                    if (this.caretOffset > 1 && this.oldExpression[this.caretOffset - 1] == ' ' &&
                        offset >= this.caretOffset - 1)
                    {
                        break;
                    }

                    if (this.oldExpression[offset] != expression[offset])
                    {
                        break;
                    }
                }
            }
            else if (addedCharacter == 9)
            {
                //delete, find first difference from previous string
                for (offset = 0; offset < expression.Length; ++offset)
                {
                    if (offset >= this.oldExpression.Length)
                    {
                        break;
                    }
                    if (this.caretOffset > 0 && offset >= this.caretOffset)
                    {
                        break;
                    }

                    if (this.oldExpression[offset] != expression[offset])
                    {
                        break;
                    }
                }
            }
            else
            {
                offset = this.caretOffset;
                while (offset < expression.Length && expression[offset] != addedCharacter)
                {
                    offset++;
                }


                if (addedCharacter != 0)
                {
                    //move offset after the just added character, character 0 means there was no change
                    offset++;
                }
            }


            this.SetCursorPosition(offset);

            this.oldExpression = expression;
        }

        /// <summary>
        /// Sets the caret position in expression box.
        /// </summary>
        /// <param name="offset">The offset.</param>
        private void SetCursorPosition(int offset)
        {
            TextPointer tp = this.ExpressionParagraph.ContentStart;
            for (var o = 0; o < offset + 1; ++o)
            {
                TextPointer nextTp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                if (nextTp != null)
                {
                    tp = nextTp;
                }
                else
                {
                    break; //EOL?
                }
            }


            this.ExpressionRichBox.CaretPosition = tp;
        }

        /// <summary>
        /// Handles the Click event of the SolveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            this.SolveExpression();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// Link constant and history list sources.
        /// Start timer for blinking caret display.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ExpressionRichBox.CaretPosition = this.ExpressionParagraph.ContentStart;

            foreach (KeyValuePair<string, double> kp in this.parser.Constants)
            {
                this.constants.Add(new Constant(kp.Key, kp.Value));
            }

            this.ConstantsListBox.ItemsSource = this.constants;
            this.HistoryListBox.ItemsSource = this.history;

            //start timer for blinking cursor
            var timer = new DispatcherTimer();
            timer.Tick += (o, args) =>
            {
                this.CaretLine.Visibility = this.CaretLine.Visibility == Visibility.Visible
                    ? Visibility.Hidden
                    : Visibility.Visible;
            };
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the expression's RichTextBox control.
        /// Used to capture mouse input for the expression box and to calculate position for the custom caret
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var rtb = (RichTextBox) sender;
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

            Rect rect = rtb.Selection.End.GetCharacterRect(LogicalDirection.Forward);
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
            this.RightEllipsis.Visibility = rect.Right - scrollOffset - this.ExpressionScrollViewer.ActualWidth < 0 ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the ConstantsListBox control.
        /// Doubleclicking an entry will append it to the expression
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ConstantsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Constant c = this.ConstantsListBox.SelectedItem as Constant;
            if (c != null)
            {
                this.ProcessKeyInput(c.Symbol);
            }
        }

        /// <summary>
        /// Handles the Click event of the AddValueButton control.
        /// Spawns a prompt and adds registers new constant in the parser
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddValueButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditConstDialog {Owner = this};
            bool result = dialog.ShowDialog() ?? false;
            if (result)
            {
                var c = new Constant(dialog.ValueName, dialog.Value);
                this.constants.Add(c);

                this.parser.SetConstant(c.Symbol, c.Value);

                this.ReformatExpression();
            }
        }

        /// <summary>
        /// Handles the Click event of the RemoveValueButton control.
        /// Deregisters the selected constant from the parser
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the UiButton control. Generic event for most of input buttons, 
        /// uses DataContext property of the button as an input string
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UiButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                this.ProcessKeyInput(button.DataContext as string ?? "?");
            }
        }

        /// <summary>
        /// Handles the Click event of the DelButton control. Special button that simulates backspace key
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            //press backspace key
            this.ProcessKeyInput((char) 8);
        }

        /// <summary>
        /// Handles the Click event of the AcButton control. Special button that clears expression and result
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AcButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExpressionParagraph.Inlines.Clear();
            this.ResultsBox.Text = "";
            this.parser.UnsetConstant("ANS");
        }

        /// <summary>
        /// Handles the Click event of the MenuItemEdit control. Spawns a dialog to edit selected constant
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            Constant c = this.ConstantsListBox.SelectedItem as Constant;
            if (c != null)
            {
                var dialog = new EditConstDialog(c.Symbol, c.Value) {Owner = this};
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

        /// <summary>
        /// Handles the Click event of the MenuItemLoad control. Loads selected history entry into expression and result boxes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            this.LoadHistoryEntry();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemRemove control. Removes selected entry from history
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            var entry = this.HistoryListBox.SelectedItem as HistoryEntry;
            this.history.Remove(entry);
        }

        /// <summary>
        /// Handles the Click event of the MenuItemClearHistory control. Removes all entries from history
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemClearHistory_Click(object sender, RoutedEventArgs e)
        {
            this.history.Clear();
        }
    }
}
