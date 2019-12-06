using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wedding_Planner.Models {

    public class Wedding {

        [Key]
        public int WeddingId { get; set; }

        [Required]
        public string WedderOne { get; set; }

        [Required]
        public string WedderTwo { get; set; }

        [Required]
        [FutureDate]
        public DateTime date { get; set; }

        [Required]
        public string Address { get; set; }

        public List<Association> Assoc_Wedding { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute {
        protected override ValidationResult IsValid (object value, ValidationContext validationContext) {
            // You first may want to unbox "value" here and cast to to a DateTime variable!
            if ((DateTime) value < DateTime.Now) {
                return new ValidationResult ("Invalid: Future Date");
            }
            return ValidationResult.Success;
        }
    }
}