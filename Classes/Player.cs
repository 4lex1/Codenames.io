namespace Codenames.Classes
{
    public class Player
    {
        public string ConnectionId  { get; set; }
        public string GameId        { get; set; }
        public string Name          { get; set; }

        public TeamColor Team       { get; set; }

        public bool IsManager       { get; set; }
        public bool IsLeader        { get; set; }

        public Player(string connectionId, string name, TeamColor team, bool isManager, bool isLeader)
        {
            ConnectionId    = connectionId;
            Name            = name;
            Team            = team;
            IsManager       = isManager;
            IsLeader        = isLeader;
        }
    }
}
