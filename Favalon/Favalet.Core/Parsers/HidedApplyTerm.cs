using Favalet.Expressions;
using System;

namespace Favalet.Parsers
{
    // HidedApplyTerm is pseudo shadowing term, will hide from ApplyTerm matcher.
    // Unveil all real ApplyTerm by VisitUnveil method when parser is finished.
    // See ParserUtilities.RunIdentity() and ApplyTerm.VisitUnveil().
    internal sealed class HidedApplyTerm : Term
    {
        public readonly ApplyTerm Term;

        public HidedApplyTerm(ApplyTerm hideTerm) =>
            this.Term = hideTerm;

        // Unveil real term.
        internal override IExpression VisitUnveil() =>
            this.Term.VisitUnveil();

        protected internal override string VisitTermString(bool includeTermName) =>
            $"Hide({this.Term.VisitTermString(includeTermName)})";

        protected internal override IExpression VisitReduce(Context context) =>
            throw new InvalidOperationException();

        protected internal override IExpression VisitReplace(string identity, IExpression replacement) =>
            throw new InvalidOperationException();
    }
}
