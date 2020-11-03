using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;
using Roulette.Core.Context;
using Roulette.Core.Entities.DTO.User;
using Roulette.Core.Entities.Roulette;
using Roulette.Core.Entities.Roulettes;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BidsHandlerLambda
{
    public class Function
    {
        private readonly DynamoContext _DynamoDb;
        public Function()
        {
            AmazonDynamoDBClient AmazonClient = new AmazonDynamoDBClient();
            _DynamoDb = new DynamoContext(AmazonClient);
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
                context.Logger.Log("Bet processed");
            }
        }

        private async Task<bool> ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            var rouletteBet = JsonConvert.DeserializeObject<RouletteBet>(message.Body);
            NewRoulette currentRoulette = new NewRoulette { Id = rouletteBet.RouletteId };
            await UpdateRoulette(rouletteToRetrieve: currentRoulette, rouletteUpdated: rouletteBet);
            await UpdateCash(new UserDynamo
            {
                Cash = rouletteBet.Player.Cash,
                Email = rouletteBet.Player.Email
            });
            return true;
        }

        public async Task UpdateRoulette(NewRoulette rouletteToRetrieve, RouletteBet rouletteUpdated)
        {
            var rouletteRetrived = await _DynamoDb.Get(rouletteToRetrieve);
            if (!rouletteRetrived.Status)
                return;
            rouletteRetrived.Bets.Add(rouletteUpdated.Player);
            await _DynamoDb.Insert(rouletteRetrived);
        }

        public async Task UpdateCash(UserDynamo UserDynamo)
        {
            var currentUser = await _DynamoDb.Get(UserDynamo);
            currentUser.Cash -= UserDynamo.Cash;

            await _DynamoDb.Insert(currentUser);
        }
    }
}
