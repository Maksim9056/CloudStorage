
using CloudStorageClass.CloudStorageModel;
using CloudStorageWebAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CloudStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("Путь: " + path);

            string paths = Path.Combine(path, "appsettings.json");
            Console.WriteLine("Путь и файл: " + paths);

            var aCS = builder.Configuration.AddJsonFile(paths).Build().GetSection("ConnectionStrings")["CloudStorageAPI"];
            //var aCS = builder.Configuration.GetConnectionString("CloudStorageAPI");
            Console.WriteLine("Строка подключения из настроек: "+aCS);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DB>(options =>
            options.UseNpgsql(aCS ?? throw new InvalidOperationException("Connection string 'CloudStorageAPI' not found.")));
            ////options.UseNpgsql("Host=localhost;Port=5432;Database=CloudStorage;Username=postgres;Password=2" ?? throw new InvalidOperationException("Connection string 'CloudStorageAPI' not found.")));
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            //    };
            //});


            var app = builder.Build();

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
