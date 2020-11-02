namespace Favalet.Parsers
{
    public interface IParseRunnerFactory
    {
        ParseRunner Waiting { get; }
        ParseRunner Applying { get; }
    }
}