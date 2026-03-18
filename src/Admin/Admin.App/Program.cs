using Admin.Common;
using Admin.CustomerRead;
using Admin.CustomerUpdate;
using Admin.CustomerEdit;
using Admin.CustomerDelete;
using Admin.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace Admin.App
{
    class Program
    {
        static List<CustomerModel> _customers = new List<CustomerModel>();
        static Random _rnd = new Random();
        static Hook _hook = new Hook();
        private static readonly StrategyResolver _strategyResolver = new StrategyResolver();
        private static readonly AfterCommitContext _afterCommitContext = new AfterCommitContext();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1 - Read Customer Data");
                Console.WriteLine("2 - Write Customer Data");
                Console.WriteLine("3 - Add Customer");
                Console.WriteLine("4 - Edit Customer");
                Console.WriteLine("5 - Delete Customer");
                
                Console.WriteLine("0 - Exit");

                var choice = Console.ReadKey();
                Console.WriteLine();

                switch (choice.Key)
                {
                    case ConsoleKey.D0:
                        return;

                    case ConsoleKey.D1:
                        ReadCustomerData();
                        foreach (var customer in _customers)
                        {
                            Console.WriteLine($"Customer: {customer.Name}");
                        }
                        break;

                    case ConsoleKey.D2:
                        CommitCustomerData();
                        break;

                    case ConsoleKey.D3:
                        ReadCustomerData();
                        _customers.Add(new CustomerModel()
                        {
                            Name = $"Customer {Guid.NewGuid()}",
                            Address = "Customer Address",
                            CreditLimit = _rnd.Next(1000),
                            Email = $"customer{_rnd.Next(10000)}@domain.com"
                        });
                        CommitCustomerData();
                        break;

                    case ConsoleKey.D4:
                        EditCustomer();
                        break;

                    case ConsoleKey.D5:
                        DeleteCustomer();
                        break;
                }

                Console.WriteLine();
            }
        }

        private static void ReadCustomerData()
        {
            var read = new ReadService();
            var file = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..",
                "testData", "text.json");
            _customers = read.ReadAll(file).ToList();
        }

        private static void CommitCustomerData()
        {
            CommitCustomerData(null);
        }

        private static void CommitCustomerData(CustomerModel changedCustomer)
        {
            var write = new WriteService();
            var file = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..",
                "testData", "text.json");

            write.Write(_customers, file);

            if (changedCustomer != null)
            {
                string jsonParams = JsonSerializer.Serialize(changedCustomer);

                var strategy = _strategyResolver.Resolve("SendEmailToCustomer");
                if (strategy != null)
                {
                    _afterCommitContext.SetStrategy(strategy);
                    _afterCommitContext.Execute(jsonParams);
                }

                return;
            }

            string allCustomersJson = JsonSerializer.Serialize(_customers);

            var commitStrategy = _strategyResolver.Resolve("CommitCustomerData");
            if (commitStrategy != null)
            {
                _afterCommitContext.SetStrategy(commitStrategy);
                _afterCommitContext.Execute(allCustomersJson);
            }
        }

        private static void EditCustomer()
        {
            ReadCustomerData();
            var editService = new EditService();
            var changedCustomer = editService.EditCustomerInteractive(_customers);
            if (changedCustomer == null)
            {
                return;
            }

            CommitCustomerData(changedCustomer);

            Console.WriteLine("Cập nhật customer thành công.");
        }

        private static void DeleteCustomer()
        {
            ReadCustomerData();
            var deleteService = new DeleteService();
            var deleted = deleteService.DeleteCustomerInteractive(_customers);
            if (!deleted)
            {
                return;
            }

            CommitCustomerData();

            Console.WriteLine("Xóa customer thành công.");
        }
    }
}
