using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("users")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _authService.GetAllUsers();
        return Ok(users);
    }

    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        var result = _authService.Register(user);
        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Message);
        }
        return BadRequest(result.Message);
    }

    [HttpPost("login")]
    public IActionResult Login(User user)
    {
        var result = _authService.Login(user);
        if (result.IsSuccess)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var result = _authService.DeleteUser(id);
        if (result.IsSuccess)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
}
