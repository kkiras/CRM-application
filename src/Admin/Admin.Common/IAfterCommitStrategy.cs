namespace Admin.Common
{
    public interface IAfterCommitStrategy
    {
        string Name { get; }
        void Execute(string jsonData);
    }
}