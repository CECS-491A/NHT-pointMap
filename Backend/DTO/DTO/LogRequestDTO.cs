using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class LogRequestDTO
    {
        [Required]
        public string ssoUserId { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public DateTime logCreatedAt { get; set; }

        [Required]
        public Constants.Constants.Sources source { get; set; }

        [Required]
        public string details { get; set; }

        [Required]
        public string timestamp { get; set; }

        [Required]
        public string signature { get; set; }

        public string token { get; set; }

        public DateTime sessionCreatedAt { get; set; }

        public DateTime sessionUpdatedAt { get; set; }

        public DateTime sessionExpiredAt { get; set; }

        public Constants.Constants.Pages page{ get; set; }

        public bool success { get; set; }        

        public LogRequestDTO(string ssoUserId, string email, string source, string details)
        {
            this.ssoUserId = ssoUserId;
            this.email = email;
            this.source = (Constants.Constants.Sources)Enum.Parse(typeof(Constants.Constants.Sources), source);
            this.details = details;
            logCreatedAt = DateTime.UtcNow;
        }

        public LogRequestDTO()
        {
            logCreatedAt = DateTime.UtcNow;
        }

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
    }
}
