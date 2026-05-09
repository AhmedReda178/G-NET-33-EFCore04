using BankAccount.Part01_Classes;
using Microsoft.EntityFrameworkCore;

namespace BankAccount
{
   internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new BankDbContext();
            context.Database.Migrate(); // Auto-update database

            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    National Bank - Management");
                Console.WriteLine("========================================");
                Console.WriteLine("  1) Add a new Customer");
                Console.WriteLine("  2) Open a new Account for a Customer");
                Console.WriteLine("  3) Update Account Status (Active / Closed)");
                Console.WriteLine("  4) Remove an Account from a Customer");
                Console.WriteLine("  5) List all Customers (with accounts)");
                Console.WriteLine("  0) Exit");
                Console.WriteLine("----------------------------------------");
                Console.Write("  Enter choice: ");

                try
                {
                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": AddCustomer(context); break;
                        case "2": OpenAccount(context); break;
                        case "3": UpdateStatus(context); break;
                        case "4": RemoveAccount(context); break;
                        case "5": ListCustomers(context); break;
                        case "0": return;
                        default: ShowError("Invalid choice!"); break;
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Error: {ex.Message}");
                }
            }
        }

        static void AddCustomer(BankDbContext context)
        {
            Console.WriteLine("\n--- Add New Customer ---");

            Console.Write("Full Name: ");
            var name = Console.ReadLine();

            Console.Write("National ID: ");
            var nationalId = Console.ReadLine();

            Console.Write("Date of Birth (yyyy-MM-dd): ");
            var dob = DateTime.Parse(Console.ReadLine());

            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("Phone: ");
            var phone = Console.ReadLine();

            Console.Write("Address: ");
            var address = Console.ReadLine();

            Console.WriteLine("Customer Type: 1) Individual  2) Business");
            Console.Write("Choice: ");
            var type = int.Parse(Console.ReadLine()) == 1 ? CustomerType.Individual : CustomerType.Business;

            var customer = new Customer
            {
                FullName = name,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = type
            };

            context.Customers.Add(customer);
            context.SaveChanges();

            Console.WriteLine($"\n✓ Customer created successfully! ID: {customer.Id}");
            Pause();
        }

        static void OpenAccount(BankDbContext context)
        {
            Console.WriteLine("\n--- Open New Account ---");

            Console.Write("Account Number: ");
            var accNum = Console.ReadLine();

            Console.WriteLine("Account Type: 1) Savings  2) Current  3) Business");
            Console.Write("Choice: ");
            var accType = (AccountType)(int.Parse(Console.ReadLine()) - 1);

            Console.Write("Branch Code: ");
            var branchCode = int.Parse(Console.ReadLine());

            // Verify branch exists
            var branch = context.Branches.Find(branchCode);
            if (branch == null) throw new Exception("Branch not found!");

            Console.Write("Customer ID: ");
            var customerId = int.Parse(Console.ReadLine());

            // Verify customer exists
            var customer = context.Customers.Find(customerId);
            if (customer == null) throw new Exception("Customer not found!");

            Console.WriteLine("Ownership Role: 1) Primary  2) CoHolder");
            Console.Write("Choice: ");
            var ownership = int.Parse(Console.ReadLine()) == 1 ? OwnershipType.Primary : OwnershipType.CoHolder;

            var account = new Account
            {
                AccountNumber = accNum,
                AccountType = accType,
                OpeningDate = DateTime.Now,
                CurrentBalance = 0,
                BranchCode = branchCode
            };

            var customerAccount = new CustomerAccount
            {
                CustomerId = customerId,
                AccountNumber = accNum,
                OwnershipStartDate = DateTime.Now,
                OwnershipType = ownership,
                AccountStatus = AccountStatus.Active
            };

            context.Accounts.Add(account);
            context.CustomerAccounts.Add(customerAccount);
            context.SaveChanges();

            Console.WriteLine($"\n✓ Account '{accNum}' created as {ownership} owner");
            Pause();
        }

        static void UpdateStatus(BankDbContext context)
        {
            Console.WriteLine("\n--- Update Account Status ---");

            Console.Write("Account Number: ");
            var accNum = Console.ReadLine();

            Console.Write("Customer ID: ");
            var custId = int.Parse(Console.ReadLine());

            var link = context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accNum && ca.CustomerId == custId);

            if (link == null) throw new Exception("Account-Customer link not found!");

            Console.WriteLine("New Status: 1) Active  2) Closed");
            Console.Write("Choice: ");
            link.AccountStatus = int.Parse(Console.ReadLine()) == 1 ? AccountStatus.Active : AccountStatus.Closed;

            context.SaveChanges();
            Console.WriteLine($"\n✓ Status updated to {link.AccountStatus}");
            Pause();
        }

        static void RemoveAccount(BankDbContext context)
        {
            Console.WriteLine("\n--- Remove Account From Customer ---");

            Console.Write("Account Number: ");
            var accNum = Console.ReadLine();

            Console.Write("Customer ID: ");
            var custId = int.Parse(Console.ReadLine());

            var link = context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accNum && ca.CustomerId == custId);

            if (link == null) throw new Exception("Link not found!");

            context.CustomerAccounts.Remove(link);
            context.SaveChanges();

            // Check if this was the last owner
            var remainingOwners = context.CustomerAccounts.Count(ca => ca.AccountNumber == accNum);
            if (remainingOwners == 0)
            {
                var account = context.Accounts.Find(accNum);
                context.Accounts.Remove(account);
                context.SaveChanges();
                Console.WriteLine($"\n✓ Ownership link deleted. That was the last owner - account '{accNum}' was also removed.");
            }
            else
            {
                Console.WriteLine("\n✓ Ownership link deleted.");
            }
            Pause();
        }

        static void ListCustomers(BankDbContext context)
        {
            Console.WriteLine("\n--- All Customers ---");

            var customers = context.Customers
                .Include(c => c.CustomerAccounts)
                .ThenInclude(ca => ca.Account)
                .ThenInclude(a => a.Branch)
                .ToList();

            for (int i = 0; i < customers.Count; i++)
            {
                var c = customers[i];
                Console.Write($"#{i + 1} {c.FullName} ({c.CustomerType}) ");

                if (c.CustomerAccounts.Any())
                {
                    foreach (var ca in c.CustomerAccounts)
                    {
                        Console.Write($"\n   {ca.Account.AccountNumber}-{ca.Account.AccountType} " +
                            $"Balance: {ca.Account.CurrentBalance:C} " +
                            $"{ca.OwnershipType} {ca.AccountStatus} @ {ca.Account.Branch.Name}");
                    }
                }
                else
                {
                    Console.Write("(no accounts)");
                }
                Console.WriteLine();
            }
            Pause();
        }

        static void ShowError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n⚠ {msg}");
            Console.ResetColor();
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}
