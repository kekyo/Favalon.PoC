using Favalon.Terms;
using System;

namespace Favalon.ParseRunners
{
    // HideTerm is pseudo shadowing term, will hide from ApplyTerm matcher.
    // Unveil inner term when parser is finished.
    // See ParserUtilities.RunIdentity() and ParserUtilities.FinalizeHideTerm().
    internal sealed class HideTerm : Term
    {
        public readonly Term Term;

        public HideTerm(Term hideTerm) =>
            this.Term = hideTerm;

        protected internal override string VisitTermString(bool includeTermName) =>
            $"Hide({this.Term.VisitTermString(includeTermName)})";

        protected internal override Term VisitReduce(Context context) =>
            throw new InvalidOperationException();

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            throw new InvalidOperationException();

        public void Deconstruct(out Term hideTerm) =>
            hideTerm = this.Term;
    }
}
