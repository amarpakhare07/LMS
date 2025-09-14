using LMS.Domain;
using LMS.Infrastructure.Repository;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
            //-----------------
            //Add Database Context DI
            builder.Services.AddDbContext<LmsDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("LMSDbConnection"))
                );
            //---------------

            //Add all the Dependancies Injection from Services and Repositories Layer

            builder.Services.AddScoped<PasswordHashing>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IRegisterUserRepository, RegisterUserRepository>();

            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();

            //------------------

            builder.Services.AddEndpointsApiExplorer();
            //------------------

            // JWT Authentication Configuration
            var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        ValidateLifetime = true,
                    };
                });
            //--------------------

            // Swagger Configuration
            builder.Services.AddSwaggerGen(
                    c =>
                    {
                        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                            Scheme = "Bearer"
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
                                    },
                                    Scheme = "bearer",
                                    Name = "Authorization",
                                    In = ParameterLocation.Header
                                },
                                new string[]{}
                            }
                        });
                    });
            //--------------------
            // Authorization Configuration
            builder.Services.AddAuthorization();


            //--------------------

            // Build the app.
            var app = builder.Build();
            //--------------------


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
