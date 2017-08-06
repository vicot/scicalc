// MainWindow.InputManager.cs Copyright (c) 2017 vicot
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

using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SciCalc
{
    /// <summary>
    ///     Partial of MainWindow class that contain input management methods
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    public partial class MainWindow
    {
        /// <summary>
        ///     Processes the key input. Will perform special actions (such as deleting a character) or add new one.
        /// </summary>
        /// <param name="key">The key.</param>
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
                switch ((int) key)
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
                        if (range.Text.Length == 0)
                        {
                            return;
                        }


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
                        if (range.Text.Length == 0)
                        {
                            return;
                        }


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
                        if (offset >= expr.Length || expr[offset] != ')' &&
                            (expr.Length <= offset || expr[offset] != ' ' ||
                             expr[offset + 1] != ')'))
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

        /// <summary>
        ///     Process every key from a string
        /// </summary>
        /// <param name="keys">The keys.</param>
        private void ProcessKeyInput(string keys)
        {
            foreach (char key in keys)
            {
                this.ProcessKeyInput(key);
            }
        }

        /// <summary>
        ///     Handles the PreviewKeyDown event of the Window control.
        ///     Intercept special keys that would not reach regular TextInput event, due to being handled by inner controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
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
                        this.ProcessKeyInput((char) 8);
                        return;

                    case Key.Delete:
                        this.ProcessKeyInput((char) 9);
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
                            if (index < 0)
                            {
                                index = 0;
                            }
                            this.HistoryListBox.SelectedIndex = index;
                        }

                        return;
                    }

                    case Key.Down:
                    {
                        if (this.history.Count > 0)
                        {
                            int index = this.HistoryListBox.SelectedIndex + 1;
                            if (index >= this.history.Count)
                            {
                                index = this.history.Count - 1;
                            }
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

        /// <summary>
        ///     Handles the PreviewTextInput event of the Window control.
        ///     Intercept normal key input before it is consumed by inner controls. We must capture all input regardless of where
        ///     it happened
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs" /> instance containing the event data.</param>
        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

                if (key == ',')
                {
                    //replace comma with dot, for languages that use comma as floating point
                    key = '.';
                }

                //if not letter, process as-is
                this.ProcessKeyInput(key);
            }


            e.Handled = true;
        }
    }
}
