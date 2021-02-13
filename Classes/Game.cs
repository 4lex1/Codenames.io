using System;
using System.Collections.Generic;
using System.Linq;

namespace Codenames.Classes
{
    public class Game
    {
        const string CSS_BLUE_PLAYED    = "bluePlayed";
        const string CSS_RED_PLAYED     = "redPlayed";
        const string CSS_BLACK_PLAYED   = "blackPlayed";
        const string CSS_WHITE_PLAYED   = "whitePlayed";


        static bool firstGame = true;

        public bool HasStarted  { get; set; }
        public bool GameOver    { get; set; }

        public PlayersList Players = new PlayersList();

        public CardsGrid Cards;

        public TeamColor PlayingTeam { get; set; }

        public int BlueWordsFound;
        public int RedWordsFound;

        public void InitialiseFromLobby(Lobby lobby)
        {
            Players.List = new List<Player>(lobby.Players.List);
        }

        public void New(int lastRedLeader, int lastBlueLeader)
        {
            GameOver         = false;
            BlueWordsFound   = 0;
            RedWordsFound    = 0;

            foreach (var player in Players.List) player.IsLeader = false;

            PlayingTeam = new Random().Next(0, 2) == 0 ? TeamColor.Blue : TeamColor.Red;

            Cards = new CardsGrid(PlayingTeam);

            if (firstGame)
            {
                firstGame = false;

                Players.List.First(p => p.Team == TeamColor.Red).IsLeader = true;
                Players.List.First(p => p.Team == TeamColor.Blue).IsLeader = true;
            }
            else
            {
                var bluePlayers = Players.List.Where(p => p.Team == TeamColor.Blue).ToList();
                var redPlayers  = Players.List.Where(p => p.Team == TeamColor.Red).ToList();

                int blueLeader = ++lastBlueLeader;
                if (blueLeader >= bluePlayers.Count) blueLeader = 0;

                int redLeader = ++lastRedLeader;
                if (redLeader >= redPlayers.Count) redLeader = 0;

                bluePlayers[blueLeader].IsLeader = true;
                redPlayers[redLeader].IsLeader   = true;
            }
        }

        public bool PlayCard(TeamColor team, int card, ref string newClass)
        {
            if (team == TeamColor.Blue)
            {
                switch (Cards.List[card].Color)
                {
                    case CardColor.Blue:
                        ++BlueWordsFound;
                        newClass = CSS_BLUE_PLAYED;
                        if (CheckVictory(team)) return true;
                        break;

                    case CardColor.Red:
                        ++RedWordsFound;
                        newClass = CSS_RED_PLAYED;
                        if (CheckVictory(TeamColor.Red))
                        {
                            NextRound();
                            return true;
                        }
                        NextRound();
                        break;

                    case CardColor.White:
                        newClass = CSS_WHITE_PLAYED;
                        NextRound();
                        break;

                    case CardColor.Black:
                        newClass = CSS_BLACK_PLAYED;
                        NextRound();
                        return true;
                }
            }
            else
            {
                switch (Cards.List[card].Color)
                {
                    case CardColor.Blue:
                        ++BlueWordsFound;
                        newClass = CSS_BLUE_PLAYED;
                        if (CheckVictory(TeamColor.Blue))
                        {
                            NextRound();
                            return true;
                        }
                        NextRound();
                        break;

                    case CardColor.Red:
                        ++RedWordsFound;
                        newClass = CSS_RED_PLAYED;
                        if (CheckVictory(team)) return true;
                        break;

                    case CardColor.White:
                        newClass = CSS_WHITE_PLAYED;
                        NextRound();
                        break;

                    case CardColor.Black:
                        newClass = CSS_BLACK_PLAYED;
                        NextRound();
                        return true;
                }
            }

            return false;
        }

        public bool CheckVictory(TeamColor team)
        {
            switch (team)
            {
                case TeamColor.Red:   return RedWordsFound  == Cards.RedWordsTotal;
                case TeamColor.Blue:  return BlueWordsFound == Cards.BlueWordsTotal;
            }
            return false;
        }

        public void NextRound()
        {
            switch (PlayingTeam)
            {
                case TeamColor.Blue: PlayingTeam = TeamColor.Red;  break;
                case TeamColor.Red:  PlayingTeam = TeamColor.Blue; break;
            }
        }
    }
}
