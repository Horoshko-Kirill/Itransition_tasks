using Microsoft.AspNetCore.Identity;
using System;


namespace MyApp.Models
{
    public class User : IdentityUser
    {

        public string Name { get; set; }

        public DateTime LastVisit {  get; set; }

        public bool IsBlocked { get; set; }
    }
}
