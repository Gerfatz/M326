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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Battleships.ViewModels;
using BusinessLayer;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlayingFieldViewModel PlayingField { get; set; }
        private bool _btnToggleOn = false;
        private Position _btnPos;

        public MainWindow()
        {
            InitializeComponent();
            PlayingField = new PlayingFieldViewModel();
            FieldSizeBox.Text = "5";
            PlayingField.Field = new Field(5);
            GeneratePlayingField();
        }

        private void FieldSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sizeText = FieldSizeBox.Text;
            int numOut;
            if (int.TryParse(sizeText, out numOut) && numOut <= 30)
            {
                PlayingField.Field = new Field(numOut);

                FieldSizeBox.Foreground = Brushes.Black;
                GeneratePlayingField();
                GC.Collect();
            }
            else
            {
                FieldSizeBox.Foreground = Brushes.Red;
            }
        }

        private void FieldBtn_Click(Position position, Field field, object sender, RoutedEventArgs e)
        {
            
            Button button = (Button)sender;
            if (_btnToggleOn)
            {
                _btnToggleOn = false;
                button.Background = Brushes.Black;

                PlayingField.Field.CreateBoat(_btnPos, position);
                GeneratePlayingField();
            }
            else
            {
                _btnToggleOn = true;
                _btnPos = position;
                button.Background = Brushes.Black;
            }
        }
        
        private void GeneratePlayingField()
        {
            int fieldSize = PlayingField.Field.SideLength;

            PlayingFieldGrid.Children.Clear();
            PlayingFieldGrid.Columns = fieldSize;
            PlayingFieldGrid.Rows = fieldSize;

            int fieldAmnt = fieldSize * fieldSize;

            for (sbyte y = 0; y < fieldSize; y++)
            {
                for (sbyte x = 0; x < fieldSize; x++)
                {
                    Button button = new Button
                    {
                        Background = Brushes.Blue,
                        Content = "i",
                        Height = Width,
                        Margin = new Thickness(1)
                    };


                    Position pos = new Position(x, y);
                    if (PlayingField.Field.Boats.Any(x => x.BoatBits.Any(x => x.XYPosition == pos)))
                    {
                        button.Background = Brushes.Black;
                    }

                    PlayingFieldGrid.Children.Add(button);
                    button.Click += (s, e) => FieldBtn_Click(pos, PlayingField.Field, s, e);
                }
            }
        }
    }
}
