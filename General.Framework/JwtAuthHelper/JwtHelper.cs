using General.Common;
using General.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace General.Framework
{
    public class JwtHelper
    {
        private static string Issuer = string.Empty;
        private static string Audience = string.Empty;
        private static string Secret = string.Empty;

        /// <summary>
        /// 生成JWT token 字符串
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GenerateToken(TokenModel model)
        {
            //获取JWT所需的参数
            GetJwtParameter();

            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Jti, model.Uid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                //这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(1000).ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,Issuer),
                new Claim(JwtRegisteredClaimNames.Aud,Audience),
               };

            //写入权限编号
            claims.AddRange(model.Role.Split(',').Select(role => new Claim(ClaimTypes.Role, role)));

            var securityToken = new JwtSecurityToken(
                issuer: Issuer,
                //audience: Audience,       //因上面代码已经添加了一个听众  new Claim(JwtRegisteredClaimNames.Aud,Audience),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret)), SecurityAlgorithms.HmacSha256),

                expires: DateTime.Now.AddHours(1),
                claims: claims
                );

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModel SerializeToken(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModel
            {
                Uid = (jwtToken.Id).ToStr(),
                Role = role != null ? role.ToStr() : "",
            };
            return tm;
        }

        #region  测试方法
        /// <summary>
        /// 获取token, 测试方法
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            //获取JWT所需的参数
            GetJwtParameter();

            var securityToken = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret)), SecurityAlgorithms.HmacSha256),

                expires: DateTime.Now.AddHours(1),
                claims:new Claim[] { 
                    
                });

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }
        #endregion

        /// <summary>
        /// 获取Jwt所需的参数
        /// </summary>
        private static void GetJwtParameter()
        {
            Issuer = Appsettings.app(new string[] { "JwtSettings", "Issuer" });
            Audience = Appsettings.app(new string[] { "JwtSettings", "Audience" });
            Secret = Appsettings.app(new string[] { "JwtSettings", "Secret" });
        }

    }
}
