using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Codenames.Classes
{
    public class CardsGrid
    {
        const int MAX_CARDS_PER_TEAM = 8;
        const int MIN_CARDS_PER_TEAM = 7;
        const int TOTAL_CARDS = 25;

        public List<Card> List { get; set; } = new List<Card>();

        public string AsJson        => JsonConvert.SerializeObject(List);
        public string AsSecretJson  => JsonConvert.SerializeObject(List.Select(c => new Card(c.Id, c.Text, CardColor.White)).ToList());

        public int RedWordsTotal    { get; set; }
        public int BlueWordsTotal   { get; set; }

        public static List<string> Words => File.ReadAllLines(Path.Combine(Utilities.AppPath, "mots.txt")).Distinct().ToList();


        public CardsGrid(TeamColor firstTeam)
        {
            List = new List<Card>();

            var words = Words.OrderBy(_ => Guid.NewGuid()).Take(TOTAL_CARDS).ToArray();
            for (int i = 0; i < TOTAL_CARDS; ++i)
            {
                List.Add(new Card(i, words[i], CardColor.None));
            }

            var rnd = new Random();
            List[rnd.Next(0, TOTAL_CARDS)].Color = CardColor.Black;

            int blueCount   = 0;
            int redCount    = 0;

            BlueWordsTotal  = firstTeam == TeamColor.Blue   ? MAX_CARDS_PER_TEAM : MIN_CARDS_PER_TEAM;
            RedWordsTotal   = firstTeam == TeamColor.Red    ? MAX_CARDS_PER_TEAM : MIN_CARDS_PER_TEAM;

            while(blueCount != BlueWordsTotal)
            {
                var position = rnd.Next(0, TOTAL_CARDS);
                if (List[position].Color == CardColor.None)
                {
                    List[position].Color = CardColor.Blue;
                    ++blueCount;
                }
            }
            
            while (redCount != RedWordsTotal)
            {
                var position = rnd.Next(0, TOTAL_CARDS);
                if (List[position].Color == CardColor.None)
                {
                    List[position].Color = CardColor.Red;
                    ++redCount;
                }
            }

            for (int i = 0; i < TOTAL_CARDS; ++i)
            {
                if (List[i].Color == CardColor.None) List[i].Color = CardColor.White;
            }
        }
    }
}
