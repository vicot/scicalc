// Parser.cs Copyright (c) 2017 vicot
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
using System.Globalization;
using System.Linq;
using SciCalc.Tokens;
using SciCalc.Tokens.Functions;
using SciCalc.Tokens.Operators;
using SciCalc.Tokens.Values;

namespace SciCalc
{
    public class Parser
    {
        public Parser()
        {
            this.Constants = new Dictionary<string, double>
            {
                {"PI", Math.PI},
                {"E", Math.E}
            };
            this.PostfixNotation = new Stack<Token>();
        }

        private Stack<Token> PostfixNotation { get; }

        public Dictionary<string, double> Constants { get; }

        /// <summary>
        /// Tokenize the expression into list of Tokens
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>List of Tokens</returns>
        public List<Token> ParseTokens(string expression)
        {
            var state = ParseState.None;
            var startPosition = 0;
            var parentLevel = 0;
            var endWord = false;
            Token lastToken = null;


            expression = expression.Replace(" ", "");

            var tokenList = new List<Token>(expression.Length);

            for (var pos = 0; pos < expression.Length; ++pos)
            {
                char c = expression[pos];
                if (c >= '0' && c <= '9') //numbers
                {
                    switch (state)
                    {
                        case ParseState.None:
                            state = ParseState.ValueInteger;
                            startPosition = pos;
                            break;

                        case ParseState.ValueInteger:
                        case ParseState.ValueDouble:
                            break; //continue number

                        default:
                            endWord = true;
                            break;
                    }
                }
                else if (c == '.') //real
                {
                    switch (state)
                    {
                        case ParseState.None:
                            tokenList.Add(new ExcessiveDotToken());
                            break;

                        case ParseState.ValueInteger:
                            state = ParseState.ValueDouble;
                            break;

                        case ParseState.ValueDouble:
                            lastToken = this.ParseLongToken(expression, state, startPosition, pos);
                            tokenList.Add(lastToken);
                            tokenList.Add(new ExcessiveDotToken());
                            state = ParseState.None;
                            continue;

                        default:
                            endWord = true;
                            break;
                    }
                }
                else if (c >= 'A' && c <= 'Z') //constants
                {
                    switch (state)
                    {
                        case ParseState.None:
                            startPosition = pos;
                            state = ParseState.Constant;
                            break;

                        case ParseState.Constant:
                            break; // continue reading name

                        default:
                            endWord = true;
                            break;
                    }
                }
                else if (c >= 'a' && c <= 'z') //functions
                {
                    switch (state)
                    {
                        case ParseState.None:
                            state = ParseState.Function;
                            startPosition = pos;
                            break;

                        case ParseState.Function:
                            break; //continue reading name

                        default:
                            endWord = true;
                            break;
                    }
                }
                else //operators
                {
                    if (state == ParseState.None)
                    {
                        Token newToken = OperatorFactory.GetToken(c, lastToken == null || lastToken is Operator && !(lastToken is CloseParentOperator));
                        if (newToken is ParentOperator)
                        {
                            parentLevel++;

                            //clear lastToken for parenthesis so they don't break unary operators inside
                            lastToken = null;
                        }
                        else if (newToken is CloseParentOperator)
                        {
                            parentLevel--;
                            if (parentLevel < 0)
                            {
                                newToken.IsValid = false;
                            }

                            //save closing parenthesis as last token, so the operator right after the ) doesn't think it's unary
                            lastToken = newToken;
                        }
                        else
                        {
                            //save other operators as last token
                            lastToken = newToken;
                        }

                        tokenList.Add(newToken);
                    }
                    else
                    {
                        endWord = true;
                    }
                }


                if (endWord)
                {
                    endWord = false;
                    lastToken = this.ParseLongToken(expression, state, startPosition, pos);
                    tokenList.Add(lastToken);

                    state = ParseState.None;
                    --pos; //scan the character again
                }
            }

            //finish parsing last token
            if (state != ParseState.None)
            {
                tokenList.Add(this.ParseLongToken(expression, state, startPosition, expression.Length));
            }

            this.VerifyTokens(tokenList);
            return tokenList;
        }

