using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace EnderPi.Framework.Pocos
{
    public class OrderData
    {
        [Required]
        [StringLength(100, ErrorMessage ="Name must be between 2 and 100 characters.", MinimumLength = 2)]
        public string FirstName { set; get; }

        [Required]
        public string LastName { set; get; }

        [Required]
        public string AddressLineOne { set; get; }
    }
}
