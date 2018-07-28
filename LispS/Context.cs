using System;
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
            => Symbols.ContainsKey(name.Value) ? Symbols[name.Value] : Parent?.Resolve(name);
    }
}
