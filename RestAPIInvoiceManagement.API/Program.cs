using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestAPIInvoiceManagement.Application.Mappings;
using RestAPIInvoiceManagement.Application.Services;
using RestAPIInvoiceManagement.Application.Validators;
using RestAPIInvoiceManagement.Domain.Interfaces;
using RestAPIInvoiceManagement.Infrastructure.Data;
using RestAPIInvoiceManagement.Infrastructure.Repositories;
using RestAPIInvoiceManagement.API.Middleware;
using FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer; 
using Microsoft.Extensions.Options; 
using Swashbuckle.AspNetCore.SwaggerUI; 
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options => 
{ 
    options.DefaultApiVersion = new(1, 0); 
    options.AssumeDefaultVersionWhenUnspecified = true; 
    options.ReportApiVersions = true; 
}); 
builder.Services.AddVersionedApiExplorer(options => 
{ 
    options.GroupNameFormat = "'v'VVV"; 
    options.SubstituteApiVersionInUrl = true; 
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{ 
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Invoice Management API", Version = "v1" }); 
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    { 
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJ...'", 
        Name = "Authorization", 
        In = ParameterLocation.Header, 
        Type = SecuritySchemeType.Http, 
        Scheme = "bearer", 
        BearerFormat = "JWT" 
    }); 
    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
    { 
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                } 
            }, 
            new List<string>() 
        } 
    }); 
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddFluentValidation(fv => { 
    fv.RegisterValidatorsFromAssemblyContaining<CreateInvoiceDtoValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<LoginDtoValidator>(); 
    fv.RegisterValidatorsFromAssemblyContaining<RegisterDtoValidator>();  
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-secret-key"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c => { 
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>(); 
        foreach (var description in provider.ApiVersionDescriptions) { 
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Invoice Management API {description.GroupName}"); 
        } 
    });
}


app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();