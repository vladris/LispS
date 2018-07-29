using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LispS
{
    class Program
    {
        static void Eval(string expr)
        {
            Console.WriteLine(expr);
            Console.Write("> ");
            Console.WriteLine(Printer.PrintExpr(Evaluator.EvalExpr(Parser.Parse(expr), Context.Make())));
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var ctx = Context.Make();

            while (true)
            {
                Console.Write("> ");
                var expr = Console.ReadLine();
                if (expr == "exit") return;

                try
                {
                    Console.WriteLine(Printer.PrintExpr(Evaluator.EvalExpr(Parser.Parse(expr), ctx)));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
        }
    }
}
