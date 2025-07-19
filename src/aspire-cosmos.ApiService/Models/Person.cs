using System;
using System.ComponentModel.DataAnnotations;

namespace aspire_cosmos.ApiService.Models
{
    public class Person
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Sex { get; set; } = string.Empty;
    }
}
