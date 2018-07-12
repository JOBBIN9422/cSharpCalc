using System;
using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();

            string testExpr = String.Empty;
            while (true)
            {
                Console.Write("> ");
                testExpr = Console.ReadLine();
                if (exitCommand(testExpr))
                {
                    break;
                }
                else if (clearCommand(testExpr))
                {
                    Console.Clear();
                    continue;
                }
                else
                {
                    parser.tokenizeExpression(testExpr);

                    ArrayList postfixTest = parser.InputToPostfix();

                    Console.WriteLine("= " + parser.evaluatePostfix(postfixTest));
                }
            }
        }

        //helper functions for non-arithmetic commands 
        public static bool exitCommand(string command)
        {
            return (command.Equals("q") || command.Equals("quit") || command.Equals("exit"));
        }

        public static bool clearCommand(string command)
        {
            return (command.Equals("clear") || command.Equals("clc") || command.Equals("cls"));
        }
    }
}

    
