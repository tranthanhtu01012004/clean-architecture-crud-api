using CleanCrudDemo.Api.Models; // LoginRequest
using CleanCrudDemo.Api.Services; // JwtTokenService
using Microsoft.AspNetCore.Mvc; // ControllerBase

namespace CleanCrudDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService; // Service tạo JWT

    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Demo account hard-code để test nhanh
        if (request.Username == "admin" && request.Password == "123456")
        {
            var token = _jwtTokenService.GenerateToken("admin", "Admin");

            return Ok(new
            {
                username = "admin",
                role = "Admin",
                token
            });
        }

        if (request.Username == "user" && request.Password == "123456")
        {
            var token = _jwtTokenService.GenerateToken("user", "User");

            return Ok(new
            {
                username = "user",
                role = "User",
                token
            });
        }

        return Unauthorized(new
        {
            message = "Sai tài khoản hoặc mật khẩu."
        });
    }
}