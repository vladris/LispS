using System;
using System.Collections.Generic;
using System.Text;

namespace LispS
{
    // Abstract base
    abstract class SExpression
    {
    }

    // Atom base, provides "True" and "Nil" atoms
    class Atom : SExpression
    {
        public static SExpression True = new Atom();
        public static SExpression Nil = new Atom();

        protected Atom() { }
    }

    // Atom value
    class Atom<T> : Atom
    {
        public T Value { get; set; }

        public override bool Equals(object obj) => (obj as Atom<T>)?.Value.Equals(Value) ?? base.Equals(obj);
    }

    // Name is a special atom (meaningful for the runtime)
    class Name : Atom<string>
    { }

    // Quoted expression
    class Quote : SExpression
    {
        public SExpression Quoted { get; set; }
    }

    // List is a pair of SExpressions
    class List : SExpression
    {
        public SExpression Head { get; set; }
        public SExpression Tail { get; set; }
    }

    // Lambda is a special list (meaningful for the runtime)
    class Lambda : List
    { }
}
