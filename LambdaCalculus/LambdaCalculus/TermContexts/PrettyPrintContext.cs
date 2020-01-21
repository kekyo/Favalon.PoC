namespace Favalon.TermContexts
{
    public interface IRightToLeftPrettyPrintingTerm
    {
    }

    public enum HigherOrderDetails
    {
        None,
        Specified,
        Full
    }

    public sealed class PrettyPrintContext
    {
        public readonly HigherOrderDetails HigherOrderDetail;

        internal PrettyPrintContext(HigherOrderDetails higherOrderDetail) =>
            this.HigherOrderDetail = higherOrderDetail;

        internal PrettyPrintContext DropSpecified() =>
            this.HigherOrderDetail switch
            {
                HigherOrderDetails.Specified => new PrettyPrintContext(HigherOrderDetails.None),
                _ => this
            };
    }
}
