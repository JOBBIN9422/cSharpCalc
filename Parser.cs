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
                //purely numeric strings are operands
                else if (stringIsNumeric(s))
                {
                    inputTokens.Add(new Operand(s));
                }
                else if (stringIsOperator(s))
                {
                    inputTokens.Add(new Operator(s));
                }

                //strings which contain parentheses AND operands must be further tokenized
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

            //loop over the tokenized infix expression from user input
            try
            {
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

                        //if current operator is a ')', pop from stack and add to output until '(' is encountered 
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

                            while ((!(operatorStack.Count() == 0)) && ((currStackToken = (Operator)operatorStack.Peek()).getPriority() >= currOpToken.getPriority()))
                            {
                                //add tokens from stack to output expression while the current token has <= priority than top of stack
                                currStackToken = operatorStack.Pop() as Operator;
                                postfixExpr.Add(currStackToken);
                            }

                            operatorStack.Push(currToken);
                        }
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                return new ArrayList();
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
            try
            {
                foreach (Token currToken in postfixExpr)
                {
                    if (currToken is Operand)
                    {
                        //move operands onto the evaluation stack
                        evalStack.Push((Operand)currToken);
                    }
                    else
                    {
                        //if we have an operator, remove two operands from the stack
                        Operand token1 = evalStack.Pop();
                        Operand token2 = evalStack.Pop();

                        double value1 = Convert.ToDouble(token1.getValue());
                        double value2 = Convert.ToDouble(token2.getValue());

                        //evaluate the two operands based on the current operator
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

                //pop the final result from the stack
                this.lastAns = Convert.ToDouble(evalStack.Pop().getValue());
            }
            catch (System.InvalidOperationException)
            {
                this.inputTokens.Clear();
                Console.WriteLine("Invalid expression/command");
                this.lastAns = Double.NaN;
            }
            
            return this.lastAns;
        }

        //helper functions
        public bool stringIsNumeric(string str)
        {
            return double.TryParse(str, out double value);
        }

        public bool stringIsOperator(string str)
        {
            //all operators (currently) are 1 character long
            if (str.Length > 1)
            {
                return false;
            }

            //check the input against all valid operators
            foreach (char currOperator in validOperators)
            {
                if (str.Equals(currOperator.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool opIsRightAssociative(Operator op)
        {
            return (op.getValue().Equals("^"));
        }
        public bool stringHasParens(string str)
        {
            return (str.Contains('(') || str.Contains(')'));
        }
    }
}
