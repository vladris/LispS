using System;
using System.Collections.Generic;
using System.IO;

namespace LispS
{
    class Parser
    {
        private string input;
        private int pos = 0;

        private Parser(string input) => this.input = input;

        public static SExpression Parse(string input)
        {
            var parser = new Parser(input);

            var result = parser.ReadNext();
            parser.SkipWhitespace();

            if (!parser.End())
                throw new ArgumentException();

            return result;
        }

        private bool End() => pos == input.Length;

        private char Peek() => input[pos];

        private char Read() => input[pos++];

        private SExpression ReadNext()
        {
            SkipWhitespace();

            if (End()) return Atom.Nil;

            var next = Peek();

            if (Char.IsDigit(next)) return ReadNumber();
            if (Char.IsLetter(next)) return ReadName();
            if (next == '"') return ReadString();
            if (next == '\'') return ReadQuote();
            if (next == '(') return ReadList();
            if (next == '+' ||
                next == '-' ||
                next == '*' ||
                next == '/') return new Name { Value = Read().ToString() };

            throw new ArgumentException();
        }

        private void Expect(char c)
        {
            if (Read() != c)
                throw new ArgumentException();
        }

        private void SkipWhitespace()
        {
            while (!End() && Char.IsWhiteSpace(Peek()))
                Read();
        }

        private SExpression ReadNumber()
        {
            string token = String.Empty;
            while (!End() && Char.IsDigit(Peek()))
                token += Read();

            return new Atom<int> { Value = Int32.Parse(token) };
        }

        private SExpression ReadString()
        {
            string token = String.Empty;

            Expect('"');
            while (Peek() != '"')
                token += Read();
            Expect('"');

            return new Atom<string> { Value = token };
        }

        private SExpression ReadName()
        {
            string token = String.Empty;
            while (!End() && Char.IsLetterOrDigit(Peek()))
                token += Read();

            return new Name { Value = token };
        }

        private SExpression ReadQuote()
        {
            Expect('\'');

            return new Quote { Quoted = ReadNext() };
        }

        private SExpression ReadList()
        {
            var expressions = new Queue<SExpression>();

            Expect('(');
            SkipWhitespace();
            while (Peek() != ')')
            {
                expressions.Enqueue(ReadNext());
                SkipWhitespace();
            }
            Expect(')');

            return ListToSExpression(expressions);
        }

        private SExpression ListToSExpression(Queue<SExpression> expressions)
        {
            if (expressions.Count == 0) return Atom.Nil;
            if (expressions.Count == 1) return expressions.Dequeue();

            return new List
            {
                Head = expressions.Dequeue(),
                Tail = ListToSExpression(expressions)
            };
        }
    }
}
