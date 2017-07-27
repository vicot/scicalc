using System;
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


        public void SetVariable(string name, double value)
        {
            this.Constants[name] = value;
        }

        public void LoadToPostfix(string expression)
        {
            var tokens = this.ParseTokens(expression);
            this.LoadToPostfix(tokens);
        }

        public bool UnsetVariable(string name)
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
                        var newToken = OperatorFactory.GetToken(c, lastToken == null || lastToken is Operator);
                        if (newToken is ParentOperator)
                        {
                            parentLevel++;
                        }
                        else if (newToken is CloseParentOperator)
                        {
                            parentLevel--;
                            if (parentLevel < 0) newToken.IsValid = false;
                        }
                        else
                        {
                            //ignore parent operators for lastToken
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
            
        }

        public void LoadToPostfix(List<Token> tokens)
        {
            this.PostfixNotation.Clear();
            var operators = new Stack<Token>();

            foreach (var token in tokens)
            {
                if (!token.IsValid)
                {
                    throw new ParseException(token.ErrorMessage);
                }

                switch (token.Type)
                {
                    case TokenType.Value:
                    case TokenType.Constant:
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
