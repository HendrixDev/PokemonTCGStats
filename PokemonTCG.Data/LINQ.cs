using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonTCG.Data
{
    
    public class LINQ
    {
        PokemonEntities1 db = new PokemonEntities1();

        public List<Deck> GetDecksList()
        {
            List<Deck> decks = db.Decks.ToList();
            return decks;
        }

        /// <summary>
        /// Returns list of decks for a specific user
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<Deck> GetDecksList(int playerId)
        {
            List<Deck> decks = db.Decks.Where(x => x.Player.PlayerID == playerId).ToList();
            return decks;
        }


        public Deck GetDeck(string deckName)
        {
            Deck playerDeck = db.Decks.SingleOrDefault(x => x.Name == deckName);
            return playerDeck;
        }


        public Player GetPlayerByName(string playerName)
        {
            Player player = db.Players.Where(x => x.Name == playerName).FirstOrDefault();
            return player;
        }

        public List<Player> GetPlayers()
        {
            List<Player> players = db.Players.ToList();
            return players;
        }

        public List<Match> GetMatches()
        {
            List<Match> matches = db.Matches.OrderBy(x => x.Date).ToList();
            return matches;
        }

        public int GetPlayerIdByName(string name)
        {
            int playerId = db.Players.Where(x => x.Name == name).Select(x => x.PlayerID).FirstOrDefault();
            return playerId;
        }

        public List<Type> GetListOfTypesByDeckId(int deckId)
        { 
            List<Type> types = db.Types.Where(x => x.DeckID == deckId).ToList();
            return types;
        }

    }
}
