using Favalon.Terms;
using System;

namespace Favalon.ParseRunners
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
        internal override Term VisitUnveil() =>
            this.Term.VisitUnveil();

        protected internal override string VisitTermString(bool includeTermName) =>
            $"Hide({this.Term.VisitTermString(includeTermName)})";

        protected internal override Term VisitReduce(Context context) =>
            throw new InvalidOperationException();

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            throw new InvalidOperationException();
    }
}
