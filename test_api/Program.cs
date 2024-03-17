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
        return "email ���ѧӧڧݧ�ߧ�ԧ� ����ާѧ��";
    }
    else
    {
        return "email �ߧ֧��ѧӧڧݧ�ߧ�ԧ� ����ާѧ��";
    }
    
}


var users = new List<User> { };


var usersApi = app.MapGroup("/users");
usersApi.MapGet("/", () => users);
usersApi.MapPost("/register", (User user) =>
{

    var existedUser = users.FirstOrDefault(u => u.email == user.email);
    if (existedUser != null) return Results.BadRequest("����ݧ�٧�ӧѧ�֧ݧ� �ߧ� �ߧѧۧէ֧�");
    if (validation(user.email) == "email ���ѧӧڧݧ�ߧ�ԧ� ����ާѧ��")
    {
        if (user.password.Length <= 8) return Results.BadRequest("���ѧ��ݧ� �է�ݧا֧� �ҧ��� �ҧ�ݧ��� 8 ��ڧާӧ�ݧ��");
        else {
            string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 5);
            User newUser = new(users.Count + 1, user.email, hashPassword);
            users.Add(newUser);
            return Results.Created();
        }
    }
    else
    {
        return Results.BadRequest("email �ߧ֧��ѧӧڧݧ�ߧ�ԧ� ����ާѧ��(");
    }
    
    //Console.WriteLine(HashPassword);
    //GetHash(newUser.password);
    //Console.WriteLine((newUser.password));
    
});

usersApi.MapPost("/login", (User user) =>
{
    var existedUser = users.FirstOrDefault(u => u.email == user.email);
    if (validation(user.email) == "email �ߧ֧��ѧӧڧݧ�ߧ�ԧ� ����ާѧ��") return Results.BadRequest("email �ߧ֧��ѧӧڧݧ�ߧ�ԧ� ����ާѧ��(");
    if (existedUser == null) return Results.BadRequest("���֧� ��ѧܧ�ԧ� ���ݧ�٧�ӧѧ�֧ݧ�");
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
            return Results.BadRequest("���֧ӧ֧�ߧ�� ��ѧ��ݧ�");
        }
    
});


usersApi.MapDelete("/{id}", (int id) =>
{
    var existedUser = users.FirstOrDefault(u => u.id == id);
    if (existedUser == null) return Results.BadRequest("���֧� ��ѧܧ�ԧ� ���ݧ�٧�ӧѧ�֧ݧ�");
    users.Remove(existedUser);
    return Results.Ok("������ӧ�");
});


//usersApi.MapDelete("/delete", (User user) =>
//{
//    int userToDelete = 5;
//    var existedUser = users.FirstOrDefault(u => u.id == user.id);
//    if (existedUser == null) return Results.BadRequest("���֧� ��ѧܧ�ԧ� ���ݧ�٧�ӧѧ�֧ݧ�");
//    users.Remove(userToDelete);


//    return Results.Created();
//});


//usersApi.MapDelete("/delete", (User user) =>
//{

//    var existedUser = users.FirstOrDefault(u => u.email == user.email);
//    if (existedUser != null) return Results.BadRequest("�ߧ֧� ��ѧܧ�ԧ� ���ݧ�٧�ӧѧ�֧ݧ�");
//    User newUser = new(1, user.email, user.password);

//    users.Add(newUser);
//    return Results.Created();
//});

// usersApi.MapDelete("/{id}", (User user) =>
// {
//    var existedUser = users.FirstOrDefault(u => u.email == user.email);
//    if (existedUser == null) return Results.BadRequest("���� �ߧѧ� �ߧѧ֧ҧѧ�");

// });



app.Run();


public record User(int id, string email, string password);


[JsonSerializable(typeof(List<User>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
