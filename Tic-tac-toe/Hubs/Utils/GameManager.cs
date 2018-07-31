using System.Collections.Generic;

namespace Tic_tac_toe.Hubs.Utils
{
    public class GameManager
    {
        private static List<Game> _games;
        public static List<Game> Games => _games ?? (_games = new List<Game>());

    }
}