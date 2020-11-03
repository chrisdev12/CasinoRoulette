using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Roulette.Core.Entities.User;
using Roulette.Core.Services.Security.System;
using System.Threading.Tasks;

namespace Roulette.Core.Services.Security
{
    public class CognitoService : EnviromentVariables
    {
        AmazonCognitoIdentityProviderClient _clientCognito;
        public CognitoService()
        {
            _clientCognito = new AmazonCognitoIdentityProviderClient(awsAccessKey, awsSecretKey, RegionEndpoint.GetBySystemName(awsRegion));
        }
        public async Task<SignUpResponse> SignUp(User user)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = cognitoAppClientId,
                Username = user.Email,
                Password = user.Password
            };

            return await _clientCognito.SignUpAsync(signUpRequest);
        }
        public async Task<string> LogIn(User user)
        {
            CognitoUserPool userPool = new CognitoUserPool(cognitoPoolId, cognitoAppClientId, _clientCognito);
            CognitoUser userlog = new CognitoUser(user.Email, cognitoAppClientId, userPool, _clientCognito);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = user.Password
            };
            AuthFlowResponse auth = await userlog.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

            return auth.AuthenticationResult.IdToken;
        }
    }
}
