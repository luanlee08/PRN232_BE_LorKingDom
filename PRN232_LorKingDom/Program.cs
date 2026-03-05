using BLL;
using BLL.DTOs;
using BLL.Interfaces;
using BLL.Worker;
using CloudinaryDotNet;
using DAL;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232_LorKingDom.Hubs;
using PRN232_LorKingDom.Middleware;
using PRN232_LorKingDom.Services;
using System.Text;

namespace PRN232_LorKingDom
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Required for SignalR WebSocket
                });
            });

            // Register Controllers + FluentValidation
            builder.Services.AddControllers().AddFluentValidation();

            // Custom response trả về lỗi khi validation fail
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Validation failed",
                        Data = errors
                    });
                };
            });

            // Connection string
            var conn = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                                 "http://localhost:3000"
                              )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();

                });
            });
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            // Cloudinary - Dùng cho ReviewProductService và các chức năng khác
            // ProfileService đã chuyển sang lưu local
            builder.Services.AddSingleton<Cloudinary>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var cloudName = config["Cloudinary:CloudName"];
                var apiKey = config["Cloudinary:ApiKey"];
                var apiSecret = config["Cloudinary:ApiSecret"];

                if (string.IsNullOrEmpty(cloudName) ||
                    string.IsNullOrEmpty(apiKey) ||
                    string.IsNullOrEmpty(apiSecret))
                {
                    throw new InvalidOperationException("Cloudinary config missing");
                }

                var account = new Account(cloudName, apiKey, apiSecret);
                var cloudinary = new Cloudinary(account);
                cloudinary.Api.Secure = true;

                return cloudinary;
            });

            // Đăng ký DAL + BLL
            builder.Services.AddDAL(conn);
            builder.Services.AddBLL();

            // SignalR — real-time shipping status push
            builder.Services.AddSignalR();
            // IShippingRealtimeService: BLL interface implemented by SignalR in Web layer
            builder.Services.AddScoped<IShippingRealtimeService, SignalRShippingRealtimeService>();

            // Hangfire Configuration
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(conn, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            builder.Services.AddHangfireServer();

            // JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Allow SignalR to receive JWT from query string (?access_token=...)
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            // Swagger with JWT
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "LorKingdom API",
                    Version = "v1",
                    Description = "API cho hệ thống LorKingdom"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            // Configure recurring jobs
            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                // Legacy notification worker (deprecated)
                recurringJobManager.AddOrUpdate<NotificationWorker>(
                    "process-scheduled-notifications",
                    worker => worker.ProcessScheduledNotificationsJob(),
                    Cron.Minutely);

                // GHN Shipping Status Sync Worker
                recurringJobManager.AddOrUpdate<ShippingStatusSyncWorker>(
                    "sync-ghn-shipping-status",
                    worker => worker.SyncGHNShippingStatusJob(),
                    Cron.MinuteInterval(5)); // Run every 5 minutes

                // Demo mode: auto-advance shipping status (enabled via DemoMode:AutoFlowEnabled)
                recurringJobManager.AddOrUpdate<DemoShippingFlowWorker>(
                    "demo-shipping-flow",
                    worker => worker.AdvanceDemoShippingFlowJob(),
                    Cron.MinuteInterval(2)); // Advance every 2 minutes

                // Auto-cancel Pending orders where external payment expired
                recurringJobManager.AddOrUpdate<ExpiredPaymentOrderWorker>(
                    "cancel-expired-payment-orders",
                    worker => worker.CancelExpiredPaymentOrdersJob(),
                    Cron.MinuteInterval(5)); // Check every 5 minutes
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowFrontend");
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // SignalR hub endpoint — clients connect to /hubs/shipping
            app.MapHub<ShippingHub>("/hubs/shipping");

            app.Run();
        }
    }
}
