using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DTO.DTOBase;

namespace DTO
{
    public class ErrorRequestDTO : BaseLogDTO
    {
        [Required]
        public string details { get; set; }

        public string exceptionCalled { get; set; }

        public string ssoUserId { get; set; }

        public ErrorRequestDTO(string details, Constants.Constants.Sources source)
        {
            this.details = details;
            this.source = source.ToString();
            logCreatedAt = DateTime.UtcNow;
        }

        public void setSource(Constants.Constants.Sources source)
        {
            this.source = source.ToString();
        }

        public ErrorRequestDTO()
        {
        }

        public override bool isValid()
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
    }
}
