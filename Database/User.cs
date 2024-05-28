using BankBackend.Database.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFGetStarted.Database
{
    public class User : IdentityUser
    {
        public string? FavoriteAnimal { get; set; }
        internal List<Account>? Accounts { get; set; }
    }
}
