
using CloudStorageClass.CloudStorageModel;
using CloudStorageWebAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO;
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
        
            builder.Services.AddDirectoryBrowser();
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartHeadersCountLimit = 1000000000;
                options.BufferBodyLengthLimit = 10000000000;
                options.MultipartBodyLengthLimit = 10000000000; // Здесь устанавливается желаемый максимальный размер в байтах (например, 10 GB)
                options.ValueLengthLimit = 999999999;
            });
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 2000000000; // Максимальный размер тела запроса в байтах (например, 1 GB)
            });
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodyBufferSize = 2000000000; // Максимальный размер заголовков запроса в байтах (например, 1 GB)
            });
            builder.Services.Configure<HttpSysOptions>(options =>
            {
                options.MaxRequestBodySize = 2000000000; // Максимальный размер заголовков запроса в байтах (например, 1 GB)
            });

      
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/upload", async context =>
                {
                    try
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // Получаем сервис провайдера из контекста запроса
                            var serviceProvider = context.RequestServices;
                            // Получаем экземпляр контекста базы данных из провайдера сервисов
                            var _context = serviceProvider.GetRequiredService<DB>();
                            context.Request.EnableBuffering(10000000000);

                            // Обработка POST-запросов по адресу /upload здесь
                            var inputModel = await context.Request.ReadFormAsync();

                            var fillesInputModel = new FillesInputModel
                            {
                                Id = int.Parse(inputModel["Id"]),
                                StoragePath = inputModel["StoragePath"],
                                TypeFiles = inputModel["TypeFiles"],
                                UserId = int.Parse(inputModel["UserId"]),
                                Fille = inputModel.Files.GetFile("Fille")
                            };

                            if (fillesInputModel == null || fillesInputModel.Fille == null || fillesInputModel.Fille.Length == 0)
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync("Ошибка: неверные данные для загрузки файла.");
                                return;
                            }

                            var filles = new Filles
                            {
                                Id = fillesInputModel.Id,
                                StoragePath = fillesInputModel.StoragePath,
                                NameFille = fillesInputModel.Fille.FileName,
                                TypeFiles = fillesInputModel.TypeFiles,
                                Size = fillesInputModel.Fille.Length,
                                UserId = fillesInputModel.UserId
                            };

                            using (var fileStream = new FileStream(paths, FileMode.Create))
                            {
                                await fillesInputModel.Fille.CopyToAsync(fileStream);
                            }

                            _context.Filles.Add(filles);
                            await _context.SaveChangesAsync();

                            context.Response.StatusCode = 200;
                            await context.Response.WriteAsJsonAsync(filles);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync($"Ошибка при загрузке файла: {ex.Message}");
                    }
                });

            });
            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDirectoryBrowser();
            app.UseAuthorization();
            app.Run();
        }
        public class FillesInputModel
        {
            public int Id { get; set; }
            public string StoragePath { get; set; }
            public string TypeFiles { get; set; }
            public IFormFile Fille { get; set; }
            public int UserId { get; set; }
        }


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

        //public class Filles
        //{
        //    /// <summary>
        //    /// Id Файла класса для Database DB Class
        //    /// </summary>
        //    public int Id { get; set; }
        //    /// <summary>
        //    /// Локальный путь ввиде сылки 
        //    /// </summary>
        //    public string StoragePath { get; set; }

        //    /// <summary>
        //    /// Имя файла
        //    /// </summary>
        //    public string NameFille { get; set; }

        //    /// <summary>
        //    /// Будет нужно для хранилища
        //    /// </summary>
        //    public byte[] Fille { get; set; }
        //    /// <summary>
        //    /// Будет нужно для хранилища
        //    /// </summary>
        //    public string TypeFiles { get; set; }
        //    /// <summary>
        //    /// Размер
        //    /// </summary>
        //    public long Size { get; set; }
        //    /// <summary>
        //    /// Id Пользователя ссылка на  пользователя который будет создан каталог 
        //    /// </summary>
        //    public int UserId { get; set; }

        //}
    }
}
