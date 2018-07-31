using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Tic_tac_toe.Hubs.Utils;

namespace Tic_tac_toe.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly ConnectionMapping<ServerUser> _connectionMapping = ConnectionManager.ConnectionMapping;
        private readonly List<Game> _games = GameManager.Games;
        public void NextTurn(string numSector)
        {
            var name = Context.User.Identity.Name;
            var game = _games.Find(g => g.Player1.Name == name || g.Player2.Name == name);
            if (game.Winner == null)
            {
                if (game.PlayerTurn.Name == name)
                {
                    game.SetSector(Convert.ToInt32(numSector), game.Player1.Name == name ? game.Player1 : game.Player2);
                    var anotherPlayer = game.NextPlayer();
                    Clients.Caller.updateGameInterface(false, game.GetSectors());
                    Clients.User(anotherPlayer.Name).updateGameInterface(true, game.GetSectors());

                    if (game.Player1.Name == name)
                    {
                        Clients.Caller.itsYourTurn(false, true);
                        Clients.User(anotherPlayer.Name).itsYourTurn(true, false);
                    }
                    else
                    {
                        Clients.Caller.itsYourTurn(false, false);
                        Clients.User(anotherPlayer.Name).itsYourTurn(true, true);
                    }

                    var winner = game.IsWinner();
                    if (winner != null)
                    {
                        game.Winner = winner;
                        if (anotherPlayer != winner)
                        {
                            Clients.Caller.youWin(game.GetSectors(), game.WinQuery);
                            Clients.User(anotherPlayer.Name).youLose(game.GetSectors(), game.WinQuery);
                        }
                        else
                        {
                            Clients.Caller.youLose(game.GetSectors(), game.WinQuery);
                            Clients.User(anotherPlayer.Name).youWin(game.GetSectors(), game.WinQuery);
                        }

                    }
                }
            }

        }

        public override Task OnConnected()
        {
            var name = Context.User.Identity.Name;
            var game = _games.Find(g => g.Player1.Name == name || g.Player2.Name == name);
            var player = game.Player1.Name == name ? game.Player1 : game.Player2;
            if (game != null)
            {
                if (game.PlayerTurn == player)
                {
                    if (player.NumPlayer == -1)
                    {
                        Clients.Caller.itsYourTurn(true, true);
                    }
                    else
                    {
                        Clients.Caller.itsYourTurn(true, false);
                    }
                    Clients.Caller.updateGameInterface(true, game.GetSectors());
                }
                else
                {
                    if (player.NumPlayer == -1)
                    {
                        Clients.Caller.itsYourTurn(false, true);
                    }
                    else
                    {
                        Clients.Caller.itsYourTurn(false, false);
                    }
                    Clients.Caller.updateGameInterface(false, game.GetSectors());
                }
            }
            else
            {
                Clients.Caller.redirectToHome();
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = Context.User.Identity.Name;
            var game = _games.Find(g => g.Player1.Name == name || g.Player2.Name == name);
            if (game != null)
            {
                var player = game.Player1.Name == name ? game.Player1 : game.Player2;
                player.Status = UserStatus.Free;
                _connectionMapping.Remove(player);

                var anotherPlayer = game.Player1.Name != name ? game.Player1 : game.Player2;
                if (anotherPlayer.Status == UserStatus.InGame)
                {
                    anotherPlayer.Status = UserStatus.Free;
                    Clients.User(anotherPlayer.Name).redirectToHome();
                }

                player.Game = null;
                anotherPlayer.Game = null;
                _games.Remove(game);
                var anotherPlayerConttections = _connectionMapping.GetAllConnections(anotherPlayer);
                GlobalHost.ConnectionManager.GetHubContext<ConnectHub>().Clients
                    .AllExcept(anotherPlayerConttections.FirstOrDefault());
            }


            return base.OnDisconnected(stopCalled);
        }
    }
}