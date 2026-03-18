using Admin.Common;

namespace Admin.Extensibility
{
    public class AfterCommitContext
    {
        private IAfterCommitStrategy _strategy;

        public void SetStrategy(IAfterCommitStrategy strategy)
        {
            _strategy = strategy;
        }

        public void Execute(string jsonData)
        {
            _strategy?.Execute(jsonData);
        }
    }
}