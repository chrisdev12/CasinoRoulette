using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace Roulette.Core.Entities.DTO.User
{
    [DynamoDBTable("test_chris_users")]
    public class UserDynamo
    {
        [DynamoDBHashKey]
        [EmailAddress]
        public string Email { get; set; }
        public double Cash { get; set; }
    }
}
