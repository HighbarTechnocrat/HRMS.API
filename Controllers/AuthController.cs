using AutoMapper;
using HRMS.API.DTOs;
using HRMS.API.Models.Response;
using HRMS.API.Models;
using HRMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HRMS.API.Services;
using Microsoft.Extensions.Logging;

namespace HRMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        public AuthController(IUserService userService, ILogger<AuthController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email and password are required"
                });
            }

            var result = await _userService.ValidateUserLoginAsync(request.Email, request.Password);
           

            if (!result.Success)
            { 
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Unauthorized(result);
            }

             

            // Here you can generate JWT token if needed
            // var token = GenerateJwtToken(result.Data);

            var userResponse = _mapper.Map<UserResponse>(result.Data);
            return Ok(new
            {
                Success = true,
                Message = "Login successful",
                User = userResponse
                // Token = token
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // No try-catch needed

            if (string.IsNullOrEmpty(request.EmpCode) ||
                string.IsNullOrEmpty(request.OldPassword) ||
                string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "All fields are required"
                });
            }

            var result = await _userService.ChangePasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("validate-email/{email}")]
        public async Task<IActionResult> ValidateEmail(string email)
        {
            // No try-catch needed

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email is required"
                });
            }

            var result = await _userService.GetUserByEmailAsync(email);
            return Ok(result);
        }
    }
}