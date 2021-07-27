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
    /// Interaction logic for NewDeck.xaml
    /// </summary>
    public partial class NewDeck : Window
    {
        public NewDeck()
        {
            InitializeComponent();

            using (PokemonEntities1 db = new PokemonEntities1())
            {
                List<Player> players = db.Players.ToList();

                foreach (Player player in players)
                {
                    PlayerSelect.Items.Add(player.Name);
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            using (PokemonEntities1 db = new PokemonEntities1())
            {
                int playerID = db.Players.Where(x => x.Name == PlayerSelect.Text).Select(x => x.PlayerID).FirstOrDefault();

                Deck deck = new Deck
                {
                    PlayerId = playerID,
                    Name = DeckNameTxt.Text,
                    HighestDamage = 0,
                    LowestDamage = 0,
                    TotalGamesPlayes = 0,
                    Wins = 0
                };

                var checkboxList = this.Deck.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

                db.Decks.Add(deck);

                foreach (var checkBox in checkboxList)
                {
                    Type type = new Type
                    {
                        TypeName = checkBox.Name,
                        DeckID = deck.DeckID
                    };
                    db.Types.Add(type);
                }

                db.SaveChanges();

                this.Close();
            }
        }
    }
}
