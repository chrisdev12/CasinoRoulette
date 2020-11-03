using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;
using Roulette.Core.Context;
using Roulette.Core.Entities.DTO.User;
using Roulette.Core.Entities.Roulette;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WinnersHandlerLambda
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
                context.Logger.Log("Winner processed");
            }
        }

        private async Task<bool> ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            var winner = JsonConvert.DeserializeObject<RouletteWinner>(message.Body);
            await UpdatePlayer(player: winner.Player, prize: winner.Prize);

            return true;
        }

        public async Task UpdatePlayer(RoulettePlayer player, double prize)
        {
            var user = new UserDynamo { Email = player.Email };
            var currentUser = await _DynamoDb.Get(user);
            currentUser.Cash += prize;

            await _DynamoDb.Insert(currentUser);
        }
    }
}
