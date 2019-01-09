namespace Vue2API.Models.Entities
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser//<Guid>
    {
        [PersonalData]
        public string Name { get; set; }
    }
}