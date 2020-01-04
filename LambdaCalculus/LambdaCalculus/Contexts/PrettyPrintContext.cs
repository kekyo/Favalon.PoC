namespace LambdaCalculus.Contexts
{
    public interface IRightToLeftPrettyPrintingTerm
    {
    }

    public enum HigherOrderDetails
    {
        None,
        Readable,
        Full
    }

    public sealed class PrettyPrintContext
    {
        public readonly HigherOrderDetails HigherOrderDetail;

        internal PrettyPrintContext(HigherOrderDetails higherOrderDetail)
        {
            this.HigherOrderDetail = higherOrderDetail;
        }

        internal PrettyPrintContext DropReadable() =>
            this.HigherOrderDetail switch
            {
                HigherOrderDetails.Readable => new PrettyPrintContext(HigherOrderDetails.None),
                _ => this
            };
    }
}