        /// <summary>
        /// Parses the string into a proper Token based on current parser state.
        /// </summary>
        /// <param name="expression">The whole expression.</param>
        /// <param name="state">The current parser state.</param>
        /// <param name="startPosition">The position where the token starts in the expression.</param>
        /// <param name="endPosition">The position where the token ends.</param>
        /// <returns>Parsed Token</returns>
        private Token ParseLongToken(string expression, ParseState state, int startPosition, int endPosition)
        {
            string tokenstring = expression.Substring(startPosition, endPosition - startPosition);
            switch (state)
            {
                case ParseState.ValueInteger:
                    return new IntegerValue(long.Parse(tokenstring));

                case ParseState.ValueDouble:
                    return new DoubleValue(double.Parse(tokenstring, NumberStyles.Any, CultureInfo.InvariantCulture));

                case ParseState.Constant:
                    return this.Constants.ContainsKey(tokenstring)
                        ? new Constant(tokenstring, this.Constants[tokenstring])
                        : new Constant(tokenstring);

                case ParseState.Function:
                    return FunctionFactory.GetToken(tokenstring);
            }

            return null;
        }

        /// <summary>
        /// Verifies the tokens. Marks them as invalid if they are paired with something unexpected (such as two operators next to each other)
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        private void VerifyTokens(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                return;
            }

            if (!tokens.First().IsLeftBound(null))
            {
                tokens.First().IsValid = false;
            }

            for (var i = 1; i < tokens.Count; ++i)
            {
                if (!tokens[i - 1].IsRightBound(tokens[i]))
                {
                    tokens[i - 1].IsValid = false;
                }
                if (!tokens[i].IsLeftBound(tokens[i - 1]))
                {
                    tokens[i].IsValid = false;
                }
            }

            if (!tokens.Last().IsRightBound(null))
            {
                tokens.Last().IsValid = false;
            }
        }

        /// <summary>
        /// Inserts the missing multiplication operators in the token list. 
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>List of Tokens with added MulOperators</returns>
        private List<Token> InsertMissingTokens(List<Token> tokens)
        {
            //we will clone the original list and insert missing * operators between some tokens
            var newTokens = new List<Token>();

            for (var i = 0; i < tokens.Count - 1; ++i)
            {
                newTokens.Add(tokens[i]);

                /* add multiplication between:
                 * - values (and constants)
                 * - values and functions
                 * - values and parenthesis
                 * - closing and opening parenthesis
                 * - left-bound unary operators and opening parenthesis
                 * - closing parenthesis and values, functions or right-bound unary operators
                 * 
                 * don't add multiplication for logartihm case: log2(...)
                 */
                if (
                    tokens[i].TokenType == TokenType.Value && tokens[i + 1].TokenType == TokenType.Value ||
                    tokens[i].TokenType == TokenType.Value && tokens[i + 1].TokenType == TokenType.Function ||
                    tokens[i].TokenType == TokenType.Value && tokens[i + 1] is ParentOperator && (i == 0 || !(tokens[i - 1] is LogFunction)) ||
                    tokens[i] is PercentOperator && tokens[i + 1] is ParentOperator ||
                    tokens[i] is FactorialOperator && tokens[i + 1] is ParentOperator ||
                    tokens[i] is CloseParentOperator && tokens[i + 1].TokenType == TokenType.Value ||
                    tokens[i] is CloseParentOperator && tokens[i + 1].TokenType == TokenType.Function ||
                    tokens[i] is CloseParentOperator && tokens[i + 1] is ParentOperator)
                {
                    newTokens.Add(new MulOperator());
                }
            }

            //add the last token that was ommited in the loop
            newTokens.Add(tokens.Last());

            return newTokens;
        }

