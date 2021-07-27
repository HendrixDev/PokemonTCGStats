using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for NewPlayer.xaml
    /// </summary>
    public partial class NewPlayer : Window
    {
        public NewPlayer()
        {
            InitializeComponent();
            //NewPlayerNameBox.Focus();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPlayerNameBox.Text))
            {
                //Throw Error message
            }

            else
            {
                try
                {
                    Player player = new Player
                    {
                        Name = NewPlayerNameBox.Text,
                        TotalGamesPlayed = 0,
                        HighestDamage = 0,
                        LowestDamage = 0,
                        Wins = 0
                    };

                    using (PokemonEntities1 db = new PokemonEntities1())
                    {
                        db.Players.Add(player);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                this.Close();
            }
        }
    }
}
