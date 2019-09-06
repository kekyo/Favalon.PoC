using Favalon.Terms;
using Favalon.Tokens;
using System;

namespace Favalon.Internals
{
    internal sealed class ParserCore
    {
        // -> a b c "aaa"
        // Var(->) Var(a) Var(b) Var(c) Str(aaa)
        // App(App(App(App(Var(->) Var(a)) Var(b)) Var(c)) Str(aaa))

        // "aaa" -> a b c
        // Str(aaa) Var(a) Var(b) Var(c)
        // Str(aaa) App(App(App(Var(->) Var(a)) Var(b)) Var(c))

        private Term? lastTerm;

        public Term? Examine(Token token)
        {
            switch (token)
            {
                case NumericToken _:
                    if (this.lastTerm is Term lastTerm1)
                    {
                        this.lastTerm = new ApplyTerm(lastTerm1, new NumericTerm(token.Value));
                        return null;
                    }
                    else
                    {
                        return new NumericTerm(token.Value);
                    }
                case StringToken _:
                    if (this.lastTerm is Term lastTerm2)
                    {
                        this.lastTerm = new ApplyTerm(lastTerm2, new StringTerm(token.Value));
                        return null;
                    }
                    else
                    {
                        return new StringTerm(token.Value);
                    }
                case IVariableToken variable:
                    if (variable.Value == "true")
                    {
                        return new BooleanTerm(true);
                    }
                    else if (variable.Value == "false")
                    {
                        return new BooleanTerm(false);
                    }
                    if (this.lastTerm is Term lastTerm3)
                    {
                        this.lastTerm = new ApplyTerm(lastTerm3, new VariableTerm(variable.Value));
                    }
                    else
                    {
                        this.lastTerm = new VariableTerm(variable.Value);
                    }
                    return null;
                default:
                    throw new Exception();
            }
        }

        public Term? Flush()
        {
            if (this.lastTerm is Term lastTerm)
            {
                this.lastTerm = null;
                return lastTerm;
            }
            else
            {
                return null;
            }
        }
    }
}
