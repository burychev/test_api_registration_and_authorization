using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var users = new List<User> { };


var usersApi = app.MapGroup("/users");
usersApi.MapGet("/", () => users);
usersApi.MapPost("/register", (User user) =>
{

    var existedUser = users.FirstOrDefault(u => u.email == user.email);
    if (existedUser != null) return Results.BadRequest("user is not found");
    if (Validation.ValidateEmail(user.email) == "email in correct format")
    {
        if (user.password.Length <= 8) return Results.BadRequest("password must be more than 8 characters");
        else {
            string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 5);
            User newUser = new(users.Count + 1, user.email, hashPassword);
            users.Add(newUser);
            return Results.Created();
        }
    }
    else
    {
        return Results.BadRequest("email is not in the correct format");
    }
    
    
});

usersApi.MapPost("/login", (User user) =>
{
    var existedUser = users.FirstOrDefault(u => u.email == user.email);
    if (Validation.ValidateEmail(user.email) == "email is not in the correct format") return Results.BadRequest("email is not in the correct format");
    if (existedUser == null) return Results.BadRequest("user is not found");
       if (BCrypt.Net.BCrypt.EnhancedVerify(user.password, existedUser.password) == true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("jvjoidfjviojfvo98732945ibvdkojfnvuh90fjw9fiqf0VUGVDKJnoowbdiuuwid");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Email, existedUser.email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Results.Ok(tokenString);
        }
        else
        {
            return Results.BadRequest("incorrect password");
        }
    
});


usersApi.MapDelete("/{id}", (int id) =>
{
    var existedUser = users.FirstOrDefault(u => u.id == id);
    if (existedUser == null) return Results.BadRequest("user is not found");
    users.Remove(existedUser);
    return Results.Ok("complete!");
});


app.Run();






