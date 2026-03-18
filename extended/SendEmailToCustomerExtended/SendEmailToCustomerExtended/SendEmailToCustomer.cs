using System;
using System.Text.Json;
using SendEmailToCustomerExtended.Interfaces;
using SendEmailToCustomerExtended.Services;

namespace SendEmailToCustomerExtended
{
    public class SendEmailToCustomer
    {
        private readonly IEmailSender _emailSender;

        public SendEmailToCustomer()
        {
            _emailSender = new MailKitEmailSender();
        }

        public void After(string parameter)
        {
            try
            {
                using var doc = JsonDocument.Parse(parameter);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var customer in root.EnumerateArray())
                    {
                        SendToCustomer(customer);
                    }
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    SendToCustomer(root);
                }
                else
                {
                    Console.WriteLine("Email plugin error: Expected JSON object or array.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email plugin error: {ex.Message}");
            }
        }

        private void SendToCustomer(JsonElement customer)
        {
            string? name = customer.TryGetProperty("Name", out var nameProp)
                ? nameProp.GetString()
                : null;

            string? email = customer.TryGetProperty("Email", out var emailProp)
                ? emailProp.GetString()
                : null;

            if (string.IsNullOrWhiteSpace(email))
                return;

            string subject = "Your customer record has been updated";
            string body = $"Hello {name}, your customer record has been changed.";

            _emailSender.SendAsync(email, subject, body).GetAwaiter().GetResult();
        }
    }
}