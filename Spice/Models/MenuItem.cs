using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Spice.Models
{
    public class MenuItem
    {
        public int Id  {get; set; }

        [Required]
        public string Name { get; set; }

        
        public string? Description { get; set; }

        public string Spicyness { get; set; }

        public enum Espicy { NA=0, NotSpicy=1, Spicy=2, VerySpicy=3 }

        
        public string? Image { get; set; }

        [DisplayName ("Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [DisplayName("SubCategory")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategoryN SubCategory { get; set; }

        [Range(1,int.MaxValue,ErrorMessage ="Price should be greater than $ {1}")]
        public double Price { get; set; }


    }
}
