using Admin.Common;
using System;
using System.Collections.Generic;

namespace Admin.CustomerEdit
{
    public class EditService : IEditService
    {
        public CustomerModel? EditCustomerInteractive(IList<CustomerModel> customers)
        {
            if (customers.Count == 0)
            {
                Console.WriteLine("Không có customer nào.");
                return null;
            }

            var selectedIndex = PromptCustomerSelection(customers);
            if (selectedIndex < 0)
            {
                return null;
            }

            var customer = customers[selectedIndex];

            customer.Name = PromptTextField("Name", customer.Name);
            customer.Address = PromptTextField("Address", customer.Address);
            customer.Email = PromptEmailField("Email", customer.Email);
            customer.CreditLimit = PromptCreditLimit(customer.CreditLimit);

            return customer;
        }

        private static int PromptCustomerSelection(IList<CustomerModel> customers)
        {
            Console.WriteLine("Danh sách customer:");
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {customers[i].Name}");
            }

            while (true)
            {
                Console.Write("Chọn customer cần sửa (Nhập 0 để thoát chương trình): ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Lua chon khong hop le.");
                    continue;
                }

                if (!int.TryParse(input, out int selectedIndex))
                {
                    Console.WriteLine("Lua chon khong hop le.");
                    continue;
                }

                if (selectedIndex == 0)
                {
                    return -1;
                }

                if (selectedIndex < 1 || selectedIndex > customers.Count)
                {
                    Console.WriteLine("Lua chon khong hop le.");
                    continue;
                }

                return selectedIndex - 1;
            }
        }

        private static string PromptTextField(string label, string currentValue)
        {
            while (true)
            {
                Console.Write($"{label} [{currentValue}]: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                var trimmed = input.Trim();
                if (trimmed == "0")
                {
                    return currentValue;
                }

                return trimmed;
            }
        }

        private static string PromptEmailField(string label, string currentValue)
        {
            while (true)
            {
                Console.Write($"{label} [{currentValue}]: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                var trimmed = input.Trim();
                if (trimmed == "0")
                {
                    return currentValue;
                }

                if (!IsValidGmail(trimmed))
                {
                    Console.WriteLine("Email khong hop le. Email phai co dang @gmail.com.");
                    continue;
                }

                return trimmed;
            }
        }

        private static decimal PromptCreditLimit(decimal currentValue)
        {
            while (true)
            {
                Console.Write($"CreditLimit [{currentValue}]: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                var trimmed = input.Trim();
                if (trimmed == "0")
                {
                    return currentValue;
                }

                if (!decimal.TryParse(trimmed, out decimal parsedCreditLimit))
                {
                    Console.WriteLine("CreditLimit khong hop le.");
                    continue;
                }

                if (parsedCreditLimit < 0)
                {
                    Console.WriteLine("CreditLimit phai >= 0.");
                    continue;
                }

                return parsedCreditLimit;
            }
        }

        private static bool IsValidGmail(string email)
        {
            const string suffix = "@gmail.com";
            return email.Length > suffix.Length &&
                   email.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }
    }
}
