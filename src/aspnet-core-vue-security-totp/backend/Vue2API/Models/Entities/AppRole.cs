namespace Vue2API.Models.Entities
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class AppRole : IdentityRole//<Guid>
    {
        public string Description { get; set; }
    }
}