using System;
using System.Collections.Generic;
using System.Text;

namespace LispS
{
    // Evaluates an SExpressions with a given context
    class Evaluator
    {
        public static SExpression EvalExpr(SExpression expr, Context ctx)
            => Eval((dynamic)expr, ctx);

        private static SExpression MakeBool(bool result) => result ? Atom.True : Atom.Nil;

        private static SExpression Eval(Atom atom, Context ctx) => atom;

        private static SExpression Eval(Name atom, Context ctx) => ctx.Resolve(atom);

        private static SExpression Eval(Quote quote, Context ctx) => quote.Quoted;

        private static SExpression Eval(List list, Context ctx)
            => EvalList((dynamic)list.Head, list.Tail, ctx);

        private static SExpression EvalList(Name name, SExpression tail, Context ctx)
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
                    return new List
                    {
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

        private static SExpression EvalList(Lambda lambda, SExpression tail, Context ctx)
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
}
