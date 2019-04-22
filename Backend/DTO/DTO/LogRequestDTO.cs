using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string source { get; set; }

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

        public string page { get; set; }

        public bool success { get; set; }

        //Used to prevent fluxation in source attributes
        public string registrationPage = "Registration";
        public string loginPage = "Login";
        public string sessionPage = "Session";
        public string logoutPage = "Logout";
        public string mapViewPage = "MapView";
        public string pointDetailsPage = "PointDetails";
        public List<string> validPage;

        public LogRequestDTO(string ssoUserId, string email, string source, string details)
        {
            this.ssoUserId = ssoUserId;
            this.email = email;
            this.source = source;
            this.details = details;
            logCreatedAt = DateTime.UtcNow;
            fillArray();
        }

        private void fillArray()
        {
            validPage = new List<string>();
            validPage.Add(registrationPage);
            validPage.Add(sessionPage);
            validPage.Add(logoutPage);
            validPage.Add(mapViewPage);
            validPage.Add(pointDetailsPage);
            validPage.Add(loginPage);
        }

        public LogRequestDTO()
        {
            logCreatedAt = DateTime.UtcNow;
            fillArray();
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
            if (!this.validPage.Contains(this.page) && this.page != null)
            {
                isValid = false;
                Console.WriteLine("Invalid Page object, please use a defined page in LogoutDTO");
            }
            return isValid;
        }
    }
}
