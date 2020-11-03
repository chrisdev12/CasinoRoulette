using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using Roulette.Core.Entities;
using Roulette.Core.Services.Security.System;
using System;

namespace Roulette.Core.Services.Security
{
    public class SecretKeyService : EnviromentVariables
    {
        public readonly SecretKeys _secretKeys;
        public SecretKeyService()
        {
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(awsAccessKey, awsSecretKey, RegionEndpoint.GetBySystemName(awsRegion));
            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = awsSecretKeysName,
                VersionStage = "AWSCURRENT"
            };

            GetSecretValueResponse response;
            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (Exception)
            {
                throw;
            }
            if (response.SecretString != null)
                _secretKeys = JsonConvert.DeserializeObject<SecretKeys>(response.SecretString);
        }
    }
}
