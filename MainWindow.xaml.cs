using PokemonTCG.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PokemonTCG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LINQ linq = new LINQ();

        public MainWindow()
        {
            InitializeComponent();

            ResetUI();

            PlayerOneSelect.Items.Add("Select Player...");
            PlayerTwoSelect.Items.Add("Select Player...");

            PlayerOneDamage.IsEnabled = false;
            PlayerTwoDamage.IsEnabled = false;

            foreach (Player player in linq.GetPlayers())
            {
                PlayerOneSelect.Items.Add(player.Name);
                PlayerTwoSelect.Items.Add(player.Name);
            }
        }

        public void UpdateDecks(string playerOneDeckName, string playerTwoDeckName, Match match, PokemonEntities1 db)
        {

            Deck playerOneDeck = db.Decks.SingleOrDefault(x => x.Name == playerOneDeckName);
            Deck playerTwoDeck = db.Decks.SingleOrDefault(x => x.Name == playerTwoDeckName);

            string playerOneName = db.Players.Where(x => x.PlayerID == playerOneDeck.PlayerId).Select(x => x.Name).FirstOrDefault();
            string playerTwoName = db.Players.Where(x => x.PlayerID == playerTwoDeck.PlayerId).Select(x => x.Name).FirstOrDefault();

            if (playerOneDeck != null && playerTwoDeck != null)
            {
                playerOneDeck.TotalGamesPlayes++;
                playerTwoDeck.TotalGamesPlayes++;

                if (match.Winner == playerOneName)
                {
                    playerOneDeck.Wins++;
                }

                if (match.Winner == playerTwoName)
                {
                    playerTwoDeck.Wins++;
                }

                //new high damage, update DB record
                if (match.PlayerOneDamage > playerOneDeck.HighestDamage)
                {
                    playerOneDeck.HighestDamage = match.PlayerOneDamage;
                }

                if (match.PlayerTwoDamage > playerTwoDeck.HighestDamage)
                {
                    playerTwoDeck.HighestDamage = match.PlayerTwoDamage;
                }

                //new low damage, update DB record
                if (match.PlayerOneDamage < playerOneDeck.LowestDamage || playerOneDeck.LowestDamage == 0)
                {
                    playerOneDeck.LowestDamage = match.PlayerOneDamage;
                }

                if (match.PlayerTwoDamage < playerTwoDeck.LowestDamage || playerTwoDeck.LowestDamage == 0)
                {
                    playerTwoDeck.LowestDamage = match.PlayerTwoDamage;
                }
            }

            db.Entry(playerOneDeck).State = System.Data.Entity.EntityState.Modified;
            db.Entry(playerTwoDeck).State = System.Data.Entity.EntityState.Modified;

        }

        public void UpdatePlayers(string playerOneName, string playerTwoName, Match match, PokemonEntities1 db)
        {
            Player playerOne = db.Players.Where(x => x.Name == playerOneName).FirstOrDefault();
            Player playerTwo = db.Players.Where(x => x.Name == playerTwoName).FirstOrDefault();

            //update players
            if (playerOne != null && playerTwo != null)
            {
                playerOne.TotalGamesPlayed++;
                playerTwo.TotalGamesPlayed++;

                if (match.Winner == playerOneName)
                {
                    playerOne.Wins++;
                }

                if (match.Winner == playerTwoName)
                {
                    playerTwo.Wins++;
                }

                if (match.PlayerOneDamage > playerOne.HighestDamage)
                {
                    playerOne.HighestDamage = match.PlayerOneDamage;
                }

                if (match.PlayerTwoDamage > playerTwo.HighestDamage)
                {
                    playerTwo.HighestDamage = match.PlayerTwoDamage;
                }

                if (match.PlayerOneDamage < playerOne.LowestDamage || playerOne.LowestDamage == 0)
                {
                    playerOne.LowestDamage = match.PlayerOneDamage;
                }

                if (match.PlayerTwoDamage < playerTwo.LowestDamage || playerTwo.LowestDamage == 0)
                {
                    playerTwo.LowestDamage = match.PlayerTwoDamage;
                }
            }

            db.Entry(playerOne).State = System.Data.Entity.EntityState.Modified;
            db.Entry(playerTwo).State = System.Data.Entity.EntityState.Modified;
        }

        public void ResetUI()
        {
            PlayerOneDeckSelect.IsEnabled = false;
            PlayerTwoDeckSelect.IsEnabled = false;
            PlayerOneDamage.Clear();
            PlayerTwoDamage.Clear();
            PlayerOneDeckSelect.Items.Clear();
            PlayerTwoDeckSelect.Items.Clear();
            PlayerOneSelect.SelectedIndex = 0;
            PlayerTwoSelect.SelectedIndex = 0;
            PlayerTwoDeckSelect.SelectedIndex = 0;
            PlayerOneDeckSelect.SelectedIndex = 0;
            MatchWinner.SelectedIndex = 0;
        }

        public void SetupDeckList(ComboBox deckBox, ComboBox playerBox)
        {
            deckBox.Items.Clear();

            if (playerBox.Text != "Select Player...")
            {
                deckBox.IsEnabled = true;
                deckBox.Items.Add("Select Deck...");
                deckBox.SelectedIndex = 0;

                //Get List Of Decks
                using (PokemonEntities1 db = new PokemonEntities1())
                {
                    int playerId = db.Players.Where(x => x.Name == playerBox.Text).Select(x => x.PlayerID).FirstOrDefault();

                    foreach (Deck deck in linq.GetDecksList(playerId))
                    {
                        List<Type> types = db.Types.Where(x => x.DeckID == deck.DeckID).ToList();
                        deckBox.Items.Add(deck.Name);
                    }
                }
            }
        }

        private void NewPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            NewPlayer window = new NewPlayer();
            window.Owner = this;
            window.ShowDialog();
        }

        private void NewDeckBtn_Click(object sender, RoutedEventArgs e)
        {
            NewDeck window = new NewDeck();
            window.Owner = this;
            window.ShowDialog();
        }

        private void PlayerOneSelect_DropDownClosed(object sender, EventArgs e)
        {
            SetupDeckList(PlayerOneDeckSelect, PlayerOneSelect);
        }

        private void PlayerTwoSelect_DropDownClosed(object sender, EventArgs e)
        {
            SetupDeckList(PlayerTwoDeckSelect, PlayerTwoSelect);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Match match = new Match
            {
                Date = DateTime.Today,
                PlayerOne = PlayerOneSelect.Text,
                PlayerOneDeck = PlayerOneDeckSelect.Text,
                PlayerOneDamage = int.Parse(PlayerOneDamage.Text),
                PlayerTwo = PlayerTwoSelect.Text,
                PlayerTwoDeck = PlayerTwoDeckSelect.Text,
                PlayerTwoDamage = int.Parse(PlayerTwoDamage.Text),
                Winner = MatchWinner.Text
            };

            using (PokemonEntities1 db = new PokemonEntities1())
            {
                db.Matches.Add(match);
                UpdatePlayers(PlayerOneSelect.Text, PlayerTwoSelect.Text, match, db);
                UpdateDecks(PlayerOneDeckSelect.Text, PlayerTwoDeckSelect.Text, match, db);
                db.SaveChanges();
            }

            ResetUI();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        private void MatchHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            MatchHistory window = new MatchHistory();
            window.Owner = this;
            window.ShowDialog();
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            Stats window = new Stats();
            window.Owner = this;
            window.ShowDialog();
        }

        private void PlayerOneDeckSelect_DropDownClosed(object sender, EventArgs e)
        {
            if (PlayerOneDeckSelect.Text != "Select Deck...")
            {
                PlayerOneDamage.IsEnabled = true;
            }

            else
            {
                PlayerOneDamage.IsEnabled = false;
            }
        }

        private void PlayerTwoDeckSelect_DropDownClosed(object sender, EventArgs e)
        {
            if (PlayerTwoDeckSelect.Text != "Select Deck...")
            {
                PlayerTwoDamage.IsEnabled = true;
            }

            else
            {
                PlayerTwoDamage.IsEnabled = false;
            }
        }
    }
}
