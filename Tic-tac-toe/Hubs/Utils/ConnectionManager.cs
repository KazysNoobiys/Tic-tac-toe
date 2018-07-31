namespace Tic_tac_toe.Hubs.Utils
{
    public class ConnectionManager
    {
        private static ConnectionMapping<ServerUser> _connectionMapping;
        public static ConnectionMapping<ServerUser> ConnectionMapping => _connectionMapping ?? (_connectionMapping = new ConnectionMapping<ServerUser>());
    }
}