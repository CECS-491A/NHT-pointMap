using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DTO.DTOBase
{
    /// <summary>
    /// Base class for all LogDTO objects
    /// </summary>
    public abstract class BaseLogDTO
    {
        [Required]
        public DateTime logCreatedAt { get; set; }

        [Required]
        public string source { get; protected set; }

        [Required]
        public string timestamp { get; set; }

        [Required]
        public string signature { get; set; }

        [Required]
        public string salt { get; set; }

        /// <summary>
        /// The validation method for LogRequestDTO
        /// </summary>
        /// <returns>Returns true if all required fields have been filled properly</returns>
        public bool isValid()
        {
            ValidationContext context = new ValidationContext(this, serviceProvider: null, items: null); //Creates validation context
            List<ValidationResult> results = new List<ValidationResult>(); //Initalizes validated results array

            //Attempts to validate object placing results per required field in results array
            bool isValid = Validator.TryValidateObject(this, context, results, true);

            if (isValid == false) //If object isn't valid print out error messages of required fields
            {

                StringBuilder sbrErrors = new StringBuilder();
                foreach (var validationResult in results)
                {
                    sbrErrors.AppendLine(validationResult.ErrorMessage);
                }
                Console.WriteLine(sbrErrors.ToString());

            }
            return isValid;
        }

        public string getValidationString()
        {
            string returnString = "";
            ValidationContext context = new ValidationContext(this, serviceProvider: null, items: null); //Creates validation context
            List<ValidationResult> results = new List<ValidationResult>(); //Initalizes validated results array

            //Attempts to validate object placing results per required field in results array
            bool isValid = Validator.TryValidateObject(this, context, results, true);

            if (isValid == false) //If object isn't valid print out error messages of required fields
            {

                StringBuilder sbrErrors = new StringBuilder();
                foreach (var validationResult in results)
                {
                    sbrErrors.AppendLine(validationResult.ErrorMessage);
                }
                returnString += sbrErrors.ToString() + "\n";

            }
            if (isValid)
                returnString += "Object is validated";
            return returnString;
        }
    }
}
