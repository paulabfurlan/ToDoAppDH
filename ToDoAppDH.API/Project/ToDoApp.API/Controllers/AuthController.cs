using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToDoApp.API.Model.V1.DTO;
using ToDoApp.API.Repositories.V1;

namespace NZWalks.API.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        // POST: /api/auth/register
        [HttpPost]
		[MapToApiVersion("1.0")]
		[Route("Register")]
        public async Task<IActionResult> RegisterV1([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                // Add roles to this user
                if ((registerRequestDto.Roles != null) && (registerRequestDto.Roles.Any()))
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("User was successfully registered! Please login.");
                    }
                }
            }

            return BadRequest("Something went wrong");
        }

        // POST: /api/auth/login
        [HttpPost]
		[MapToApiVersion("1.0")]
		[Route("Login")]
        public async Task<IActionResult> LoginV1([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Get the roles for this User
                    var roles = await userManager.GetRolesAsync(user);

                    if (roles != null)
                    {
                        // Create Token
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList(), loginRequestDto.Expiration);

                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response);
                    }
                }
            }

            return BadRequest("Username and/or password is incorrect");
        }

		// Delete User
		[HttpDelete]
		[MapToApiVersion("1.0")]
		[Route("Delete")]
		[Authorize]
		public async Task<IActionResult> DeleteV1([FromBody] DeleteAuthRequestDto deleteRequestDto)
		{
			var user = await userManager.FindByEmailAsync(deleteRequestDto.Username);

            if (user != null)
            {
                var identityResult = await userManager.DeleteAsync(user);

                if (identityResult.Succeeded)
                {
                    return Ok(user);
                }
            }

			return BadRequest($"We couldn't delete the user {deleteRequestDto.Username}");
		}
	}
}
