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
            AddDefaultPlayerListBoxItems("Select Player...");
            PlayerOneDamage.IsEnabled = false;
            PlayerTwoDamage.IsEnabled = false;
            PopulatePlayerListBoxes();
        }

        public void UpdateDeck(string deckName, Match match)
        {
            Deck deck = linq.GetDeckByName(deckName);
            Player player = linq.GetPlayerByDeckId(deck);

            if (deck != null)
            {
                deck.TotalGamesPlayes++;

                if (match.Winner == player.Name)
                {
                    deck.Wins++;
                }

                if (CurrentPlayerIsPlayer1(player))
                {
                    if (CurrentDamageIsHigherThanPrevious(match.PlayerOneDamage, deck.HighestDamage))
                    {
                        deck.HighestDamage = match.PlayerOneDamage;
                    }

                    if (CurrentDamageIsLowerThanPreviousOrPreviousIsZero(match.PlayerOneDamage, deck.LowestDamage))
                    {
                        deck.LowestDamage = match.PlayerOneDamage;
                    }
                }

                else
                {
                    if (CurrentDamageIsHigherThanPrevious(match.PlayerTwoDamage, deck.HighestDamage))
                    {
                        deck.HighestDamage = match.PlayerTwoDamage;
                    }

                    if (CurrentDamageIsLowerThanPreviousOrPreviousIsZero(match.PlayerTwoDamage, deck.LowestDamage))
                    {
                        deck.LowestDamage = match.PlayerTwoDamage;
                    }
                }
                linq.UpdateDatabaseRecord(deck);
            }
        }

        public void UpdatePlayer(string playerName, Match match)
        {
            Player player = linq.GetPlayerByName(playerName);

            if (player != null)
            {
                player.TotalGamesPlayed++;

                if (match.Winner == player.Name)
                {
                    player.Wins++;
                }

                if (CurrentPlayerIsPlayer1(player))
                {
                    if (CurrentDamageIsHigherThanPrevious(match.PlayerOneDamage, player.HighestDamage))
                    {
                        player.HighestDamage = match.PlayerOneDamage;
                    }

                    if (CurrentDamageIsLowerThanPreviousOrPreviousIsZero(match.PlayerOneDamage, player.LowestDamage))
                    {
                        player.LowestDamage = match.PlayerOneDamage;
                    }
                }

                else
                {
                    if (CurrentDamageIsHigherThanPrevious(match.PlayerTwoDamage, player.HighestDamage))
                    {
                        player.HighestDamage = match.PlayerTwoDamage;
                    }

                    if (CurrentDamageIsLowerThanPreviousOrPreviousIsZero(match.PlayerTwoDamage, player.LowestDamage))
                    {
                        player.LowestDamage = match.PlayerTwoDamage;
                    }
                }
                linq.UpdateDatabaseRecord(player);
            }
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
                    int playerId = linq.GetPlayerIdByName(playerBox.Name);

                    foreach (Deck deck in linq.GetDecksList(playerId))
                    {
                        //types not currently used - will use when I figure out how to display type images in listbox
                        List<Type> types = linq.GetListOfTypesByDeckId(deck.DeckID);
                        deckBox.Items.Add(deck.Name);
                    }
                }
            }
        }

        public bool CurrentPlayerIsPlayer1(Player player)
        {
            return player.Name == PlayerOneSelect.Text;
        }

        public bool CurrentDamageIsHigherThanPrevious(int currentDamage, int previousDamage)
        {
            return currentDamage > previousDamage;
        }

        public bool CurrentDamageIsLowerThanPreviousOrPreviousIsZero(int currentDamage, int previousDamage)
        {
            return currentDamage < previousDamage || currentDamage == 0;
        }

        private void AddDefaultPlayerListBoxItems(string item)
        {
            PlayerOneSelect.Items.Add(item);
            PlayerTwoSelect.Items.Add(item);
        }

        private void PopulatePlayerListBoxes()
        {
            foreach (Player player in linq.GetPlayers())
            {
                PlayerOneSelect.Items.Add(player.Name);
                PlayerTwoSelect.Items.Add(player.Name);
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

            linq.SaveNewMatch(match);
            UpdatePlayer(PlayerOneSelect.Text, match);
            UpdatePlayer(PlayerTwoSelect.Text, match);
            UpdateDeck(PlayerOneDeckSelect.Text, match);
            UpdateDeck(PlayerTwoDeckSelect.Text, match);
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
