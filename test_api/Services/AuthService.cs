public class AuthService
{
    private readonly List<User> _users = new List<User>();

    private readonly PasswordHashService _passwordHashService;
    private readonly TokenService _TokenService;

    public AuthService(PasswordHashService passwordHashService, TokenService TokenService)
    {
        _passwordHashService = passwordHashService;
        _TokenService = TokenService;

    }

    public (bool IsSuccess, string Message) Register(User user)
    {
        if (_users.Any(u => u.email == user.email))
        {
            return (false, "Пользователь уже существует");
        }

        if (Validation.ValidateEmail(user.email) != "email in correct format")
        {
            return (false, "Email имеет некорректный формат");
        }

        if (user.password.Length <= 8)
        {
            return (false, "Пароль должен быть длиннее 8 символов");
        }


        string hashedPassword = _passwordHashService.HashPassword(user.password);
        User newUser = new User(_users.Count + 1, user.email, hashedPassword);
        _users.Add(newUser);

        return (true, "Пользователь успешно зарегистрирован");
    }

    public (bool IsSuccess, string Message) Login(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.email == user.email);

        if (Validation.ValidateEmail(user.email) != "email in correct format")
        {
            return (false, "Email имеет некорректный формат");
        }

        if (existingUser == null)
        {
            return (false, "Пользователь не найден");
        }

        if (!_passwordHashService.VerifyPassword(user.password, existingUser.password))
        {
            return (false, "Неверный пароль");
        }

            string tokenString = _TokenService.GenerateToken(existingUser.email);
            return (true, tokenString);
        }



        //string tokenString = "sdf";
        //string tokenString = _TokenService.GenerateToken(existingUser.email);

        //return (true, tokenString);
    

    public (bool IsSuccess, string Message) DeleteUser(int id)
    {
        var existingUser = _users.FirstOrDefault(u => u.id == id);
        if (existingUser == null)
        {
            return (false, "Пользователь не найден");
        }

        _users.Remove(existingUser);
        return (true, "Пользователь успешно удален");
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }
}
