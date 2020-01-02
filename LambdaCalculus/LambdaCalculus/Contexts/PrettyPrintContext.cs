namespace LambdaCalculus.Contexts
{
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
    }
}
