using LATESTReactKanBanDo.Auth;
using LATESTReactKanBanDo.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LATESTReactKanBanDo.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<KanbanRestUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserManager<KanbanRestUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            var user = await _userManager.FindByNameAsync(registerUserDto.UserName);
            if (user != null)
            {
                return BadRequest("User already exists.");
            }

            var newUser = new KanbanRestUser
            {
                Email = registerUserDto.Email,
                UserName = registerUserDto.UserName,
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = DateTime.UtcNow
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);

            if (!createUserResult.Succeeded)
            {
                return BadRequest("Could not create a user.");
            }

            await _userManager.AddToRoleAsync(newUser, KanbanRoles.KanbanUser);

            return CreatedAtAction(nameof(Register), new UserDto(newUser.Id, newUser.UserName, newUser.Email));
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return BadRequest("UserName or Password is not valid.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordValid)
            {
                return BadRequest("UserName or Password is not valid.");
            }

            user.ForceRelogin = false;
            await _userManager.UpdateAsync(user);

            var refreshToken = _jwtTokenService.CreateRefreshToken(user.Id);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);

            return Ok(new SuccessfullLoginDto(accessToken));
        }

        [HttpPost]
        [Route("accessToken")]
        public async Task<ActionResult> Refresh(RefreshAccessTokenDto refreshDto)
        {
            if(!_jwtTokenService.TryParseRefreshToken(refreshDto.RefreshToken, out var claims))
            {
                return BadRequest("Unprocessable.");
            }

            var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("Invalid token.");
            }

            if (user.RefreshToken != refreshDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired refresh token.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);

            var newRefreshToken = _jwtTokenService.CreateRefreshToken(user.Id);
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24);
            await _userManager.UpdateAsync(user);

            return Ok(new SuccessfullLoginDtoWithRefresh(accessToken, newRefreshToken));
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.ForceRelogin = true;
                await _userManager.UpdateAsync(user);
                return Ok("User logged out successfully.");
            }

            return BadRequest("User not found.");
        }

    }
}
