namespace Vue2API.Models.Entities
{
    using System;
    using System.Collections.Generic;

    public class Profile
    {
        public string Id { get; set; }

        public string IdentityId { get; set; }
        public AppUser Identity { get; set; } // navigation property

        public string Location { get; set; }
    }
}