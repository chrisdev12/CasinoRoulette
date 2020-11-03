using Roulette.Core.Entities.Roulette;

namespace Roulette.Core.Helpers
{
    public class RouletteCalculator
    {
        public static double Prize(RoulettePlayer player)
        {
            if (player.BetType == "number")
            {
                return player.Cash * 5;
            }

            return player.Cash * 1.8;
        }

        public static bool IsWinner(int winnerMove, RoulettePlayer player)
        {
            string typeBid = player.BetType;
            int moveBid = player.Bet;

            if (winnerMove % 2 == 0 && moveBid % 2 == 0 && typeBid == "color")
                return true;
            if (winnerMove % 2 != 0 && moveBid % 2 != 0 && typeBid == "color")
                return true;
            if (typeBid == "number" && winnerMove == moveBid)
                return true;

            return false;
        }
    }
}
