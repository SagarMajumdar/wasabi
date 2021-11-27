

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wasabi.Models{
    public class User {
        public int Id {get; set;}
        //[Required]
        public string? FirstName { get; set;}
        //[Required]
        public string? LastName { get; set;}
        [Required]
        [EmailAddress]
        public string? Email { get; set;}
        [MaxLength(300)]
        public string? About {get; set;}

        [Required]
        public string Password {get; set;}
        

        public DateTime JoinDate {get; set;}

    }
}
