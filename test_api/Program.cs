using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using BCrypt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();


string validation(string emailtext)
{
    Regex regex_v = new Regex(@"[^@\s]+@[^@\s]+\.[^@\s]+$");
    bool isvalid_v = regex_v.IsMatch(emailtext);
    if (isvalid_v)
    {
        return "email §á§â§Ñ§Ó§Ú§Ý§î§ß§à§Ô§à §æ§à§â§Þ§Ñ§ä§Ñ";
    }
    else
    {
        return "email §ß§Ö§á§â§Ñ§Ó§Ú§Ý§î§ß§à§Ô§à §æ§à§â§Þ§Ñ§ä§Ñ";
    }
    
}


var users = new List<User> { };


var usersApi = app.MapGroup("/users");
usersApi.MapGet("/", () => users);
usersApi.MapPost("/register", (User user) =>
{

    var existedUser = users.FirstOrDefault(u => u.email == user.email);
    if (existedUser != null) return Results.BadRequest("§±§à§Ý§î§Ù§à§Ó§Ñ§ä§Ö§Ý§î §ß§Ö §ß§Ñ§Û§Õ§Ö§ß");
    if (validation(user.email) == "email §á§â§Ñ§Ó§Ú§Ý§î§ß§à§Ô§à §æ§à§â§Þ§Ñ§ä§Ñ")
    {
        if (user.password.Length <= 8) return Results.BadRequest("§±§Ñ§â§à§Ý§î §Õ§à§Ý§Ø§Ö§ß §Ò§í§ä§î §Ò§à§Ý§î§ê§Ö 8 §ã§Ú§Þ§Ó§à§Ý§à§Ó");
        else {
            string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 5);
            User newUser = new(users.Count + 1, user.email, hashPassword);
            users.Add(newUser);
            return Results.Created();
        }
    }
    else
    {
        return Results.BadRequest("email §ß§Ö§á§â§Ñ§Ó§Ú§Ý§î§ß§à§Ô§à §æ§à§â§Þ§Ñ§ä§Ñ(");
    }
    
    //Console.WriteLine(HashPassword);
    //GetHash(newUser.password);
    //Console.WriteLine((newUser.password));
    
});

usersApi.MapPost("/login", (User user) =>
{
    var existedUser = users.FirstOrDefault(u => u.email == user.email);
    if (validation(user.email) == "email §ß§Ö§á§â§Ñ§Ó§Ú§Ý§î§ß§à§Ô§à §æ§à§â§Þ§Ñ§ä§Ñ") return Results.BadRequest("email §ß§Ö§á§â§Ñ§Ó§Ú§Ý§î§ß§à§Ô§à §æ§à§â§Þ§Ñ§ä§Ñ(");
    if (existedUser == null) return Results.BadRequest("§¯§Ö§ä §ä§Ñ§Ü§à§Ô§à §á§à§Ý§î§Ù§à§Ó§Ñ§ä§Ö§Ý§ñ");
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
            return Results.BadRequest("§¯§Ö§Ó§Ö§â§ß§í§Û §á§Ñ§â§à§Ý§î");
        }
    
});


usersApi.MapDelete("/{id}", (int id) =>
{
    var existedUser = users.FirstOrDefault(u => u.id == id);
    if (existedUser == null) return Results.BadRequest("§¯§Ö§ä §ä§Ñ§Ü§à§Ô§à §á§à§Ý§î§Ù§à§Ó§Ñ§ä§Ö§Ý§ñ");
    users.Remove(existedUser);
    return Results.Ok("§¤§à§ä§à§Ó§à");
});


//usersApi.MapDelete("/delete", (User user) =>
//{
//    int userToDelete = 5;
//    var existedUser = users.FirstOrDefault(u => u.id == user.id);
//    if (existedUser == null) return Results.BadRequest("§¯§Ö§ä §ä§Ñ§Ü§à§Ô§à §á§à§Ý§î§Ù§à§Ó§Ñ§ä§Ö§Ý§ñ");
//    users.Remove(userToDelete);


//    return Results.Created();
//});


//usersApi.MapDelete("/delete", (User user) =>
//{

//    var existedUser = users.FirstOrDefault(u => u.email == user.email);
//    if (existedUser != null) return Results.BadRequest("§ß§Ö§ä §ä§Ñ§Ü§à§Ô§à §á§à§Ý§î§Ù§à§Ó§Ñ§ä§Ö§Ý§ñ");
//    User newUser = new(1, user.email, user.password);

//    users.Add(newUser);
//    return Results.Created();
//});

// usersApi.MapDelete("/{id}", (User user) =>
// {
//    var existedUser = users.FirstOrDefault(u => u.email == user.email);
//    if (existedUser == null) return Results.BadRequest("§´§í §ß§Ñ§ã §ß§Ñ§Ö§Ò§Ñ§Ý");

// });



app.Run();


public record User(int id, string email, string password);


[JsonSerializable(typeof(List<User>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
