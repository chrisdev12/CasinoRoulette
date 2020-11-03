using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roulette.Core.Context;
using Roulette.Core.Entities.DTO.User;
using Roulette.Core.Entities.User;
using Roulette.Core.Helpers;
using Roulette.Core.Services.Security;

namespace Roulette.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly CognitoService _CognitoService;
        private readonly DynamoContext _DBDynamo;
        private readonly IMapper _Mapper;
        public UserController(CognitoService CognitoService, DynamoContext DBDynamo, IMapper Mapper)
        {
            _CognitoService = CognitoService;
            _DBDynamo = DBDynamo;
            _Mapper = Mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CognitoSignUp([FromBody] User userInfo)
        {
            await _CognitoService.SignUp(userInfo);
            UserDynamo newUser = _Mapper.Map<UserDynamo>(userInfo);
            await _DBDynamo.Insert(newUser);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> CognitoLogin([FromBody] User userInfo)
        {
            var token = await _CognitoService.LogIn(userInfo);

            return Ok(token);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("edit")]
        public async Task<IActionResult> CognitoEditCash([FromBody] User userInfo)
        {

            UserDynamo UserUpdate = new UserDynamo
            {
                Cash = userInfo.Cash,
                Email = ClaimsHelper.GetValue(HttpContext)
            };
            await _DBDynamo.Update(UserUpdate);

            return Ok(UserUpdate);
        }
    }
}
