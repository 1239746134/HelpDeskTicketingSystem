using System.Text;
using HelpDesk.BLL.Interfaces;
using HelpDesk.BLL.Security;
using HelpDesk.BLL.Services;
using HelpDesk.BLL.Settings;
using HelpDesk.DAL.Data;
using HelpDesk.DAL.Repositories;
using HelpDesk.Web.Middlewares;
using HelpDesk.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace HelpDesk.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Serilog
            builder.Host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            //DI 注册
            var connStr = builder.Configuration.GetConnectionString("Default")
                          ?? throw new InvalidOperationException("缺少连接字符串 ConnectionStrings:Default");

            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITicketService, TicketService>();

            builder.Services.AddHttpContextAccessor();               // IHttpContextAccessor 必需
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));     // 绑定配置

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

            //JWT 认证
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
                              ?? throw new InvalidOperationException("缺少 JwtSettings 配置");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };
                });

            builder.Services.AddAuthorization();

            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HelpDesk API", Version = "v1" });

                // JWT 授权按钮
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "粘贴 token（不含 Bearer 前缀）"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins(builder.Configuration["Cors:Origins"]?.Split(',') ?? Array.Empty<string>())
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            var app = builder.Build();

            // 管道顺序
            app.UseSerilogRequestLogging();               // 请求日志

            app.UseMiddleware<ExceptionHandlingMiddleware>();   //最外层，兜异常

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");                 //CORS

            app.UseAuthentication();                       //认证（解析 token）
            app.UseAuthorization();                        //授权（[Authorize] 生效）

            app.MapControllers();

            app.Run();
        }
    }
}
