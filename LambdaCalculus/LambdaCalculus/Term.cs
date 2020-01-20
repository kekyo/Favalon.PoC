using Favalon.Contexts;
using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#pragma warning disable 659

namespace Favalon
{
    public abstract partial class Term
    {
        protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public void Deconstruct(out Term higherOrder) =>
            higherOrder = this.HigherOrder;

        public abstract Term Infer(InferContext context);

        public abstract Term Fixup(FixupContext context);

        public abstract Term Reduce(ReduceContext context);

        public override string ToString() =>
            this.PrettyPrint(new PrettyPrintContext(HigherOrderDetails.None));
    }

    partial class Term : IEquatable<Term?>
    {
        protected abstract bool OnEquals(Term? other);

        public bool Equals(Term? other, bool includeHigherOrder = false) =>
            object.ReferenceEquals(this, other) ||
            (this.OnEquals(other) &&
                (!includeHigherOrder ||
                (this.HigherOrder is Term && other?.HigherOrder is Term) ?
                    this.HigherOrder.Equals(other?.HigherOrder, true) :
                    false));

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    partial class Term
    {
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
    }
}
