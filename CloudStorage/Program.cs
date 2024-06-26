
using CloudStorageClass.CloudStorageModel;
using CloudStorageWebAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text;
using Prometheus;

namespace CloudStorage
{
    public class Program
    {
        static readonly Prometheus.Counter s_hatsSold1 = Metrics.CreateCounter("hats_sold1",
            "The number of hats sold in our store");
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("����: " + path);

            string paths = Path.Combine(path, "appsettings.json");
            Console.WriteLine("���� � ����: " + paths);

            var aCS = builder.Configuration.AddJsonFile(paths).Build().GetSection("ConnectionStrings")["CloudStorageAPI"];
            //var aCS = builder.Configuration.GetConnectionString("CloudStorageAPI");
            Console.WriteLine("������ ����������� �� ��������: "+aCS);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DB>(options =>
            options.UseNpgsql(aCS ?? throw new InvalidOperationException("Connection string 'CloudStorageAPI' not found.")));
        
            builder.Services.AddDirectoryBrowser();
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartHeadersCountLimit = 999999999;
                options.BufferBodyLengthLimit = 99999999999999;
                options.MultipartBodyLengthLimit = 99999999999999; // ����� ��������������� �������� ������������ ������ � ������ (��������, 10 GB)
                options.ValueLengthLimit = 999999999;
            });
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 99999999999999; // ������������ ������ ���� ������� � ������ (��������, 1 GB)
            });
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodyBufferSize = 999999999; // ������������ ������ ���������� ������� � ������ (��������, 1 GB)
            });
            builder.Services.Configure<HttpSysOptions>(options =>
            {
                options.MaxRequestBodySize = 99999999999999; // ������������ ������ ���������� ������� � ������ (��������, 1 GB)
            });

            builder.Services.AddHttpClient();

            var app = builder.Build();
          
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
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
                            // �������� ������ ���������� �� ��������� �������
                            var serviceProvider = context.RequestServices;
                            // �������� ��������� ��������� ���� ������ �� ���������� ��������
                            var _context =  serviceProvider.GetRequiredService<DB>();
                            //context.Request.EnableBuffering(10000000000);

                            // ��������� POST-�������� �� ������ /upload �����
                            var inputModel = await context.Request.ReadFormAsync();

                            var fillesInputModel = new FillesInputModel
                            {
                                Id = int.Parse(inputModel["Id"]),
                                StoragePath = inputModel["StoragePath"],
                                TypeFiles = inputModel["TypeFiles"],
                                UserId = int.Parse(inputModel["UserId"]),
                                Fille = inputModel.Files.GetFile("Fille")
                            };
                            fillesInputModel.Fille.OpenReadStream().CopyTo(memoryStream);
                            if (fillesInputModel == null || fillesInputModel.Fille == null || fillesInputModel.Fille.Length == 0)
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync("������: �������� ������ ��� �������� �����.");
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

                            filles.Fille = memoryStream.ToArray();
                                // �������� ���������� IFormFile � �����


                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            string fileExtension = Path.GetExtension(filles.NameFille).Trim('.');
                            filles.TypeFiles = fileExtension;
                            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == filles.UserId);
                            Console.WriteLine("��� ������������ ����� :" + filles.NameFille);
                            Console.WriteLine("�����  ������������ ����� :" + filles.Fille.Length);
                            Console.WriteLine("StoragePath  ������������ ����� :" + filles.StoragePath);

                            Console.WriteLine("���� 1 :"+path);
                            var paths = Path.Combine(path, user.Name);
                            //!Directory.Exists(path + $"\\{user.Name}"
                            if (!Directory.Exists(paths))
                            {

                                Directory.CreateDirectory(paths);
                                paths = Path.Combine(paths, Guid.NewGuid().ToString() + filles.NameFille);
                                using (MemoryStream memoryStream1 = new MemoryStream(filles.Fille))
                                {
                                    Console.WriteLine("���� 2 :" + paths);

                                    //$"\\{user.Name}\\{Guid.NewGuid().ToString() + "_DATE_" + DATE + filles.NameFille}"
                                    using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                                    {
                                        await memoryStream1.CopyToAsync(fileStream);
                                        filles.StoragePath = fileStream.Name;
                                        filles.Size = fileStream.Length;
                                    }
                                }


                                Console.WriteLine($"����� ������� �������! {paths}");
                            }
                            else
                            {
                                paths = Path.Combine(paths, Guid.NewGuid().ToString()  + filles.NameFille);
                                using (MemoryStream memoryStream2 = new MemoryStream(filles.Fille))
                                {
                                    Console.WriteLine("���� 2 :" + paths);

                                    //$"\\{user.Name}\\{Guid.NewGuid().ToString() + "_DATE_" + DATE + filles.NameFille}
                                    using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                                    {
                                        await memoryStream2.CopyToAsync(fileStream);
                                        filles.StoragePath = fileStream.Name;

                                        filles.Size = fileStream.Length;
                                    }
                                }
                                Console.WriteLine($"����� � ��������� ����� ��� ����������: {paths}");
                            }
                            _context.Filles.Add(filles);
                            await _context.SaveChangesAsync();                     
                            await context.Response.WriteAsJsonAsync(filles);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"������ ��� �������� �����: {ex.Message}");
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync($"������ ��� �������� �����: {ex.Message}");
                    }
                });

            });



            // Setup Prometheus metrics endpoint
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/metrics"))
                {
                    await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body);
                }
                else
                {
                    await next();
                }
            });

            var rand = new Random();
            var timer = new Timer(_ =>
            {
                s_hatsSold1.Inc(rand.Next(0, 1000)); // Increment the hats sold counter
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Example: add hats sold every 10 seconds

            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDirectoryBrowser();
            app.UseAuthorization();

            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Start the timer

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
        //    /// Id ����� ������ ��� Database DB Class
        //    /// </summary>
        //    public int Id { get; set; }
        //    /// <summary>
        //    /// ��������� ���� ����� ����� 
        //    /// </summary>
        //    public string StoragePath { get; set; }

        //    /// <summary>
        //    /// ��� �����
        //    /// </summary>
        //    public string NameFille { get; set; }

        //    /// <summary>
        //    /// ����� ����� ��� ���������
        //    /// </summary>
        //    public byte[] Fille { get; set; }
        //    /// <summary>
        //    /// ����� ����� ��� ���������
        //    /// </summary>
        //    public string TypeFiles { get; set; }
        //    /// <summary>
        //    /// ������
        //    /// </summary>
        //    public long Size { get; set; }
        //    /// <summary>
        //    /// Id ������������ ������ ��  ������������ ������� ����� ������ ������� 
        //    /// </summary>
        //    public int UserId { get; set; }

        //}
    }
}
