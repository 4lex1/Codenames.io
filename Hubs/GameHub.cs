using Codenames.Classes;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Codenames.Hubs
{
    public class GameHub : Hub
    {
        private static Game game;
        public static Game Game
        {
            get
            {
                if (game == null) game = new Game();
                return game;
            }
        }

        public async Task AssociateGameId(string connectionId)
        {
            var player = Game.Players.GetPlayerByConnectionId(connectionId);
            if (player != null)
            {
                player.GameId = Context.ConnectionId;

                if (Game.Players.AllReady && !Game.HasStarted)
                {
                    Game.HasStarted = true;
                    Game.New(-1,-1);

                    await Clients.All.SendAsync("GetPlayers", Game.Players.AsJson);
                    await Clients.All.SendAsync("SetupLocalPlayer", Game.Players.AsJson);

                    var leaders = Game.Players.List.Where(p => p.IsLeader).Select(p => p.GameId).ToList();
                    await Clients.Clients(leaders).SendAsync("DisplayCards", Game.Cards.AsJson);
                    await Clients.AllExcept(leaders).SendAsync("DisplayCards", Game.Cards.AsSecretJson);
                    await SyncGameStatus();
                }
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var player = Game.Players.GetPlayerByGameId(Context.ConnectionId);

                if (player != null)
                {
                    Game.Players.Remove(player);
                }

                Clients.All.SendAsync("GetPlayers", Game.Players.AsJson);
            }
            catch { }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task NewGame()
        {
            var player = Game.Players.GetPlayerByGameId(Context.ConnectionId);
            if (player != null && player.IsManager)
            {
                await Clients.All.SendAsync("NewGame");

                var redPlayers = Game.Players.List.Where(p => p.Team == TeamColor.Red).ToList();
                var bluePlayers = Game.Players.List.Where(p => p.Team == TeamColor.Blue).ToList();

                var redLeader = redPlayers.IndexOf(redPlayers.FirstOrDefault(p => p.IsLeader));
                var blueLeader = bluePlayers.IndexOf(bluePlayers.FirstOrDefault(p => p.IsLeader));
                Game.New(redLeader, blueLeader);

                await Clients.All.SendAsync("GetPlayers", Game.Players.AsJson);
                await Clients.All.SendAsync("SetupLocalPlayer", Game.Players.AsJson);

                var leaders = Game.Players.List.Where(p => p.IsLeader).Select(p => p.GameId).ToList();
                await Clients.Clients(leaders).SendAsync("DisplayCards", Game.Cards.AsJson);
                await Clients.AllExcept(leaders).SendAsync("DisplayCards", Game.Cards.AsSecretJson);
                await SyncGameStatus();
            }
        }

        public async Task SyncGameStatus()
        {
            var gameStatus = new
            {
                PlayingTeam = Game.PlayingTeam,
                RedWordsLeft = Game.Cards.RedWordsTotal - Game.RedWordsFound,
                BlueWordsLeft = Game.Cards.BlueWordsTotal - Game.BlueWordsFound,
            };

            await Clients.All.SendAsync("SyncGameStatus", JsonConvert.SerializeObject(gameStatus));
        }

        public async Task PlayCard(int card)
        {
            if (Game.GameOver) return;

            var player = Game.Players.GetPlayerByGameId(Context.ConnectionId);
            if (player != null && player.IsLeader && player.Team == Game.PlayingTeam)
            {
                var newClass = "";
                var gameOver = Game.PlayCard(player.Team, card, ref newClass);
                await Clients.All.SendAsync("AnimateCard", card, newClass);

                if (gameOver)
                {
                    Game.GameOver = true;
                    await EndGame(Game.PlayingTeam);
                }
                else await SyncGameStatus();
            }
        }

        public async Task NextRound()
        {
            if (Game.GameOver) return;

            var player = Game.Players.GetPlayerByGameId(Context.ConnectionId);
            if (player != null && player.IsLeader && player.Team == Game.PlayingTeam)
            {
                Game.NextRound();
                await SyncGameStatus();
            }
        }

        public async Task EndGame(TeamColor winner)
        {
            await Clients.All.SendAsync("GameOver", winner);
        }
    }
}
