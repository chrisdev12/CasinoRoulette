using System;

namespace Roulette.Core.Services.Security.System
{
    public class EnviromentVariables
    {
        protected readonly string cognitoPoolId = Environment.GetEnvironmentVariable("COGNITO_POOL_ID");
        protected readonly string cognitoAppClientId = Environment.GetEnvironmentVariable("COGNITO_APP_ID");
        protected readonly string awsSecretKeysName = Environment.GetEnvironmentVariable("AWS_SECRET_NAME");
        protected readonly string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");
        protected readonly string awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
        protected readonly string awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
        protected readonly string awsLogGroup = Environment.GetEnvironmentVariable("AWS_LOG_GROUP");
        protected readonly string logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL");
    }
}
