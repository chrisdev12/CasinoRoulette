using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Roulette.Core.Entities.Roulettes;
using Roulette.Core.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Roulette.Core.Services.Queue;
using Roulette.Core.Entities.Roulette;
using Roulette.Core.Services.Security;
using Roulette.Core.Entities.DTO.User;
using Roulette.Core.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Roulette.API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RouletteController : ControllerBase
    {
        private readonly DynamoContext _DBDynamo;
        private readonly BetHandler _Queue;
        private readonly string sqsBet;
        private readonly string sqsWinner;
        public RouletteController(DynamoContext DBDynamo, BetHandler Queue, SecretKeyService SecretService)
        {
            _DBDynamo = DBDynamo;
            _Queue = Queue;
            sqsBet = SecretService._secretKeys.SQS_BET;
            sqsWinner = SecretService._secretKeys.SQS_WINNERS;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create()
        {
            NewRoulette newRoulette = new NewRoulette { Bets = new List<RoulettePlayer>() };
            await _DBDynamo.Insert(newRoulette);

            return Ok(newRoulette);
        }

        [HttpPatch("open")]
        public async Task<IActionResult> Open([FromQuery] string Id)
        {
            NewRoulette newRoulette = new NewRoulette
            {
                Id = Id,
                Status = true,
                Bets = new List<RoulettePlayer>()
            };
            await _DBDynamo.Update(newRoulette);

            return Ok(newRoulette);
        }

        [HttpPatch("close")]
        public async Task<IActionResult> Close([FromQuery] string Id)
        {
            int winnerMove = new Random().Next(0, 36);
            NewRoulette newRoulette = new NewRoulette{ Id = Id,Status = false, WinnerMove = winnerMove};
            var roulettes = await _DBDynamo.Get(newRoulette);
            foreach (var player in roulettes.Bets)
            {
                if (RouletteCalculator.IsWinner(winnerMove: winnerMove, player: player))
                {
                    var jsonObject = JsonConvert.SerializeObject(
                        new RouletteWinner {Player = player, Prize = RouletteCalculator.Prize(player)
                    });
                    await _Queue.SendMessageAsync(message: jsonObject, sqsUrl: sqsWinner);
                }
            }
            await _DBDynamo.Insert(newRoulette);

            return Ok(roulettes);
        }

        [HttpPost("bet")]
        public async Task<IActionResult> Bet([FromBody] RouletteBet Bet)
        {
            UserDynamo userInfo = await _DBDynamo.Get(new UserDynamo { Email = ClaimsHelper.GetValue(HttpContext) });
            if (Quantity.IsBigger(baseValue: Bet.Player.Cash, comparedValue: userInfo.Cash))
                return BadRequest("Not enough cash. Charge your account and try later please.");
            Bet.Player.Email = userInfo.Email;
            var jsonObject = JsonConvert.SerializeObject(Bet);
            await _Queue.SendMessageAsync(message: jsonObject, sqsUrl: sqsBet);

            return StatusCode(201,Bet);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<NewRoulette> roulettes = await _DBDynamo.GetAll<NewRoulette>("test_chris_roulettes");

            return Ok(roulettes);
        }
    }
}
