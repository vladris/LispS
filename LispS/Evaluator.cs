using System;
using System.Collections.Generic;
using System.Text;

namespace LispS
{
    // Evaluates an SExpressions with a given context
    class Evaluator
    {
        private static readonly Dictionary<string, Func<int, int, int>> Arithmetic = new Dictionary<string, Func<int, int, int>>
        {
            { "+", (x, y) => x + y },
            { "-", (x, y) => x - y },
            { "*", (x, y) => x * y },
            { "/", (x, y) => x / y },
        };

        public static SExpression EvalExpr(SExpression expr, Context ctx)
            => Eval((dynamic)expr, ctx);

        private static SExpression MakeBool(bool result) => result ? Atom.True : Atom.Nil;

        private static SExpression Eval(Atom atom, Context ctx) => atom;

        private static SExpression Eval(Name atom, Context ctx)
        {
            var resolved = ctx.Resolve(atom);

            if (resolved.Equals(atom)) return resolved;
            if (resolved is Name) return Eval(resolved as Name, ctx);

            return resolved;
        }

        private static SExpression Eval(Quote quote, Context ctx) => quote.Quoted;

        private static SExpression Eval(List list, Context ctx)
            => EvalList((dynamic)list.Head, list.Tail, ctx);

        private static SExpression EvalList(List head, SExpression tail, Context ctx)
            => EvalList((dynamic)Eval(head, ctx), tail, ctx);

        private static SExpression EvalList(Name name, SExpression tail, Context ctx)
        {
            var args = tail as List;

            switch (name.Value)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                    var op1 = (EvalExpr(args.Head, ctx) as Atom<int>).Value;
                    var op2 = (EvalExpr(args.Tail, ctx) as Atom<int>).Value;

                    return new Atom<int> { Value = Arithmetic[name.Value](op1, op2) };
                case "atom":
                    return MakeBool(EvalExpr(tail, ctx) is Atom);
                case "car":
                    return (EvalExpr(tail, ctx) as List).Head;
                case "cdr":
                    return (EvalExpr(tail, ctx) as List).Tail;
                case "cons":
                    return new List
                    {
                        Head = EvalExpr(args.Head, ctx),
                        Tail = EvalExpr(args.Tail, ctx)
                    };
                case "eq":
                    return MakeBool(EvalExpr(args.Head, ctx).Equals(EvalExpr(args.Tail, ctx)));
                case "eval":
                    return EvalExpr(EvalExpr(tail, ctx), ctx);
                case "if":
                    if (EvalExpr(args.Head, ctx) != Atom.Nil)
                        return EvalExpr((args.Tail as List).Head, ctx);
                    else
                        return EvalExpr((args.Tail as List).Tail, ctx);
                case "lambda":
                    return new Lambda
                    {
                        Head = args.Head,
                        Tail = args.Tail,
                        Context = ctx
                    };
                case "store":
                    var stored = ctx.TryResolve(args.Head as Name);
                    return ctx.Root().Store(stored as Name ?? args.Head as Name, EvalExpr(args.Tail, ctx));
                default:
                    return Eval(new List
                    {
                        Head = ctx.Resolve(name),
                        Tail = tail
                    }, ctx);
            }
        }

        private static SExpression EvalList(Lambda lambda, SExpression tail, Context ctx)
        {
            var scope = lambda.Context.Push();
            SExpression args = lambda.Head;
            while (args is List)
            {
                scope.Store((args as List).Head as Name, EvalExpr((tail as List).Head, ctx));
                args = (args as List).Tail;
                tail = (tail as List).Tail;
            }
            scope.Store(args as Name, EvalExpr(tail, ctx));

            return EvalExpr(lambda.Tail, scope);
        }
    }
}
