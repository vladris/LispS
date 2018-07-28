using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LispS
{
 

    class Program
    {
        static void Main(string[] args)
        {
            var expr = new List
            {
                Head = new Name { Value = "cons" },
                Tail = new List
                {
                    Head = new List
                    {
                        Head = new Name { Value = "cons" },
                        Tail = new List
                        {
                            Head = new Atom<int> { Value = 1 },
                            Tail = new List
                            {
                                Head = new Name { Value = "cons" },
                                Tail = new List
                                {
                                    Head = new Atom<int> { Value = 2 },
                                    Tail = Atom.Nil
                                }
                            }
                        }
                    },
                    Tail = new Atom<int> { Value = 3 }
                }
            };

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

            Console.WriteLine(Printer.PrintExpr(Evaluator.EvalExpr(expr, Context.Make())));
            Console.WriteLine(Printer.PrintExpr(Evaluator.EvalExpr(expr2, Context.Make())));
            Console.ReadLine();
        }
    }
}
