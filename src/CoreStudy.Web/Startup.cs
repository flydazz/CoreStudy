using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CoreStudy.Core.IdentityServer;
using CoreStudy.Core.Redis.Extensions;
using CoreStudy.Data;
using CoreStudy.Services.Categories;
using CoreStudy.Services.Posts;
using CoreStudy.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CoreStudy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllersWithViews();

            var sqlConnectionString = Configuration.GetConnectionString("Connection");

            //�������������
            services.AddDbContext<MyContext>(options =>
                options.UseSqlServer(sqlConnectionString)
            );

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Blog API",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            //AddSingleton������ʵ��������������ģ�����ͬ�١�����Ӧ�ó����н���һ��ʵ����
            //AddTransient�������������ģ��������������ϼӰాҹ�����úܿ졣��������£��������ʵ�����õ�ʱ�򴴽��������ֱ�����١�
            //AddScoped������Ƚ�����⡣�������������ڵ��������ڣ������ͻ����������֮���������������󣬷���ֻҪ����ĻỰ�����ˣ��ͻ�����
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton<HttpClient>(i =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return new HttpClient(handler);
            });

            services.AddRedis();

            var identityServerUrl = IdentityServerUrlsConfigurationString.GetIdentityServerUrlsConfigurationString(Configuration);

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    //options.Authority = "http://localhost:5000";
                    options.Authority = identityServerUrl;

                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestResponseLogging();

            app.UseSwagger();
            //�����м�������swagger-ui��ָ��Swagger JSON�ս��
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Blog API");
                c.RoutePrefix = "";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            IdentityModelEventSource.ShowPII = true;

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

        }
    }
}
