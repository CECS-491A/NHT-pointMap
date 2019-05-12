using DTO.Constants;
using DTO.LoggingAPI;
using System;
using System.Net;
using System.Web.Http;
using static ServiceLayer.Services.ExceptionService;
using DTO;
using Logging.Logging;

namespace WebApi_PointMap.Controllers
{
    /// <summary>
    /// Logging Enpoints:
    ///     - Admin Dash Analytics
    ///         - The top 5 most used application features
    ///         - The top 5 most time spent on app webpage
    /// </summary>
    public class LoggingController : ApiController
    {
        LogRequestDTO newLog;
        Logger logger;

        // Token is passed in header of request
        [Route("api/log/webpageusage")]
        [HttpPost]
        public IHttpActionResult LogWebpageUsage(LogWebpageUsageRequest payload)
        {
            try
            {
                // Throws SessionNotFound
                //var session = ControllerHelpers.ValidateSession(Request);
                var isValidPage = Enum.IsDefined(typeof(Constants.Pages), payload.Page);
                if (!isValidPage)
                {
                    return Content(HttpStatusCode.PreconditionFailed, "Webpage is not valid.");
                }

                var session = ControllerHelpers.ValidateAndUpdateSession(Request);
                newLog = new LogRequestDTO(Constants.Sources.Analytics, session.UserId.ToString(), session.CreatedAt, session.UpdatedAt,
                    session.ExpiresAt, session.Token);
                Constants.Pages page;
                Enum.TryParse(payload.Page, out page);
                newLog.setPage(page);
                newLog.pageDuration = payload.GetDuration();
                logger.sendLogAsync(newLog);

                return Ok("Webpage (" + payload.Page + ") usage has been logged.");
            }
            catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
            {
                return Content(HttpStatusCode.Unauthorized, e.Message);
            }
            catch (Exception e) when (e is ArgumentException)
            {
                return Content(HttpStatusCode.PreconditionFailed, e.Message);
            }
        }

        // Token is passed in header of request
        [Route("api/log/appfeatureusage")]
        [HttpPost]
        public IHttpActionResult LogAppFeatureUsage()
        {

            try
            {
                // Throws SessionNotFound
                var session = ControllerHelpers.ValidateSession(Request);

                return Ok();
            }
            catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
            {
                return Content(HttpStatusCode.Unauthorized, e.Message);
            }
        }
    }
}
