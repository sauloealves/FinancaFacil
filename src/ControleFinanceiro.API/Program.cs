using ControleFinanceiro.API.Middlewares;
using ControleFinanceiro.Application;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Budgets;
using ControleFinanceiro.Application.UseCases.Invoice;
using ControleFinanceiro.Infrastructure;
using ControleFinanceiro.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Resend;

using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters
        .Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddHttpClient<OpenAiClient>();
builder.Services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
builder.Services.AddScoped<IAiParserService, AiParserService>();
builder.Services.AddScoped<ICsvTextExtractor, CsvTextExtractor>();
builder.Services.AddScoped<CreateBudgetUseCase>();
builder.Services.AddScoped<GetBudgetsUseCase>();
builder.Services.AddScoped<GetBudgetByIdUseCase>();
builder.Services.AddScoped<GetBudgetSummaryUseCase>();
builder.Services.AddScoped<GetBudgetMonthsUseCase>();
builder.Services.AddScoped<UpdateBudgetItemsUseCase>();
builder.Services.AddScoped<GetTransactionsByMonthAndCategoryUseCase>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.UseInlineDefinitionsForEnums();
});

// Infrastructure (EF Core, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();


// CORS (temporário para desenvolvimento)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});



var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddHttpClient<IResend, ResendClient>();
builder.Services.AddSingleton(new ResendClientOptions {
    ApiToken = builder.Configuration["Resend:ApiKey"]
});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseForwardedHeaders();


if(!app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();
