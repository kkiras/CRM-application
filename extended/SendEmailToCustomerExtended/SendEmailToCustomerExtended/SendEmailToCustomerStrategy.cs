using Admin.Common;
using System;

namespace SendEmailToCustomerExtended
{
    public class SendEmailToCustomerStrategy : IAfterCommitStrategy
    {
        public string Name => "SendEmailToCustomer";

        public void Execute(string jsonData)
        {
            Console.WriteLine("Running SendEmailToCustomer strategy...");

            var service = new SendEmailToCustomer();
            service.After(jsonData);
        }
    }
}