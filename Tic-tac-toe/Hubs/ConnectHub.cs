using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Tic_tac_toe.Hubs.Utils;

namespace Tic_tac_toe.Hubs
{
    [Authorize]
    public class ConnectHub : Hub
    {
        private readonly ConnectionMapping<ServerUser> _connectionMapping = ConnectionManager.ConnectionMapping;
        private readonly List<Game> _games = GameManager.Games;
        public void UpdateListUsers()
        {
            var name = Context.User.Identity.Name;
            var allUsers = _connectionMapping.GetAllKeys();
            var user = allUsers.FirstOrDefault(u => u.Name == name);
            if (user != null && user.Status == UserStatus.Free)
            {
                var otherUsers = from u in allUsers
                                 where u.Name != name
                                 select u;
                Clients.Caller.updateConnections(otherUsers);
            }

        }

        public void NewGame(string namePlayer2)
        {
            var namePlayer1 = Context.User.Identity.Name;
            var player1 = _connectionMapping.GetAllKeys().FirstOrDefault(u => u.Name == namePlayer1);
            var player2 = _connectionMapping.GetAllKeys().FirstOrDefault(u => u.Name == namePlayer2);
            var game = new Game(player1, player2);
            _games.Add(game);
            player1.Game = player2.Game = game;
            player1.Status = player2.Status = UserStatus.InGame;
            player1.NumPlayer = -1;
            player2.NumPlayer = 1;
            //List<string> list = new List<string>();
            //list.Add(_connectionMapping.GetAllConnections(player1).FirstOrDefault());
            //list.Add(_connectionMapping.GetAllConnections(player2).FirstOrDefault());
            //Clients.Clients(list).inGame();           

            Clients.Users(new List<string>() { player1.Name, player2.Name }).inGame();

        }
        public void Invite(string nameInvitedUser)
        {
            var name = Context.User.Identity.Name;
            var invitedUser = _connectionMapping.GetAllKeys().FirstOrDefault(u => u.Name == nameInvitedUser);
            var connectionId = _connectionMapping.GetAllConnections(invitedUser).FirstOrDefault();
            Clients.Client(connectionId).invite(name);
        }

        public void RefuseInvite(string nameInvitedUser)
        {
            var name = Context.User.Identity.Name;
            var invitedUser = _connectionMapping.GetAllKeys().FirstOrDefault(u => u.Name == nameInvitedUser);
            var connectionId = _connectionMapping.GetAllConnections(invitedUser).FirstOrDefault();
            Clients.Client(connectionId).refuseInvite(name);
        }
        public override Task OnConnected()
        {
            var name = Context.User.Identity.Name;
            var serverUser = _connectionMapping.GetAllKeys().FirstOrDefault(u => u.Name == name);
            if (serverUser == null)
            {
                serverUser = new ServerUser() { Name = name, Status = UserStatus.Free };
            }
            _connectionMapping.Add(serverUser, Context.ConnectionId);
            Clients.Others.userConnectionsChanged();
            var allUsers = _connectionMapping.GetAllKeys();
            var othetUsers = from u in allUsers
                             where u.Name != name
                             select u;
            Clients.Caller.updateConnections(othetUsers);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = Context.User.Identity.Name;
            var serverUser = _connectionMapping.GetAllKeys().FirstOrDefault(u => u.Name == name);
            if (serverUser != null)
            {
                if (serverUser.Status == UserStatus.InGame)
                {
                    _connectionMapping.Remove(serverUser, Context.ConnectionId, true);
                }
                else
                {
                    _connectionMapping.Remove(serverUser, Context.ConnectionId);
                }
                Clients.Others.userConnectionsChanged();
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}