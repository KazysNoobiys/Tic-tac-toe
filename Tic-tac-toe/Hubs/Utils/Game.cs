using System;
using System.Collections.Generic;

namespace Tic_tac_toe.Hubs.Utils
{
    public class Game
    {
        public ServerUser Player1 { get; set; }
        public ServerUser Player2 { get; set; }
        public ServerUser PlayerTurn { get; set; }
        public ServerUser Winner { get; set; }

        public int[] WinQuery { get; set; }

        private readonly int[,] _sectors;


        public Game(ServerUser player1, ServerUser player2)
        {
            Player1 = player1;
            Player2 = player2;
            Random random = new Random();
            if (random.Next(0, 2) == 0)
            {
                PlayerTurn = Player1;
            }
            else
            {
                PlayerTurn = Player2;
            }
            _sectors = new int[3, 3];
        }

        public int[,] GetSectors()
        {
            return _sectors;
        }

        public ServerUser NextPlayer()
        {
            PlayerTurn = PlayerTurn == Player1 ? Player2 : Player1;
            return PlayerTurn;
        }
        public void SetSector(int numSector, ServerUser numPlayer)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((i * 3 + j) == numSector)
                        _sectors[i, j] = numPlayer.NumPlayer;
                }
            }

        }

        public ServerUser IsWinner()
        {
            var winQuery = new List<int>();
            var sum = 0;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                { 
                    sum += _sectors[i, j];
                    winQuery.Add((i * 3 + j));
                }

                if (sum == -3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player1;
                }

                if (sum == 3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player2;
                }
            }

            sum = 0;
            winQuery.Clear();
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    winQuery.Add((i * 3 + j));
                    sum += _sectors[i, j];
                }
                if (sum == -3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player1;
                }

                if (sum == 3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player2;
                }
            }

            winQuery.Clear();
            sum = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j)
                    {
                        winQuery.Add((i * 3 + j));
                        sum += _sectors[i, j];
                    }
                }
                if (sum == -3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player1;
                }

                if (sum == 3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player2;
                }
            }

            winQuery.Clear();
            sum = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i + j == 2)
                    {
                        winQuery.Add((i * 3 + j));
                        sum += _sectors[i, j];
                    }
                }
                if (sum == -3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player1;
                }

                if (sum == 3)
                {
                    this.WinQuery = winQuery.ToArray();
                    return Player2;
                }
            }

            return null;
        }
    }
}