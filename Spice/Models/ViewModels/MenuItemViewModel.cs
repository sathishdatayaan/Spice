﻿namespace Spice.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }

        public IEnumerable<Category> Category { get; set; }

        public IEnumerable<SubCategoryN> SubCategory  { get; set; }
    }
}
