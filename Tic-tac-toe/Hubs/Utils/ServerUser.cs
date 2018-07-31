using Newtonsoft.Json;

namespace Tic_tac_toe.Hubs.Utils
{
    public class ServerUser
    {
        public string Name { get; set; }
        public UserStatus Status { get; set; }
        public int NumPlayer { get; set; }
        [JsonIgnore]
        public Game Game { get; set; }
    }

    public enum UserStatus
    {
        Free,
        InGame
    }
}