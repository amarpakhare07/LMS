using LMS.Domain;
using LMS.Domain.Models;
using LMS.Infrastructure.Repository;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                        builder => builder.WithOrigins("http://localhost:4200") // Replace with your client application's origin
                                            .AllowAnyHeader()
                                            .AllowAnyMethod());
            });


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
            builder.Services.Configure<FileUploadLimits>(
            builder.Configuration.GetSection("FileUploadLimits"));
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();


            builder.Services.AddScoped<PasswordHashing>();
            builder.Services.AddScoped<JwtService>();

            builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
            builder.Services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            builder.Services.AddScoped<IQuestionRepository , QuestionRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IQuizScoreRepository, QuizScoreRepository>();
            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IQuizScoreService, QuizScoreService>();

            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<ILessonService, LessonService>();  

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
            app.UseCors("AllowAngularApp");

            app.UseAuthorization();


            app.MapControllers();

            //var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    // Points to the physical folder where FileUploadService saves images
            //    FileProvider = new PhysicalFileProvider(uploadPath),

            //    // This makes the files accessible via the URL path: /images
            //    RequestPath = "/images"
            //});

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // 2. Configure a StaticFiles middleware to serve files from the Uploads folder
            //    The RequestPath is the URL prefix the client will use (e.g., "https://localhost:7049/Uploads/nigt.png")
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/Uploads"
            });

            // Optional: Enable directory browsing for the Uploads folder (useful for verification)
            // You may want to remove this in a production environment for security.
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/Uploads"
            });


            app.Run();
        }
    }
}
