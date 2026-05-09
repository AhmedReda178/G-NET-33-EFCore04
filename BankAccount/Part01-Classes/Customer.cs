using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.Part01_Classes
{
    public enum CustomerType { Individual, Business }

    public class Customer
    {
        public int Id { get; set; }            // PK
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public CustomerType CustomerType { get; set; }

        // Navigation
        public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    }
}
