using System.ComponentModel.DataAnnotations;

namespace Roulette.Core.Entities.Roulette
{
    public class RouletteBet
    {
        [Required]
        public string RouletteId { get; set; }
        [Required]
        public RoulettePlayer Player { get; set; }
    }
}
