namespace CleanCrudDemo.Api.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty; // Username đăng nhập
    public string Password { get; set; } = string.Empty; // Password đăng nhập
}