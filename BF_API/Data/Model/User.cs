using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BF_API.Data
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
