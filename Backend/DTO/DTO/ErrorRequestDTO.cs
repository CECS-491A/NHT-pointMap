using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DTO.DTOBase;

namespace DTO
{
    /// <summary>
    /// The error log object that is set and stored to the logging server
    /// </summary>
    public class ErrorRequestDTO : BaseLogDTO
    {
        [Required]
        public string details { get; set; }

        public string exceptionCalled { get; set; }

        public string ssoUserId { get; set; }

        /// <summary>
        /// Constructor for a ErrorRequestDTO object
        /// </summary>
        /// <param name="details">A string type holding the stacktrace for the error</param>
        /// <param name="source">A Constants.Constants.Sources enumeration for the source the error originated</param>
        public ErrorRequestDTO(string details, Constants.Constants.Sources source)
        {
            this.details = details;
            this.source = source.ToString();
            logCreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Setter method converting Constants.Constants.Sources to the equivalent string value
        /// </summary>
        /// <param name="source">A Constants.Constants.Sources enumeration for the source the error originated</param>
        public void setSource(Constants.Constants.Sources source)
        {
            this.source = source.ToString();
        }

        public ErrorRequestDTO()
        {
        }

        /// <summary>
        /// The validation method for ErrorRequestDTO
        /// </summary>
        /// <returns>Returns true if all required fields have been filled properly</returns>
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
