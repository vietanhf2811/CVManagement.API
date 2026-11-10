using CVManagement.Application.DTOs.Candidates;
using CVManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;
        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<CandidateProfileDto>> GetMyProfile()
        {

            var userIdClaim =User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (!long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var profile = await _candidateService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Candidate profile not found."
                });
            }

            return Ok(profile);
        }
    }
}
