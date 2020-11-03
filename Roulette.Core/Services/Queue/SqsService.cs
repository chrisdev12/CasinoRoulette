using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Roulette.Core.Services.Security.System;
using System.Threading.Tasks;

namespace Roulette.Core.Services.Queue
{
    public class BetHandler : EnviromentVariables
    {

        private readonly IAmazonSQS _amazonSQSClient;
        public BetHandler(IAmazonSQS client)
        {
            _amazonSQSClient = client;
        }

        public async Task<ReceiveMessageResponse> ReceiveMessageAsync(string sqsUrl)
        {
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(sqsUrl);

            return await _amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);
        }

        public async Task<DeleteMessageResponse> DeleteMessageAsync(Message message, string sqsUrl)
        {
            DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest(sqsUrl, message.ReceiptHandle);

            return await _amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);
        }

        public async Task<SendMessageResponse> SendMessageAsync(string message, string sqsUrl)
        {
            SendMessageRequest sendMessageRequest = new SendMessageRequest(sqsUrl, message);

            return await _amazonSQSClient.SendMessageAsync(sendMessageRequest);
        }
    }
}
