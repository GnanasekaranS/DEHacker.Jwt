using DEHacker.Businesslogic;
using DEHacker.Jwt.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DEHacker.Jwt.Helpers
{

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications


        private readonly AppSettings _appSettings;
        private readonly IBusinessLayer _businessLayer;
        public UserService(IOptions<AppSettings> appSettings, IBusinessLayer businessLayer)
        {
            _appSettings = appSettings.Value;
            _businessLayer = businessLayer;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {

            var user = _businessLayer.IsValidUser(model.Username, model.Password);

            // return null if user not found
            if (user == false) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(model.Username);

            return new AuthenticateResponse(token);
        }



        // helper methods

        private string generateJwtToken(string userId)
        {

           // var secret = _secretManager.GetSecret("JWTSecret1");
           // var finalSecret = string.IsNullOrEmpty(secret.Result) ? _appSettings.Secret : secret.Result;

            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId) }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
