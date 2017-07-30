﻿using System;
using System.Collections.Generic;
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
            new Stack<Token>();
        }

        public Stack<Token> PostfixNotation { get; }

        public Dictionary<string, double> Constants { get; }


        public void SetConstant(string name, double value)
        {
            this.Constants[name] = value;
        }

        public void LoadToPostfix(string expression)
        {
            var tokens = this.ParseTokens(expression);
            this.LoadToPostfix(tokens);
        }

        public bool UnsetConstant(string name)
        {
            return this.Constants.Remove(name);
        }

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
                        var newToken = OperatorFactory.GetToken(c, lastToken == null || (lastToken is Operator && !(lastToken is CloseParentOperator)));
                        if (newToken is ParentOperator)
                        {
                            parentLevel++;

                            //clear lastToken for parenthesis so they don't break unary operators inside
                            lastToken = null;
                        }
                        else if (newToken is CloseParentOperator)
                        {
                            parentLevel--;
                            if (parentLevel < 0) newToken.IsValid = false;

                            //save closing parenthesis as last token, so the operator right after the ) doesn't think it's unary
                            lastToken = newToken;
                        }
                        else
                        {
                            //save other operators as last token
                            lastToken = newToken;
                        }

                        //don't add excessive parents
                        //if(newToken != null)
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

//            while (parentLevel-- > 0)
//            {
//                //autoclose remaining parents
//                tokenList.Add(new CloseParentOperator{Inferred = true});
//            }

            this.VerifyTokens(tokenList);
            return tokenList;
        }

        private Token ParseLongToken(string expression, ParseState state, int startPosition, int endPosition)
        {
            string tokenstring = expression.Substring(startPosition, endPosition - startPosition);
            switch (state)
            {
                case ParseState.ValueInteger:
                    return new IntegerValue(long.Parse(tokenstring));

                case ParseState.ValueDouble:
                    return new DoubleValue(double.Parse(tokenstring));

                case ParseState.Constant:
                    return this.Constants.ContainsKey(tokenstring)
                        ? new Constant(tokenstring, this.Constants[tokenstring])
                        : new Constant(tokenstring);

                case ParseState.Function:
                    return FunctionFactory.GetToken(tokenstring);
            }

            return null;
        }

        private void VerifyTokens(List<Token> tokens)
        {
            if (tokens.Count == 0) return;

            if (!tokens.First().IsLeftBound(null))
                tokens.First().IsValid = false;

            for (int i = 1; i < tokens.Count; ++i)
            {
                if (!tokens[i - 1].IsRightBound(tokens[i]))
                    tokens[i - 1].IsValid = false;
                if (!tokens[i].IsLeftBound(tokens[i-1]))
                    tokens[i].IsValid = false;
            }

            if (!tokens.Last().IsRightBound(null))
                tokens.Last().IsValid = false;

        }

        public void LoadToPostfix(List<Token> tokens)
        {
            this.PostfixNotation.Clear();
            var operators = new Stack<Token>();

            tokens = this.InsertMissingTokens(tokens);

            foreach (var token in tokens)
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
                                if (t is ParentOperator) break;

                                this.PostfixNotation.Push(t);
                            }

                            //skip pushing of ) operator
                            continue;
                        }
                        else if (token.Priority > 0 && operators.Count > 0)
                        {
                            Token top = operators.Peek();
                            if (top.Priority >= token.Priority)
                            {
                                while (operators.Count > 0)
                                {
                                    Token t = operators.Peek();
                                    if (t is ParentOperator) break; // dump only from inside parent
                                    if (t.Priority < token.Priority) break; // dump only operators with higher priority
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

        private List<Token> InsertMissingTokens(List<Token> tokens)
        {
            //we will clone the original list and insert missing * operators between some tokens
            var newTokens = new List<Token>();

            for (int i = 0; i < tokens.Count-1; ++i)
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
                    (tokens[i].TokenType == TokenType.Value && tokens[i + 1].TokenType == TokenType.Value) ||
                    (tokens[i].TokenType == TokenType.Value && tokens[i + 1].TokenType == TokenType.Function) ||
                    (tokens[i].TokenType == TokenType.Value && tokens[i + 1] is ParentOperator && (i == 0 || !(tokens[i - 1] is LogFunction))) ||
                    (tokens[i] is PercentOperator && tokens[i + 1] is ParentOperator) ||
                    (tokens[i] is FactorialOperator && tokens[i + 1] is ParentOperator) ||
                    (tokens[i] is CloseParentOperator && tokens[i + 1].TokenType == TokenType.Value) ||
                    (tokens[i] is CloseParentOperator && tokens[i + 1].TokenType == TokenType.Function) ||
                    (tokens[i] is CloseParentOperator && tokens[i + 1] is ParentOperator))
                {
                    newTokens.Add(new MulOperator());
                }


            }

            //add the last token that was ommited in the loop
            newTokens.Add(tokens.Last());

            return newTokens;
        }

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
                    else if(token.ArgumentCount == 2)
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
