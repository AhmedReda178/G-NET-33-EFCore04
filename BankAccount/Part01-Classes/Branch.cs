using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.Part01_Classes
{
    public class Branch
    {
        public int Code { get; set; }          // PK - Branch Code
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        // Navigation Properties
        public Manager Manager { get; set; }    // 1:1 with Manager
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
