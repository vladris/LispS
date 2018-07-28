using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LispS
{
    abstract class SExpression
    {
        public virtual bool IsAtom() => false;

        public virtual bool IsNull() => false;

        public override bool Equals(object obj)
            => obj is SExpression ? this.IsNull() && ((SExpression)obj).IsNull() : false;
    }

    class Atom<T> : SExpression
    {
        public T Value { get; set; }

        public override bool IsAtom() => true;

        public override bool Equals(object obj)
            => (obj as Atom<T>)?.Value.Equals(Value) ?? base.Equals(obj);
    }

    class Quote : SExpression
    {
        public SExpression Quoted { get; set; }

        public override bool IsAtom() => Quoted.IsAtom();
    }

    class List : SExpression
    {
        public SExpression Head { get; set; }
        public SExpression Tail { get; set; }

        public override bool IsAtom() => IsNull();

        public override bool IsNull() => Head == null;
    }

    class Context
    {
        private Dictionary<string, SExpression> Symbols { get; } = new Dictionary<string, SExpression>();
        private Context Parent { get; }

        private Context(Context parent = null) => Parent = parent;

        public static Context Make() => new Context();

        public Context Push() => new Context(this);

        public void Store(string name, SExpression symbol) => Symbols[name] = symbol;

        public SExpression Resolve(string name) => Symbols.ContainsKey(name) ? Symbols[name] : Parent?.Resolve(name);
    }

    class Evaluator
    {
        public static SExpression EvalExpr(SExpression expr, Context ctx)
            => Eval((dynamic)expr, ctx);

        private static SExpression Eval<T>(Atom<T> atom, Context ctx)
            => atom;

        private static SExpression Eval(Quote quote, Context ctx)
            => quote.Quoted;

        private static SExpression Eval(List list, Context ctx)
        {
            if (list.IsNull()) return list;

            var scope = ctx.Push();
            var name = (EvalExpr(list.Head, ctx) as Atom<string>).Value;
            var args = list.Tail;

            switch (name)
            {
                case "atom":
                    return EvalExpr(args, ctx).IsAtom() ? new Atom<string> { Value = "T" } as SExpression: new List();
                case "car":
                    return EvalExpr((args as List).Head, ctx);
                case "cdr":
                    return EvalExpr((args as List).Tail, ctx);
                case "cons":
                    return new List {
                        Head = EvalExpr((args as List).Head, ctx),
                        Tail = EvalExpr((args as List).Tail, ctx)
                    };
                case "if":
                    var expr = args as List;
                    if (!EvalExpr(expr.Head, ctx).IsNull())
                        return EvalExpr((expr.Tail as List).Head, ctx);
                    else
                        return EvalExpr((expr.Tail as List).Tail, ctx);
                case "quote":
                    return new Quote { Quoted = args };
                case "store":
                    var storeSymbol = EvalExpr(list.Head, ctx) as Atom<string>;
                    ctx.Store(storeSymbol.Value, list.Tail);
                    return storeSymbol;
                case "resolve":
                    var callSymbol = EvalExpr(list.Head, ctx) as Atom<string>;
                    return EvalExpr(ctx.Resolve(callSymbol.Value), ctx);
                default:
                    throw new InvalidProgramException();
            }
        }
    }

    class Printer
    {
        public static string PrintExpr(SExpression expr) => Print((dynamic)expr);

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
                Head = new Quote
                {
                    Quoted = new Atom<string> { Value = "cons" }
                },
                Tail = new List
                {
                    Head = new List
                    {
                        Head = new Atom<string> { Value = "cons" },
                        Tail = new List
                        {
                            Head = new Atom<int> { Value = 1 },
                            Tail = new Atom<int> { Value = 2 }
                        }
                    },
                    Tail = new Atom<int> { Value = 3 }
                }
            };

            Console.WriteLine(Printer.PrintExpr(Evaluator.EvalExpr(expr, Context.Make())));
            Console.ReadLine();
        }
    }
}
