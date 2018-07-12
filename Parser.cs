using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Parser
    {
        private ArrayList inputTokens;
        private ArrayList validOperators;
        private double lastAns;

        public Parser()
        {
            inputTokens = new ArrayList();
            validOperators = new ArrayList(new char[] {'+', '-', '*', '/', '^', '(', ')'});
        }

        public void tokenizeExpression(string expression)
        {
            //initial expression parse - separate into strings by spaces
            ArrayList stringTokens = new ArrayList(expression.Split(new char[] {' '}));

            foreach (string s in stringTokens)
            {
                //replace 'ans' keyword with last calculated answer
                if (s.Equals("ans"))
                {
                    inputTokens.Add(new Operand(lastAns.ToString()));
                }
                //additionally parse each expression substring 
                if (stringIsNumeric(s))
                {
                    //purely numeric strings are operands
                    inputTokens.Add(new Operand(s));
                }
                else if (stringIsOperator(s))
                {
                    inputTokens.Add(new Operator(s));
                }

                //strings which contain parentheses AND operands
                else if (stringHasParens(s))
                {   
                    for (int i = 0; i < s.Length; i++)
                    {
                        char currentChar = s[i];
                        //separate parenthesis as individual tokens
                        if (currentChar.Equals('(') || currentChar.Equals(')'))
                        {
                            inputTokens.Add(new Operator(currentChar.ToString()));
                        }
                        else
                        {
                            //separate operands as individual tokens (remove parentheses)
                            string remainingStr = s.Substring(i).Trim('(').Trim(')');
                            inputTokens.Add(new Operand(remainingStr));

                            //advance loop index by # of digits in operand so we don't process it more than once
                            i += remainingStr.Length - 1;
                        }
                    }
                }
            }
        }

        public ArrayList InputToPostfix()
        {
            Stack<Token> operatorStack = new Stack<Token>();
            ArrayList postfixExpr = new ArrayList();

            foreach (Token currToken in inputTokens)
            {
                //if current token is an operand, add to output expression
                if (currToken is Operand)
                {
                    postfixExpr.Add(currToken);
                }
                else if (currToken is Operator)
                {
                    //if current operator is a '(', add it to op stack
                    if (currToken.getValue().Equals("("))
                    {
                        operatorStack.Push(currToken);
                    }

                    //if current operator is a ')', pop from stack and add to expression until '(' is encountered 
                    else if (currToken.getValue().Equals(")"))
                    {
                        Token topStackToken = operatorStack.Pop();
                        while (!(topStackToken.getValue().Equals("(")))
                        {
                            postfixExpr.Add(topStackToken);
                            topStackToken = operatorStack.Pop();
                        }
                    }

                    //operators that are not parentheses
                    else
                    {
                        Operator currOpToken = currToken as Operator;
                        Operator currStackToken = new Operator("");
                        while (!(operatorStack.Count() == 0) && currOpToken.getPriority() <= currStackToken.getPriority())
                        {
                            //add tokens from stack to output expression while the current token has <= priority than top of stack
                            currStackToken = operatorStack.Pop() as Operator;
                            postfixExpr.Add(currStackToken);
                        }

                        operatorStack.Push(currToken);
                    }
                }
            }

            //add remainder of stack to postfix expression
            while (!(operatorStack.Count() == 0))
            {
                postfixExpr.Add(operatorStack.Pop());
            }

            return postfixExpr;
        }

        public double evaluatePostfix(ArrayList postfixExpr)
        {
            Stack<Operand> evalStack = new Stack<Operand>();

            foreach (Token currToken in postfixExpr)
            {
                if (currToken is Operand)
                {
                    evalStack.Push((Operand)currToken);
                }
                else
                {
                    Operand token1 = new Operand("");
                    Operand token2 = new Operand("");
                    try
                    {
                        token1 = evalStack.Pop();
                        token2 = evalStack.Pop();
                    }

                    catch (System.InvalidOperationException)
                    {
                        Console.WriteLine("Invalid expression/command");
                        return -1;
                    }

                    double value1 = Convert.ToDouble(token1.getValue());
                    double value2 = Convert.ToDouble(token2.getValue());

                    double result;
                    switch (currToken.getValue())
                    {
                        case "+":
                            result = value2 + value1;
                            evalStack.Push(new Operand(result.ToString()));
                            break;
                        case "-":
                            result = value2 - value1;
                            evalStack.Push(new Operand(result.ToString()));
                            break;
                        case "*":
                            result = value2 * value1;
                            evalStack.Push(new Operand(result.ToString()));
                            break;
                        case "/":
                            result = value2 / value1;
                            evalStack.Push(new Operand(result.ToString()));
                            break;
                        case "^":
                            result = Math.Pow(value2, value1);
                            evalStack.Push(new Operand(result.ToString()));
                            break;
                    }
                }
            }

            //update calculator state info (store answer and clear expression buffer)
            this.inputTokens.Clear();
            try
            {
                this.lastAns = Convert.ToDouble(evalStack.Pop().getValue());
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("Invalid expression/command");
            }
            
            return this.lastAns;
        }

        //helper functions
        public bool stringIsNumeric(string str)
        {
            double value;
            return double.TryParse(str, out value);
        }

        public bool stringIsOperator(string str)
        {
            if (str.Length > 1)
            {
                return false;
            }

            foreach (char currOperator in validOperators)
            {
                if (str.Equals(currOperator.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool stringHasParens(string str)
        {
            return (str.Contains('(') || str.Contains(')'));
        }
    }
}
