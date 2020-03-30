using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Autofac;
using General.Common;
using General.Framework;
using Autofac.Extras.DynamicProxy;

namespace General.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }
        //public string ApiName { get; set; } = "General.Api";

        private string basePath = PlatformServices.Default.Application.ApplicationBasePath;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options=>options.EnableEndpointRouting=false);

            //数据库连接
            string strsql = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<GeneralDbContext>(options => options.UseSqlServer(strsql));


            //添加跨域配置
            services.AddCorsSetup();
            //Appsettings  注入
            services.AddSingleton(new Appsettings(Env.ContentRootPath));
            //添加swagger
            services.AddSwaggerSetup();
            //添加认证配置
            services.AddAuthorizationSetup();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region Autofac  dll文件注入
            //日志aop
            builder.RegisterType<GeneralLogAop>();

            var repository = Assembly.LoadFrom(Path.Combine(basePath, "General.Repository.dll"));
            var iRepository = Assembly.LoadFrom(Path.Combine(basePath, "General.IRepository.dll"));
            var service = Assembly.LoadFile(Path.Combine(basePath, "General.Service.dll"));
            var iService = Assembly.LoadFile(Path.Combine(basePath, "General.IService.dll"));

            //根据名称约定（仓储的接口和实现均以Repository结尾）
            builder.RegisterAssemblyTypes(repository).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(iRepository).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(iService).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()  //引用Autofac.Extras.DynamicProxy
                .InterceptedBy(typeof(GeneralLogAop));     
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region 启用Swagger
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var ApiName = Appsettings.app(new string[] { "Startup", "ApiName" });
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{ApiName} v1");
                /*
                 * 路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，
                 * 去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
                 */
                c.RoutePrefix = "";
            });
            #endregion

            app.UseCors("CorsRequest");

            // 使用静态文件
            app.UseStaticFiles();

            app.UseRouting();

            // 先开启认证
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
