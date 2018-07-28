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

            var expr2 = new List
            {
                Head = new Lambda
                {
                    Head = new Name { Value = "x" },
                    Tail = new Lambda
                    {
                        Head = new Name { Value = "y" },
                        Tail = new List
                        {
                            Head = new Name { Value = "cons" },
                            Tail = new List
                            {
                                Head = new Name { Value = "x" },
                                Tail = new Name { Value = "y" }
                            }
                        }
                    }
                },
                Tail = new List
                {
                    Head = new Atom<int> { Value = 1 },
                    Tail = new List
                    {
                        Head = new Atom<int> { Value = 2 },
                        Tail = Atom.Nil
                    }
                }
            };

            Console.WriteLine(Printer.PrintExpr(Evaluator.EvalExpr(expr2, Context.Make())));
            Console.ReadLine();
        }
    }
}
