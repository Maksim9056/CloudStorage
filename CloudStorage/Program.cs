
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

            builder.Services.AddDirectoryBrowser();
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartHeadersCountLimit = 1000000000;
                options.BufferBodyLengthLimit = 10000000000;
                options.MultipartBodyLengthLimit = 10000000000; // ����� ��������������� �������� ������������ ������ � ������ (��������, 10 GB)
                options.ValueLengthLimit = 999999999;
            });
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 2000000000; // ������������ ������ ���� ������� � ������ (��������, 1 GB)
            });
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodyBufferSize = 2000000000; // ������������ ������ ���������� ������� � ������ (��������, 1 GB)
            });
            builder.Services.Configure<HttpSysOptions>(options =>
            {
                options.MaxRequestBodySize = 2000000000; // ������������ ������ ���������� ������� � ������ (��������, 1 GB)
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
                        using (MemoryStream mc = new MemoryStream())
                        {


                            // �������� ������ ���������� �� ��������� �������
                            var serviceProvider = context.RequestServices;
                            // �������� ��������� ��������� ���� ������ �� ���������� ��������
                            var _context = serviceProvider.GetRequiredService<DB>();
                            context.Request.EnableBuffering(10000000000);

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

                            if (fillesInputModel == null || fillesInputModel.Fille == null || fillesInputModel.Fille.Length == 0)
                            {
                                context.Response.StatusCode = 500; // ��������� ��� ������� ������ (����� �������� �� ����� ����������)
                            }
                            var filles = new Filles
                            {
                                Id = fillesInputModel.Id,
                                StoragePath = fillesInputModel.StoragePath,
                                NameFille = fillesInputModel.Fille.FileName,
                                TypeFiles = fillesInputModel.TypeFiles,
                                Size = fillesInputModel.Fille.Length,
                                UserId = fillesInputModel.UserId
                                // �� ��������� Fille, ��� ��� �� ��� ��� ��������� �� �������
                            };
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                fillesInputModel.Fille.CopyTo(memoryStream);
                                filles.Fille = memoryStream.ToArray();
                                // �������� ���������� IFormFile � �����

                            }
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            string fileExtension = Path.GetExtension(filles.NameFille).Trim('.');
                            filles.TypeFiles = fileExtension;
                            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == filles.UserId);

                            List<string> list = new List<string>();
                            DateTime dateTime = DateTime.Now;

                            string DATE = "";
                            var date = dateTime.ToString();
                            var dates = date.Replace('.', '_');

                            var DA = dates.Replace(':', '_');
                            for (int i = 0; i < DA.Length; i++)
                            {
                                if (DA[i].ToString() == "13")
                                {
                                    DATE += "_";
                                }
                                else
                                {
                                    list.Add(DA[i].ToString());

                                }
                            }
                            DATE = "";
                            list.RemoveAt(10);
                            list.Insert(10, "_");
                            for (int i = 0; i < list.Count(); i++)
                            {
                                if (list[i] == "")
                                {

                                }
                                else
                                {
                                    DATE += list[i].ToString();
                                }
                            }

                            if (user == null)
                            {
                                await context.Response.WriteAsync($"��������� ������: user  �� ������");

                            }
                            var paths = Path.Combine(path, user.Name);
                            //!Directory.Exists(path + $"\\{user.Name}"
                            if (!Directory.Exists(paths))
                            {

                                Directory.CreateDirectory(paths);
                                paths = Path.Combine(paths, Guid.NewGuid().ToString() + "_" + filles.NameFille);
                                using (MemoryStream memoryStream = new MemoryStream(filles.Fille))
                                {
                                    //$"\\{user.Name}\\{Guid.NewGuid().ToString() + "_DATE_" + DATE + filles.NameFille}"
                                    using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                                    {
                                        await memoryStream.CopyToAsync(fileStream);
                                        filles.StoragePath = fileStream.Name;
                                        filles.Size = fileStream.Length;
                                    }
                                }

                                Console.WriteLine($"����� ������� �������! {paths}");
                            }
                            else
                            {
                                paths = Path.Combine(paths, Guid.NewGuid().ToString() + "_" + filles.NameFille);
                                using (MemoryStream memoryStream = new MemoryStream(filles.Fille))
                                {
                                    //$"\\{user.Name}\\{Guid.NewGuid().ToString() + "_DATE_" + DATE + filles.NameFille}
                                    using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                                    {
                                        await memoryStream.CopyToAsync(fileStream);
                                        filles.StoragePath = fileStream.Name;

                                        filles.Size = fileStream.Length;
                                    }
                                }
                                Console.WriteLine($"����� � ��������� ����� ��� ����������: {paths}");
                            }

                            _context.Filles.Add(filles);
                            await _context.SaveChangesAsync();
                            //// ������� ������ ������ Filles


                            // ���������� ��������� ������ Filles � �����
                            context.Response.StatusCode = 200;
                            await context.Response.WriteAsJsonAsync(filles);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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
