using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerApp.Handlers
{
    public  class JwtHelper
    {
        
        

        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public JwtHelper(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool ValidateToken(AccessToken accessToken,out ClaimsPrincipal claimsPrincipal, out AuthenticationProperties authProperties)
        {


            RSA rsa = RSA.Create();
          /*  rsa.ImportRSAPublicKey(
                source: Convert.FromBase64String(_config["JwtSettings:Asymmetric:PublicKey"]),
                bytesRead: out int _
            );
          */
          
            rsa.FromXmlString(_config["JwtSettings:Asymmetric:PublicKey"]);
            

            var keyBytes = Encoding.UTF8.GetBytes(_config["JwtSettings:SecretEncryptionKey"]);
            var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);

            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new RsaSecurityKey(rsa),
                TokenDecryptionKey = symmetricSecurityKey,
                ValidateIssuer = true,
                ValidIssuer = _config["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero

            };


            var handler = new JwtSecurityTokenHandler();
            claimsPrincipal = null;
            authProperties = null;

            try
            {
                claimsPrincipal = handler.ValidateToken(
                accessToken.JwtToken,
                tokenValidationParameters,
                out SecurityToken securityToken);
            }
            catch (Exception)
            {
                return false;
            }

            List<Claim> claimsToAdd = new List<Claim>();
            claimsToAdd.AddRange(claimsPrincipal.Claims);
            claimsToAdd.Add(new Claim("Access_token", accessToken.JwtToken));


            authProperties = new AuthenticationProperties
            {
                ExpiresUtc = accessToken.Expire
            };
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claimsToAdd, CookieAuthenticationDefaults.AuthenticationScheme));

            return true;
        }
    }
}
