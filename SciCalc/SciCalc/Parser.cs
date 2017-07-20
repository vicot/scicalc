using System;
using System.Collections.Generic;
using System.Linq;
using SciCalc.Tokens;
using SciCalc.Tokens.Functions;
using SciCalc.Tokens.Operators;

namespace SciCalc
{
    public class Parser
    {
        private readonly Stack<Token> operators;

        private readonly Stack<Token> rpn;
        private Token lastToken;

        public Parser()
        {
            this.Variables = new Dictionary<string, double>
            {
                {"PI", Math.PI},
                {"E", Math.E}
            };
            this.rpn = new Stack<Token>();
            this.operators = new Stack<Token>();
        }

        public Dictionary<string, double> Variables { get; }


        public void SetVariable(string name, double value)
        {
            this.Variables[name] = value;
        }

        public bool UnsetVariable(string name)
        {
            return this.Variables.Remove(name);
        }

        public void Parse(string expression)
        {
            this.operators.Clear();
            this.rpn.Clear();
            this.lastToken = null;

            var state = ParseState.None;
            var startPosition = 0;
            var endWord = false;


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
                else if (c == '.') //fraction
                {
                    switch (state)
                    {
                        case ParseState.None:
                            startPosition = pos;
                            break;

                        case ParseState.ValueInteger:
                            state = ParseState.ValueDouble;
                            break;

                        case ParseState.ValueDouble:
                            throw new ParseException($"Unexpected '.' at position {pos + 1}");

                        default:
                            endWord = true;
                            break;
                    }
                }
                else if (c >= 'A' && c <= 'Z') //variables
                {
                    switch (state)
                    {
                        case ParseState.None:
                            startPosition = pos;
                            state = ParseState.Variable;
                            break;

                        case ParseState.Variable:
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
                else if (c == ')')
                {
                    if (state == ParseState.None)
                    {
                        while (true)
                        {
                            if (this.operators.Count == 0)
                            {
                                throw new ParseException("Unexpected ')'. Does not match with any '('");
                            }

                            Token token = this.operators.Pop();
                            if (token is ParentOperator)
                            {
                                break;
                            }

                            this.rpn.Push(token);
                        }
                    }
                    else
                    {
                        endWord = true;
                    }
                }
                else //operator
                {
                    if (state == ParseState.None)
                    {
                        this.PushNewOperator(c);
                    }
                    else
                    {
                        endWord = true;
                    }
                }

                if (endWord)
                {
                    endWord = false;
                    this.PushLongToken(expression, state, startPosition, pos);

                    state = ParseState.None;
                    --pos; //scan the character again
                }
            }

            //finish parsing last token
            this.PushLongToken(expression, state, startPosition, expression.Length);

            //copy remaining operators to RPN stack
            while (this.operators.Count > 0)
            {
                this.rpn.Push(this.operators.Pop());
            }
        }

        private void PushLongToken(string expression, ParseState state, int startPosition, int endPosition)
        {
            string tokenstring = expression.Substring(startPosition, endPosition - startPosition);
            switch (state)
            {
                case ParseState.ValueInteger:
                case ParseState.ValueDouble:
                    this.lastToken = new Value(double.Parse(tokenstring));
                    this.rpn.Push(this.lastToken);
                    break;

                case ParseState.Variable:
                    if (this.Variables.ContainsKey(tokenstring))
                    {
                        this.lastToken = new Value(this.Variables[tokenstring]);
                        this.rpn.Push(this.lastToken);
                    }
                    else
                    {
                        throw new ParseException($"Variable '{tokenstring}' is undefined.");
                    }

                    break;

                case ParseState.Function:
                {
                    if (tokenstring == "m")
                    {
                        this.PushNewOperator(tokenstring[0]);
                    }
                    else
                    {
                        this.PushNewFunction(tokenstring);
                    }

                    break;
                }
            }
        }

        private void PushNewFunction(string name)
        {
            Token op = FunctionFactory.GetToken(name);
            if (this.operators.Count > 0)
            {
                Token top = this.operators.Peek();
                if (top.Priority > op.Priority)
                {
                    while (this.operators.Count > 0)
                    {
                        this.rpn.Push(this.operators.Pop());
                    }
                }
            }

            this.operators.Push(op);
            this.lastToken = op;
        }

        private void PushNewOperator(char c)
        {
            Token op = OperatorFactory.GetToken(c, this.lastToken == null || this.lastToken is Operator);

            if (op.Priority > 0 && this.operators.Count > 0)
            {
                Token top = this.operators.Peek();
                if (top.Priority > op.Priority)
                {
                    while (this.operators.Count > 0)
                    {
                        this.rpn.Push(this.operators.Pop());
                    }
                }
            }

            this.operators.Push(op);
            this.lastToken = op;
        }

        public double Solve()
        {
            var results = new Stack<double>();
            IEnumerable<Token> input = this.rpn.Reverse();

            foreach (Token token in input)
            {
                if (token is Value)
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
                    else
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
            Variable,
            Function
        }
    }
}
