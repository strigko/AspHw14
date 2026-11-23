using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using p48_lessons_asp.Data;
using p48_lessons_asp.Data.Entity;
using p48_lessons_asp.Models.UserRequestModels;
using p48_lessons_asp.Services.Auth;
using p48_lessons_asp.Services.FluentValidation;

namespace p48_lessons_asp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IValidator<RegisterUserModel> _registerValidator;
    private readonly IValidator<LoginUserModel> _loginValidator;
    private readonly JwtService _jwtService;

    public UsersController(
        DataContext dataContext,
        IValidator<RegisterUserModel> registerValidator,
        IValidator<LoginUserModel> loginValidator,
        JwtService jwtService)
    {
        _dataContext = dataContext;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserModel model)
    {
        var errors = await _registerValidator.GetErrorsAsync(model);

        if (errors != null)
            return BadRequest(errors);

        var exists = await _dataContext.Users.AnyAsync(
            u => u.Username == model.Username || u.Email == model.Email
        );

        if (exists)
        {
            return Conflict(new
            {
                Message = "Username or email already exists"
            });
        }

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = PassService.HashPassword(model.Password)
        };

        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return Created("", new
        {
            Message = "User successfully registered",
            Data = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            },
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserModel model)
    {
        var errors = await _loginValidator.GetErrorsAsync(model);

        if (errors != null)
            return BadRequest(errors);

        var user = await _dataContext.Users.FirstOrDefaultAsync(
            u => u.Username == model.Username
        );

        if (user == null ||
            !PassService.VerifyPassword(
                model.Password,
                user.PasswordHash))
        {
            return Unauthorized(new
            {
                Message = "Incorrect username or password"
            });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            Message = "Login successful",
            Data = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            },
            Token = token
        });
    }
}
