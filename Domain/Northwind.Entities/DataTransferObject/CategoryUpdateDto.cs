using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Entities.DataTransferObject
{
    public class CategoryUpdateDto : CategoryForManipulationDto
    {
        [Required(ErrorMessage = "CategoryName is required")]
        [MaxLength(15, ErrorMessage = "Maximum length for categoryName is 15 characters")]
        public string categoryName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }
    }
}
