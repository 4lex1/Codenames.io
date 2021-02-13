using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Codenames.Classes
{
    public class PlayersList
    {
        public List<Player> List { get; set; } = new List<Player>();

        public Player GetPlayerByConnectionId(string connectionId)  => List.FirstOrDefault(p => p.ConnectionId.Equals(connectionId));
        public Player GetPlayerByGameId(string gameId)              => List.FirstOrDefault(p => p.GameId.Equals(gameId));

        public string AsJson => JsonConvert.SerializeObject(List);

        public bool AllReady => List.Where(p => !string.IsNullOrWhiteSpace(p.GameId)).Count() == List.Count;

        public void Add(Player p)       { try { List.Add(p); }      catch { } }
        public void Remove(Player p)    { try { List.Remove(p); }   catch { } }
        public void Clear() => List.Clear();
    }
}
