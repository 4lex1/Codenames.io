using Codenames.Classes;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Codenames.Hubs
{
    public class LobbyHub : Hub
    {
        const int MIN_PLAYERS_PER_TEAM = 2;

        private static Lobby lobbyInstance;
        public static Lobby Lobby
        {
            get
            {
                if (lobbyInstance == null) lobbyInstance = new Lobby();
                return lobbyInstance;
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var player = Lobby.Players.GetPlayerByConnectionId(Context.ConnectionId);
                if (player != null)
                {
                    Lobby.RemovePlayer(player);
                    _ = GetPlayers();
                }

            }
            catch
            {

            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinTeam(TeamColor team, string playerName)
        {
            var player = new Player(Context.ConnectionId, playerName, team, false, false);
            Lobby.AddPlayer(player);

            if (Lobby.Players.List.FirstOrDefault(p => p.IsManager) == null && Lobby.Players.List.Count > 0)
            {
                Lobby.Players.List.First().IsManager = true;
            }

            await GetPlayers();
        }

        public async Task LeaveTeam()
        {
            var player = Lobby.Players.GetPlayerByConnectionId(Context.ConnectionId);
            if (player != null)
            {
                Lobby.RemovePlayer(player);
                await GetPlayers();
            }
        }

        public async Task GetPlayers()
        {
            await Clients.All.SendAsync("GetPlayers", Lobby.Players.AsJson);
        }

        public async Task StartGame()
        {
            var player = Lobby.Players.GetPlayerByConnectionId(Context.ConnectionId);
            if (!player?.IsManager ?? false) return;

            if (Lobby.Players.List.Where(p => p.Team == TeamColor.Red).Count()  >= MIN_PLAYERS_PER_TEAM &&
                Lobby.Players.List.Where(p => p.Team == TeamColor.Blue).Count() >= MIN_PLAYERS_PER_TEAM)
            {
                GameHub.Game.InitialiseFromLobby(Lobby);
                await Clients.All.SendAsync("StartGame");
            }
        }
    }
}
