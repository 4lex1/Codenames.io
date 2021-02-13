namespace Codenames.Classes
{
    public class Lobby
    {
        public PlayersList Players { get; set; } = new PlayersList();

        public void Reset()
        {
            Players = new PlayersList();
        }

        public void AddPlayer(Player p)
        {
            Players.Add(p);

            if (Players.List.Count == 1) p.IsManager = true;
        }

        public void RemovePlayer(Player p)
        {
            Players.Remove(p);
        }
    }
}
