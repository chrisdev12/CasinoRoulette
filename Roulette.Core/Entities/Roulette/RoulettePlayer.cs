using System.ComponentModel.DataAnnotations;

namespace Roulette.Core.Entities.Roulette
{
    public class RoulettePlayer
    {
        public double Cash { get; set; }
        public string Email { get; set; }
        [EnumDataType(typeof(BetTypeEnum), ErrorMessage = "The bet only can be number or color")]
        public string BetType { get; set; }
        [Range(0, 36, ErrorMessage = "The values allowed are in range of 0-36")]
        public int Bet { get; set; }
    }
}