        /// <summary>
        /// Converts the infix expression into postfix notation (RPN)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ParseException"></exception>
        public void LoadToPostfix(string expression)
        {
            List<Token> tokens = this.ParseTokens(expression);
            this.PostfixNotation.Clear();
            var operators = new Stack<Token>();

            tokens = this.InsertMissingTokens(tokens);

            foreach (Token token in tokens)
            {
                if (!token.IsValid)
                {
                    throw new ParseException(token.ErrorMessage);
                }

                switch (token.TokenType)
                {
                    case TokenType.Value:
                        this.PostfixNotation.Push(token);
                        break;

                    case TokenType.Function:
                        if (operators.Count > 0)
                        {
                            Token top1 = operators.Peek();
                            if (top1.Priority > token.Priority)
                            {
                                while (operators.Count > 0)
                                {
                                    this.PostfixNotation.Push(operators.Pop());
                                }
                            }
                        }

                        operators.Push(token);
                        break;

                    case TokenType.Operator:
                        if (token is CloseParentOperator)
                        {
                            while (true)
                            {
                                Token t = operators.Pop();
                                if (t is ParentOperator)
                                {
                                    break;
                                }

                                this.PostfixNotation.Push(t);
                            }

                            //skip pushing of ) operator
                            continue;
                        }

                        if (token.Priority > 0 && operators.Count > 0)
                        {
                            Token top = operators.Peek();
                            if (top.Priority >= token.Priority)
                            {
                                while (operators.Count > 0)
                                {
                                    Token t = operators.Peek();
                                    if (t is ParentOperator)
                                    {
                                        break; // dump only from inside parent
                                    }
                                    if (t.Priority < token.Priority)
                                    {
                                        break; // dump only operators with higher priority
                                    }

                                    this.PostfixNotation.Push(operators.Pop());
                                }
                            }
                        }

                        operators.Push(token);
                        break;
                }
            }

            //copy remaining operators to RPN stack
            while (operators.Count > 0)
            {
                this.PostfixNotation.Push(operators.Pop());
            }
        }

        /// <summary>
        /// Solve the postfix expression that was loaded with LoadToPostfix method
        /// </summary>
        /// <returns>Result of the equation</returns>
        /// <exception cref="ParseException">Invalid expression.</exception>
        public double Solve()
        {
            var results = new Stack<double>();
            IEnumerable<Token> input = this.PostfixNotation.Reverse();

            foreach (Token token in input)
            {
                if (token is Value || token is Constant)
                {
                    results.Push(token.Value);
                }
                else if (token is Operator || token is Function)
                {
                    if (token.ArgumentCount == 1)
                    {
                        double arg = results.Pop();
                        results.Push(token.Execute(arg));
                    }
                    else if (token.ArgumentCount == 2)
                    {
                        double arg2 = results.Pop();
                        double arg1 = results.Pop();
                        results.Push(token.Execute(arg1, arg2));
                    }
                }
            }

            if (results.Count != 1)
            {
                throw new ParseException("Invalid expression.");
            }

            return results.Pop();
        }

        /// <summary>
        /// Sets a constant that can be used in the expression.
        /// </summary>
        /// <param name="name">The constant's name.</param>
        /// <param name="value">The constant's value.</param>
        public void SetConstant(string name, double value)
        {
            this.Constants[name] = value;
        }

        /// <summary>
        /// Removes a constant from allowed constants in the expression
        /// </summary>
        /// <param name="name">The constant's name.</param>
        /// <returns>False if constant wasn't set, True otherwise</returns>
        public bool UnsetConstant(string name)
        {
            return this.Constants.Remove(name);
        }

        /// <summary>
        /// Internal states used during tokenization of string input
        /// </summary>
        private enum ParseState
        {
            None,
            ValueInteger,
            ValueDouble,
            Constant,
            Function
        }
    }
}
