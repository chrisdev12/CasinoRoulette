using Amazon.DynamoDBv2.DataModel;
using Roulette.Core.Entities.Roulette;
using System;
using System.Collections.Generic;

namespace Roulette.Core.Entities.Roulettes
{
    [DynamoDBTable("test_chris_roulettes")]
    public class NewRoulette
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool Status { get; set; } = false;
        public List<RoulettePlayer> Bets { get; set; }
        public int WinnerMove { get; set; }
    }
}
