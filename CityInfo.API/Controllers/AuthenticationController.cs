using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthenticationController(IConfiguration config)
        {
            this._configuration = config;   
        }
        
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // Step1: Validate the username/password
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);
        
            if(user == null)
            {
                return Unauthorized();
            }

            //Step2: Create a token
            var securityKeyBytes = Convert.FromBase64String(_configuration["Authentication:SecretForKey"]!);
            var securityKey = new SymmetricSecurityKey(securityKeyBytes); //package Microsoft.IdentityModel.Tokens
            var signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub",user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name",user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            //For the Demo if there is a DB check in the DB first 

            //We assume that userName and password are valid 
            return new CityInfoUser(
                1,
                userName ?? "",
                "Ahmad",
                "El Beltagy",
                "Tanta"
            );
        }
    }
}