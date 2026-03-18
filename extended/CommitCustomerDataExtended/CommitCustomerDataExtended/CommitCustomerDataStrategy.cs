using Admin.Common;
using System;

namespace CommitCustomerDataExtended
{
    public class CommitCustomerDataStrategy : IAfterCommitStrategy
    {
        public string Name => "CommitCustomerData";

        public void Execute(string jsonData)
        {
            Console.WriteLine("Running CommitCustomerData strategy...");

            var service = new CommitCustomerData();
            service.After(jsonData);
        }
    }
}