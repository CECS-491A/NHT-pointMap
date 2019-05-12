using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DTO.DTOBase;

namespace DTO
{
    /// <summary>
    /// Data Transfer object for log analytics, derived object of BaseLogDTO
    /// </summary>
    public class LogRequestDTO : BaseLogDTO
    {
        [Required]
        public string ssoUserId { get; set; }

        public string details { get; set; }

        public string token { get; set; }

        public DateTime sessionCreatedAt { get; set; }

        public DateTime sessionUpdatedAt { get; set; }

        public DateTime sessionExpiredAt { get; set; }

        public string page{ get; private set; }

        /// <summary>
        /// Constructor for the LogRequestDTO class
        /// </summary>
        /// <param name="ssoUserId">A string of a GUID of a User object</param>
        /// <param name="source">A Constants.Constants.Sources enumeration for the source the error originated</param>
        public LogRequestDTO(Constants.Constants.Sources source, string userId)
        {
            this.ssoUserId = ssoUserId;
            this.source = source.ToString();
            logCreatedAt = DateTime.UtcNow;
        }

        public LogRequestDTO(Constants.Constants.Sources source, string userId, DateTime createdAt, 
            DateTime updatedAt, DateTime expiredAt, string token)
        {
            this.ssoUserId = userId;
            this.source = source.ToString();
            this.token = token;
            this.sessionCreatedAt = createdAt;
            this.sessionExpiredAt = expiredAt;
            this.sessionUpdatedAt = updatedAt;
        }

        /// <summary>
        /// Base constructor for the LogRequestDTO object
        /// </summary>
        public LogRequestDTO()
        {
            logCreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Setter method converting Constants.Constants.Pages to the equivalent string value
        /// </summary>
        /// <param name="source">A Constants.Constants.Pages enumeration for the page the log originated</param>
        public void setPage(Constants.Constants.Pages page)
        {
            this.page = page.ToString();
        }

        /// <summary>
        /// Setter method converting Constants.Constants.Sources to the equivalent string value
        /// </summary>
        /// <param name="source">A Constants.Constants.Sources enumeration for the source the log originated</param>
        public void setSource(Constants.Constants.Sources source)
        {
            this.source = source.ToString();
        }

        /// <summary>
        /// The validation method for LogRequestDTO
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
