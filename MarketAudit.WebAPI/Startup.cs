using System;
using System.IO;
using System.Reflection;
using Marketaudit.WebAPI.Helpers;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Common.Log;
using MarketAudit.Service;
using MarketAudit.WebAPI.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Marketaudit.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            SetNLog();
            Configuration = configuration;
            SetGlobalVariables();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            CargarSingletones(services);

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Marketaudit Web API", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Marketaudit.WebAPI.xml");
                x.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "v1";
                var basePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{basePath}/swagger/{c.RoutePrefix}/swagger.json", "Marketaudit Web API");
            });

        }

        private void SetGlobalVariables()
        {
            AppConfiguration appConfig = ConfigurationHelper.GetAppConfiguration();
            GlobalVariables.SetDatabaseConnectionString(appConfig.ConnectionString);
            GlobalVariables.SetReportDatabaseConnectionString(appConfig.ReportConnectionString);

            GlobalVariables.LogsPath = appConfig.PathLogs;
            GlobalVariables.CurrentDirectory = Directory.GetCurrentDirectory();
        }

        private void CargarSingletones(IServiceCollection services)
        {
            foreach (MethodInfo m in typeof(ServiceFactory).GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                services.AddSingleton(m.ReturnType, f => m.Invoke(null, null));
            }
        }

        private void SetNLog()
        {
            var configLog = Directory.GetCurrentDirectory() + "/nlog.config";
            NLog.LogManager.LoadConfiguration(configLog);
            if (!Directory.Exists(ConfigurationHelper.GetAppConfiguration().PathLogs))
            {
                Directory.CreateDirectory(ConfigurationHelper.GetAppConfiguration().PathLogs);
            }
        }
    }
}
