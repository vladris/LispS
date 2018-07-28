using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LispS
{
    abstract class SExpression
    {
    }

    class Atom : SExpression
    {
        public static SExpression True = new Atom<string> { Value = "true" };

        public static SExpression Nil = new Atom();

        protected Atom() { }
    }

    class Atom<T> : Atom
    {
        public T Value { get; set; }

        public override bool Equals(object obj)
            => (obj as Atom<T>)?.Value.Equals(Value) ?? base.Equals(obj);
    }

    class Name : Atom<string>
    { }

    class Quote : SExpression
    {
        public SExpression Quoted { get; set; }
    }

    class List : SExpression
    {
        public SExpression Head { get; set; }
        public SExpression Tail { get; set; }
    }

    class Lambda : List
    { }

    class Context
    {
        private Dictionary<string, SExpression> Symbols { get; } = new Dictionary<string, SExpression>();
        private Context Parent { get; }

        private Context(Context parent = null) => Parent = parent;

        public static Context Make() => new Context();

        public Context Push() => new Context(this);

        public SExpression Store(Name name, SExpression symbol)
        {
            Symbols[name.Value] = symbol;
            return name;
        }

        public SExpression Resolve(Name name)
            => Symbols.ContainsKey(name.Value) ? Symbols[name.Value] : Parent?.Resolve(name);
    }

    class Evaluator
    {
        public static SExpression EvalExpr(SExpression expr, Context ctx)
            => Eval((dynamic)expr, ctx);

        public static SExpression MakeBool(bool result)
            => result ? Atom.True : Atom.Nil;

        private static SExpression Eval(Atom atom, Context ctx) => atom;

        private static SExpression Eval(Name atom, Context ctx) => ctx.Resolve(atom);

        private static SExpression Eval(Quote quote, Context ctx) => quote.Quoted;

        private static SExpression Eval(List list, Context ctx)
            => EvalList((dynamic)list.Head, list.Tail, ctx);

        public static SExpression EvalList(Name name, SExpression tail, Context ctx)
        {
            var args = tail as List;

            switch (name.Value)
            {
                case "atom":
                    return MakeBool(EvalExpr(tail, ctx) is Atom);
                case "car":
                    return EvalExpr(args.Head, ctx);
                case "cdr":
                    return EvalExpr(args.Tail, ctx);
                case "cons":
                    return new List {
                        Head = EvalExpr(args.Head, ctx),
                        Tail = EvalExpr(args.Tail, ctx)
                    };
                case "eq":
                    return MakeBool(EvalExpr(args.Head, ctx).Equals(EvalExpr(args.Tail, ctx)));
                case "eval":
                    return EvalExpr(args, ctx);
                case "if":
                    if (EvalExpr(args.Head, ctx) != Atom.Nil)
                        return EvalExpr((args.Tail as List).Head, ctx);
                    else
                        return EvalExpr((args.Tail as List).Tail, ctx);
                case "quote":
                    return new Quote { Quoted = tail };
                case "store":
                    return ctx.Store(EvalExpr(args.Head, ctx) as Name, EvalExpr(args.Tail, ctx));
                default:
                    return EvalList((dynamic)ctx.Resolve(name), args, ctx);
            }
        }

        public static SExpression EvalList(Lambda lambda, SExpression tail, Context ctx)
        {
            var scope = ctx.Push();
            SExpression l = lambda;
            while (l is Lambda)
            {
                scope.Store((l as Lambda).Head as Name, EvalExpr((tail as List).Head, ctx));
                l = (l as Lambda).Tail;
                tail = (tail as List).Tail;
            }

            return EvalExpr(l, scope);
        }
    }

    class Printer
    {
        public static string PrintExpr(SExpression expr) => Print((dynamic)expr);

        private static string Print(Atom atom) => atom == Atom.True ? "T" :
                                                  atom == Atom.Nil ? "()" :
                                                  throw new InvalidDataException();

        private static string Print(Atom<int> number) => number.Value.ToString();

        private static string Print(Atom<string> name) => name.Value;

        private static string Print(Quote quote) => PrintExpr(quote);

        private static string Print(List list) => $"({PrintExpr(list.Head)} . {PrintExpr(list.Tail)})";
    }

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
