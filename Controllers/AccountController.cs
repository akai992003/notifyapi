using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using notifyApi.Data;
using notifyApi.Model;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace notifyApi.Controllers {
    [Route ("api/[controller]")]
    [EnableCors ("CorsDomain")]
    [ApiController]
    public class AccountController : Controller {
        private IMemberService MemberService;
        private IConfiguration Configuration { get; }
        public AccountController (
            IMemberService _MemberService,
            IConfiguration _Configuration
        ) {
            this.MemberService = _MemberService;
            this.Configuration = _Configuration;
        }
        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login ([FromBody] LoginViewModel dto) {
            string msg = "";

            var q = await this.MemberService.accountLogin (dto.Email, dto.Pwd);
            if (q == null) {
                // 沒有資料列
                msg = "沒有此帳號資料";

                return BadRequest (new { Msg=msg });
            } else if (q.password == dto.Pwd) {
                // 登入成功
                msg = "登入成功";

            } else {
                // 密碼錯誤
                msg = "密碼錯誤";
                return BadRequest (new { Msg = msg });
            }
            var claims = new [] {

                new Claim (ClaimTypes.Name, q.name),
                // new Claim (ClaimTypes.Sid, user.Guid.ToString ().Replace ("-", "")),
                // new Claim (ClaimTypes.Role, userRole),
            };

            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Configuration["Tokens:IssuerSigningKey"]));
            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken (
                issuer: Configuration["Tokens:ValidIssuer"],
                audience : Configuration["Tokens:ValidAudience"],
                claims : claims,
                expires : DateTime.Now.AddMonths (1),
                signingCredentials : creds);

            var result = new JObject { { "Name", q.name }, { "Counter", q.counter },{ "Msg", msg },

            };
            return Ok (new {
                token = new JwtSecurityTokenHandler ().WriteToken (token),
                    Data = result
            });

        }
    }
}