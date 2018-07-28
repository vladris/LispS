using System;

namespace LispS
{
    // Pretty-printing for SExpressions
    class Printer
    {
        public static string PrintExpr(SExpression expr) => Print((dynamic)expr);

        private static string Print(Atom atom) => atom == Atom.True ? ":true" :
                                                  atom == Atom.Nil ? "()" :
                                                  throw new ArgumentException();

        private static string Print(Atom<int> number) => number.Value.ToString();

        private static string Print(Atom<string> name) => name.Value;

        private static string Print(Quote quote) => $"'{PrintExpr(quote.Quoted)}";

        private static string Print(List list) => $"({PrintExpr(list.Head)} . {PrintExpr(list.Tail)})";

        private static string Print(Lambda lambda) => $"({PrintExpr(lambda.Head)} => ({PrintExpr(lambda.Tail)})";
    }
}
