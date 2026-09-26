using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class MiscelleanousController : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetPhotoLocations")]
        public IActionResult GetPhotoLocations()
        {
            return Ok(new List<string>
            {
                "Living Room",
                "Bedroom",
                "Bathroom",
                "Laundry",
                "Exterior",
                "Additional"
            });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetRuleTypes")]
        public IActionResult GetRuleTypes()
        {
            return Ok(new List<object>
            {
                new { Key = "PARTIES", Value = "Parties" },
                new { Key = "PETS", Value = "Pets" },
                new { Key = "QUIET_HOURS_START", Value = "Quiet Hour Start" },
                new { Key = "QUIET_HOURS_END", Value = "Quiet Hour End" },
                new { Key = "SMOKING", Value = "Smoking" },
                new { Key = "NUMBER_OF_GUESTS", Value = "Number Of Guests" },
                new { Key = "CHECK_IN", Value = "Check In" },
                new { Key = "CHECK_IN_END", Value = "Check In End"},
                new { Key = "COMMERCIAL_PHOTOGRAPHY", Value = "Commercial Photography"},
                new { Key = "CHECK_OUT", Value = "Check Out" },
                new { Key = "ADDITIONAL", Value = "Additional Rules" }
            });
        }
    }
}
