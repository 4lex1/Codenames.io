namespace Codenames.Classes
{
    public class Card
    {
        public int Id           { get; set; }

        public string Text      { get; set; }
        public string Css       { get; set; }

        public CardColor Color  { get; set; }

        public Card(int id, string text, CardColor color)
        {
            Id      = id;
            Text    = text;
            Color   = color;
        }
    }
}
