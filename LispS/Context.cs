﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LispS
{
    // Execution context
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
            => Get(name) ?? throw new ArgumentException($"Unknown <{name.Value}>");

        public SExpression TryResolve(Name name)
            => Get(name) ?? name;

        private SExpression Get(Name name)
            => Symbols.ContainsKey(name.Value) ? Symbols[name.Value] : Parent?.Resolve(name);

        public Context Root()
            => Parent == null ? this : Parent.Root();
    }
}
