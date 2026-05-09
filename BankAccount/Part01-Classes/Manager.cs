using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.Part01_Classes
{
    public class Manager
    {
        public int Id { get; set; }            // PK
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }

        // Navigation
        public int BranchCode { get; set; }     // FK
        public Branch Branch { get; set; }
    }
}
