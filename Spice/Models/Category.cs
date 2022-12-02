using MessagePack;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Spice.Models
{
    public class Category
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        [Display(Name="CategoryName")]
        [System.ComponentModel.DataAnnotations.Required]      
        public string Name { get; set; }


       
    }
}
