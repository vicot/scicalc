using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SciCalc.Tokens;
using SciCalc.Tokens.Functions;
using SciCalc.Tokens.Operators;

namespace SciCalc {
    public class Parser {
        public Parser()
        {
            Variables = new Dictionary<string, double>()
            {
                {"PI", Math.PI},
                {"E", Math.E},
            };
            rpn = new Stack<Token>();
            operators = new Stack<Token>();
        }

        public Dictionary<string, double> Variables { get; private set; }

        private readonly Stack<Token> rpn;
        private readonly Stack<Token> operators;
        private Token lastToken;
        

        public void SetVariable(string name, double value)
        {
            Variables[name] = value;
        }

        public bool UnsetVariable(string name)
        {
            return Variables.Remove(name);
        }

        private enum ParseState {
            None,
            ValueInteger,
            ValueDouble,
            Variable,
            Function,
        }

        public void Parse(string expression)
        {
            operators.Clear();
            rpn.Clear();
            lastToken = null;

            ParseState state = ParseState.None;
            int startPosition = 0;
            bool endWord = false;


            for (var pos = 0; pos < expression.Length; ++pos)
            {
                var c = expression[pos];
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
                            if (operators.Count == 0)
                            {
                                throw new ParseException("Unexpected ')'. Does not match with any '('");
                            }

                            var token = operators.Pop();
                            if (token is ParentOperator) break;
                            rpn.Push(token);
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
                        PushNewOperator(c);
                    }
                    else
                    {
                        endWord = true;
                    }
                }

                if (endWord)
                {
                    endWord = false;
                    PushLongToken(expression, state, startPosition, pos);

                    state = ParseState.None;
                    --pos; //scan the character again
                }

            }

            //finish parsing last token
            PushLongToken(expression, state, startPosition, expression.Length);

            //copy remaining operators to RPN stack
            while (operators.Count > 0) rpn.Push(operators.Pop());

        }

        private void PushLongToken(string expression, ParseState state, int startPosition, int endPosition)
        {
            var tokenstring = expression.Substring(startPosition, endPosition - startPosition);
            switch (state)
            {
                case ParseState.ValueInteger:
                case ParseState.ValueDouble:
                    lastToken = new Value(double.Parse(tokenstring));
                    rpn.Push(lastToken);
                    break;

                case ParseState.Variable:
                    if (Variables.ContainsKey(tokenstring))
                    {
                        lastToken = new Value(Variables[tokenstring]);
                        rpn.Push(lastToken);
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
                        PushNewOperator(tokenstring[0]);
                    }
                    else
                    {
                        PushNewFunction(tokenstring);
                    }

                    break;
                }
            }
        }

        private void PushNewFunction(string name)
        {
            var op = FunctionFactory.GetToken(name);
            if (operators.Count > 0)
            {
                var top = operators.Peek();
                if (top.Priority > op.Priority)
                {
                    while (operators.Count > 0) rpn.Push(operators.Pop());
                }
            }

            operators.Push(op);
            lastToken = op;
        }

        private void PushNewOperator(char c)
        {
            var op = OperatorFactory.GetToken(c, lastToken == null || lastToken is Operator);

            if (op.Priority > 0 && operators.Count > 0)
            {
                var top = operators.Peek();
                if (top.Priority > op.Priority)
                {
                    while (operators.Count > 0) rpn.Push(operators.Pop());
                }
            }

            operators.Push(op);
            lastToken = op;
        }

        public double Solve()
        {
            var results = new Stack<double>();
            var input = rpn.Reverse();

            foreach (var token in input)
            {
                if (token is Value)
                {
                    results.Push(token.Value);
                }
                else if(token is Operator || token is Function)
                {
                    if (token.ArgumentCount == 1)
                    {
                        var arg = results.Pop();
                        results.Push(token.Execute(arg));
                    }
                    else
                    {
                        var arg2 = results.Pop();
                        var arg1 = results.Pop();
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
    }

}
