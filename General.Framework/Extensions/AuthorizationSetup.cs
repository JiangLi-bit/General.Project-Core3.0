using General.Common;
using General.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace General.Framework
{
    /// <summary>
    /// JWT 授权认证
    /// </summary>
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            string Issuer = Appsettings.app(new string[] { "JwtSettings", "Issuer" });
            string Audience = Appsettings.app(new string[] { "JwtSettings", "Audience" });
            string Secret = Appsettings.app(new string[] { "JwtSettings", "Secret" });
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
            var permissionModel = new List<PermissionModel>();

            // 角色与接口的权限要求参数
            var permission = new PermissionRequirement(
                "",// 拒绝授权的跳转地址（暂时无用）
                permissionModel,
                ClaimTypes.Role,//基于角色的授权（这里是权限表中的类型编号）
                Issuer,//发行人
                Audience,//听众
                signingCredentials,//签名凭据
                expiration: TimeSpan.FromSeconds(60 * 60)//接口的过期时间
                );

            services.AddAuthentication(o =>{
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>{

                o.TokenValidationParameters = new TokenValidationParameters(){
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateIssuer = true,
                    ValidIssuer = Issuer,

                    ValidateAudience = true,
                    ValidAudience = Audience,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                };
            });

            services.AddAuthorization(o =>
            {

                /*策略授权三大模块
                * 1.基于角色*/
                o.AddPolicy("AdminPolicy", o =>
                {
                    o.RequireRole("Admin").Build();
                });

                // 2.基于声明
                o.AddPolicy("AdminClaim", o =>
                {
                    o.RequireClaim("Claim", "Admin", "User").Build();
                });

                //3.基于需求
                o.AddPolicy(Permission.Name, o =>
                {
                    o.Requirements.Add(permission);
                });
            });

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(permission);
        }
    }
}
