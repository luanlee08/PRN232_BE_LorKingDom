using BLL;
using BLL.DTOs;
using BLL.Validators.SuperCategory;
using DAL;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register Controllers + FluentValidation
            builder.Services.AddControllers().AddFluentValidation();

            // Custom response trả về lỗi khi validation fail
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var error = context.ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();

                    return new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = error ?? "Error",
                        Data = null
                    });
                };
            });

            // Connection string
            var conn = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

            // Đăng ký DAL + BLL
            builder.Services.AddDAL(conn);
            builder.Services.AddBLL();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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
