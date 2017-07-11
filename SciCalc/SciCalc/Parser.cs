using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SciCalc.Tokens;
using SciCalc.Tokens.Operators;

namespace SciCalc {
    public class Parser {
        public Parser()
        {
            Variables = new Dictionary<string, double>();
            rpn = new Stack<Token>();
            operators = new Stack<Token>();
        }

        public Dictionary<string, double> Variables { get; private set; }

        private Stack<Token> rpn;
        private Stack<Token> operators;
        

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
                    throw new NotImplementedException("Variables are not supported yet");
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
                else //operator
                {
                    if (state == ParseState.None)
                    {

                        var op = OperatorFactory.GetToken(c);
                        if (operators.Count > 0)
                        {
                            var top = operators.Peek();
                            if (top.Priority > op.Priority)
                            {
                                while (operators.Count > 0) rpn.Push(operators.Pop());
                            }

                        }

                        operators.Push(op);
                    }
                    else
                    {
                        endWord = true;
                    }
                }

                if (endWord)
                {
                    endWord = false;
                    switch (state)
                    {
                        case ParseState.ValueInteger:
                        case ParseState.ValueDouble:
                            rpn.Push(new Value(double.Parse(expression.Substring(startPosition, pos - startPosition))));
                            break;


                    }

                    state = ParseState.None;
                    --pos; //scan the character again
                }

            }

            //finish parsing last token
            switch (state) {
                case ParseState.ValueInteger:
                case ParseState.ValueDouble:
                    rpn.Push(new Value(double.Parse(expression.Substring(startPosition, expression.Length - startPosition))));
                    break;


            }

            //copy remaining operators to RPN stack
            while (operators.Count > 0) rpn.Push(operators.Pop());

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
                else if(token is Operator)
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
