var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    });

builder.Services.AddSingleton<AuthService>();

builder.Services.AddSingleton<PasswordHashService>();

builder.Services.AddSingleton<TokenService>();

var app = builder.Build();

app.MapControllers();

app.Run();
