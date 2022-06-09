using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Entities.DataTransferObject
{
    public class CustomerUpdateDto : CustomerForManipulationDto
    {
        [Required(ErrorMessage = "CustomerName is required")]
        [MaxLength(40, ErrorMessage = "Maximum length for customerId is 40 characters")]
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
    }
}
