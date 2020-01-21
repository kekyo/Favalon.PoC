using Favalon.Terms.Contexts;
using Favalon.Terms;
using System;
using System.Diagnostics;

#pragma warning disable 659

namespace Favalon
{
    public abstract partial class Term
    {
        protected Term()
        { }

        public abstract Term HigherOrder { get; }

        internal virtual bool ValidTerm =>
            true;

        internal bool SpecifiedTerm =>
            this.ValidTerm && !(this is UnspecifiedTerm);

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
        protected abstract bool OnEquals(EqualsContext context, Term? other);

        public bool Equals(EqualsContext context, Term? other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (!this.ValidTerm || !(other?.ValidTerm ?? false))
            {
                return false;
            }

            if (!this.OnEquals(context, other))
            {
                return false;
            }

            if (!context.IncludeHigherOrder)
            {
                return true;
            }

            if (this.HigherOrder is Term && other?.HigherOrder is Term)
            {
                var result = this.HigherOrder.Equals(context, other?.HigherOrder);
                return result;
            }

            return false;
        }

        public bool Equals(Term? other) =>
            this.Equals(new EqualsContext(false), other);

        public bool EqualsWithHigherOrder(Term? other) =>
            this.Equals(new EqualsContext(true), other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);
    }

    [DebuggerDisplay("{Specified}")]
    partial class Term
    {
        protected abstract string OnPrettyPrint(PrettyPrintContext context);

        protected virtual bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            higherOrderDetail switch
            {
                HigherOrderDetails.None => false,
                HigherOrderDetails.Specified => this.HigherOrder.SpecifiedTerm,
                _ => this.HigherOrder.ValidTerm,
            };

        public string PrettyPrint(PrettyPrintContext context) =>
            (this.HigherOrder, this.IsIncludeHigherOrderInPrettyPrinting(context.HigherOrderDetail), this) switch
            {
                (Term _, true, _) =>
                    $"({this.OnPrettyPrint(context)}):{this.HigherOrder.PrettyPrint(context.DropSpecified())}",
                (_, _, IRightToLeftPrettyPrintingTerm _) =>
                    $"({this.OnPrettyPrint(context)})",
                _ =>
                    this.OnPrettyPrint(context)
            };

        public string Simple =>
            this.PrettyPrint(new PrettyPrintContext(HigherOrderDetails.None));
        public string Specified =>
            this.PrettyPrint(new PrettyPrintContext(HigherOrderDetails.Specified));
        public string Full =>
            this.PrettyPrint(new PrettyPrintContext(HigherOrderDetails.Full));
    }
}
