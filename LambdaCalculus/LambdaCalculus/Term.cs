using Favalon.Contexts;
using Favalon.Terms;
using LambdaCalculus.Contexts;
using System;
using System.Diagnostics;

namespace Favalon
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract partial class TermBase
    {
        private protected TermBase()
        { }

        public abstract Term HigherOrder { get; }

        public void Deconstruct(out Term higherOrder) =>
            higherOrder = this.HigherOrder;

        protected abstract string OnPrettyPrint(PrettyPrintContext context);

        protected virtual bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            higherOrderDetail switch
            {
                HigherOrderDetails.None => false,
                HigherOrderDetails.Full => true,
                _ => !(this.HigherOrder is UnspecifiedTerm)
            };

        public string PrettyPrint(PrettyPrintContext context) =>
            (this.HigherOrder, this.IsInclude(context.HigherOrderDetail), this) switch
            {
                (Term _, true, _) =>
                    $"({this.OnPrettyPrint(context)}):{this.HigherOrder.PrettyPrint(context.DropReadable())}",
                (_, _, IRightToLeftPrettyPrintingTerm _) =>
                    $"({this.OnPrettyPrint(context)})",
                _ =>
                    this.OnPrettyPrint(context)
            };

        public string DebuggerDisplay =>
            this.PrettyPrint(new PrettyPrintContext(HigherOrderDetails.Readable));

        public override string ToString() =>
            this.PrettyPrint(new PrettyPrintContext(HigherOrderDetails.None));
    }

#pragma warning disable 659

    public abstract partial class Term : TermBase, IEquatable<Term?>
    {
        protected Term()
        { }

        public abstract Term Infer(InferContext context);

        public abstract Term Fixup(FixupContext context);

        public abstract Term Reduce(ReduceContext context);

        public abstract bool Equals(Term? other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);
    }
}
