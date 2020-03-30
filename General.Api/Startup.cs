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

            //���ݿ�����
            string strsql = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<GeneralDbContext>(options => options.UseSqlServer(strsql));


            //��ӿ�������
            services.AddCorsSetup();
            //Appsettings  ע��
            services.AddSingleton(new Appsettings(Env.ContentRootPath));
            //���swagger
            services.AddSwaggerSetup();
            //�����֤����
            services.AddAuthorizationSetup();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region Autofac  dll�ļ�ע��
            //��־aop
            builder.RegisterType<GeneralLogAop>();

            var repository = Assembly.LoadFrom(Path.Combine(basePath, "General.Repository.dll"));
            var iRepository = Assembly.LoadFrom(Path.Combine(basePath, "General.IRepository.dll"));
            var service = Assembly.LoadFile(Path.Combine(basePath, "General.Service.dll"));
            var iService = Assembly.LoadFile(Path.Combine(basePath, "General.IService.dll"));

            //��������Լ�����ִ��Ľӿں�ʵ�־���Repository��β��
            builder.RegisterAssemblyTypes(repository).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(iRepository).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(iService).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()  //����Autofac.Extras.DynamicProxy
                .InterceptedBy(typeof(GeneralLogAop));     
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region ����Swagger
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var ApiName = Appsettings.app(new string[] { "Startup", "ApiName" });
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{ApiName} v1");
                /*
                 * ·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�,ע��localhost:8001/swagger�Ƿ��ʲ����ģ�
                 * ȥlaunchSettings.json��launchUrlȥ����������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "doc";
                 */
                c.RoutePrefix = "";
            });
            #endregion

            app.UseCors("CorsRequest");

            // ʹ�þ�̬�ļ�
            app.UseStaticFiles();

            app.UseRouting();

            // �ȿ�����֤
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
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
