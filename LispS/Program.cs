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
            /*
            Eval("(car (cons (cons 1 (cons 2 ())) 3))");
            Eval("(car (cons 1 2))");
            Eval("(cdr (cons 1 2))");
            Eval("(atom 5)");
            Eval("(atom ())");
            Eval("(atom (cons 1 2))");
            Eval("(atom (car (cons 1 2)))");
            Eval("(eq 5 5)");
            Eval("(eq 5 6)");
            Eval("(if (eq 5 4) (cons 1 2) (cons 2 3))");
            Eval("'(cons 1 2)");
            Eval("(eval '(cons 1 2))");
            Eval("((lambda (x y) (cons x y)) 1 2)");
            Eval("((lambda (x y z) (cons (cons x z) y)) 1 2 3)");
            Eval("(store pair (lambda (x y) (cons x y)))");
            */

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
