using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class MiscelleanousController : ControllerBase
    {
        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetLeaseContractEditableSections")]
        public IActionResult GetLeaseContractEditableSections()
        {
            return Ok(new List<object>
            {
                new { SectionNumber = 6, Title = "Utilities", IsEditable = true },
                new { SectionNumber = 7, Title = "Pets", IsEditable = true },
                new { SectionNumber = 8, Title = "Parking", IsEditable = true },
                new { SectionNumber = 9, Title = "Use and Occupancy", IsEditable = true },
                new { SectionNumber = 10, Title = "Maintenance", IsEditable = true },
                new { SectionNumber = 11, Title = "Entry and Notice", IsEditable = true },
                new { SectionNumber = 12, Title = "Damage, Liability, and Repairs", IsEditable = true },
                new { SectionNumber = 13, Title = "Utilities Allocation Schedule", IsEditable = true },
                new { SectionNumber = 14, Title = "Insurance", IsEditable = true },
                new { SectionNumber = 15, Title = "Default and Remedies", IsEditable = true },
                new { SectionNumber = 16, Title = "Early Termination", IsEditable = true },
                new { SectionNumber = 17, Title = "Renewal and Holdover", IsEditable = true },
                new { SectionNumber = 18, Title = "Compliance with Laws and Community Rules", IsEditable = true },
                new { SectionNumber = 19, Title = "Move-In and Move-Out Inspection", IsEditable = true },
                new { SectionNumber = 20, Title = "Dispute Resolution and Governing Venue", IsEditable = true },
                new { SectionNumber = 21, Title = "Notices", IsEditable = true },
                new { SectionNumber = 22, Title = "Privacy and Data Handling", IsEditable = true },
                new { SectionNumber = 23, Title = "Entire Agreement and Severability", IsEditable = true },
                new { SectionNumber = 24, Title = "Joint and Several Liability", IsEditable = true },
                new { SectionNumber = 25, Title = "Guest Policy", IsEditable = true },
                new { SectionNumber = 26, Title = "Unauthorized Occupants and Subletting", IsEditable = true },
                new { SectionNumber = 27, Title = "Returned Payment, NSF, and Chargebacks", IsEditable = true },
                new { SectionNumber = 28, Title = "No Waiver", IsEditable = true },
                new { SectionNumber = 29, Title = "No Oral Modifications", IsEditable = true },
                new { SectionNumber = 30, Title = "Surrender of Premises at Move-Out", IsEditable = true },
                new { SectionNumber = 35, Title = "Signatures", IsEditable = true }
            });
        }
    }
}
