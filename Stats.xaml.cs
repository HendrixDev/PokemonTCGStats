using PokemonTCG.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PokemonTCG
{
    /// <summary>
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : Window
    {
        LINQ linq = new LINQ();

        //TODO: Fix bug - After selecting a player and a deck,
        //and then switching to another player, first players deck stats still show.

        public Stats()
        {
            InitializeComponent();

            List<Player> players = linq.GetPlayers();

            foreach (Player player in players)
            {
                StatsListPlayers.Items.Add(player.Name);
            }
        }

        private void StatsListPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatsListDecks.Items.Clear();

            using (PokemonEntities1 db = new PokemonEntities1())
            {
                Player player = db.Players.Where(x => x.Name == StatsListPlayers.SelectedItem.ToString()).FirstOrDefault();

                PlayerOverallStats.Content = player.Name + " overall stats";
                PlayerWins.Content = player.Wins;
                PlayerLosses.Content = player.TotalGamesPlayed - player.Wins;

                if (player.Wins == 0)
                {
                    PlayerWinRatio.Value = 0;
                }
                else
                {
                    PlayerWinRatio.Value = Math.Round(((double)player.Wins / player.TotalGamesPlayed) * 100, 2);
                }
                PlayerTotalGames.Content = player.TotalGamesPlayed;
                PlayerLowestDamage.Content = player.LowestDamage;
                PlayerHighestDamage.Content = player.HighestDamage;

                //add each players deck to the list of decks
                List<Deck> decks = linq.GetDecksList(player.PlayerID);

                foreach (Deck deck in decks)
                {
                    StatsListDecks.Items.Add(deck.Name);
                }
            }
        }

        private void StatsListDecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatsListDecks.SelectedItem != null)
            {
                Deck deck = linq.GetDeck(StatsListDecks.SelectedItem.ToString());

                DeckStats.Content = deck.Name + " deck stats";
                DeckWins.Content = deck.Wins;
                DeckLosses.Content = deck.TotalGamesPlayes - deck.Wins;

                if (deck.Wins == 0)
                {
                    DeckWinRatio.Value = 0;
                }
                else
                {
                    DeckWinRatio.Value = Math.Round(((double)deck.Wins / deck.TotalGamesPlayes) * 100, 2);
                }
                DeckTotalGames.Content = deck.TotalGamesPlayes;
                DeckLowestDamage.Content = deck.LowestDamage;
                DeckHighestDamage.Content = deck.HighestDamage;
            }
        }
    }
}
