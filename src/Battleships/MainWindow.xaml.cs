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
            FieldSizeBox.Text = "7";
            PlayingField.Field = new Field(7);
            DataContext = PlayingField;
            GenerateEditPlayingField();
        }



        private void FieldSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sizeText = FieldSizeBox.Text;
            int numOut;
            if (int.TryParse(sizeText, out numOut) && numOut <= 20 && numOut >= 5)
            {
                PlayingField.Field = new Field(numOut);

                FieldSizeBox.Foreground = Brushes.Black;
                PlayingField.Field.Boats.Clear();
                PlayingField.SelectedPositions.Clear();
                GenerateEditPlayingField();
                GC.Collect();
            }
            else
            {
                FieldSizeBox.Foreground = Brushes.Red;
            }
        }



        private void FieldBtn_Click(Position position, object sender, RoutedEventArgs e)
        {
            if (PlayingField.ButtonState == true && PlayingField.EditorMode)
            {
                PlayingField.Field.DeleteBoat(position);
                GenerateEditPlayingField();
                _btnToggleOn = false;
            }
            else if (!PlayingField.EditorMode)
            {
                BtnGameClick((Button)sender, position);
            }
            else
            {
                Button button = (Button)sender;
                BtnCreateBoat(button, position);
            }
        }



        private void BtnGameClick(Button button, Position position)
        {
            if (button.Background == Brushes.Black)
            {
                button.Background = Brushes.Blue;
                PlayingField.SelectedPositions.Remove(position);
            }
            else
            {
                button.Background = Brushes.Black;
                PlayingField.SelectedPositions.Add(position);
            }
        }



        private void BtnCreateBoat(Button button, Position position)
        {
            if (_btnToggleOn)
            {
                _btnToggleOn = false;
                button.Background = Brushes.Black;

                PlayingField.Field.CreateBoat(_btnPos, position);
                GenerateEditPlayingField();
            }
            else
            {
                _btnToggleOn = true;
                _btnPos = position;
                button.Background = Brushes.Gray;
            }
        }


        
        private void GenerateEditPlayingField()
        {
            int fieldSize = PlayingField.Field.SideLength;

            PlayingFieldGrid.Children.Clear();
            PlayingFieldGrid.Columns = fieldSize;
            PlayingFieldGrid.Rows = fieldSize;

            for (sbyte y = 0; y < fieldSize; y++)
            {
                for (sbyte x = 0; x < fieldSize; x++)
                {
                        Button button = new Button
                        {
                            Background = Brushes.Blue,
                            Height = Width,
                            Margin = new Thickness(1)
                        };


                        Position pos = new Position(x, y);
                        if (PlayingField.Field.Boats.Any(x => x.BoatBits.Any(x => x.XYPosition == pos)))
                        {
                            button.Background = Brushes.Black;
                        }

                        PlayingFieldGrid.Children.Add(button);
                        button.Click += (s, e) => FieldBtn_Click(pos, s, e);
                }
            }
        }



        private void GeneratePlayingField(bool showResult = false)
        {
            int fieldSize = PlayingField.Field.SideLength;

            PlayingFieldGrid.Children.Clear();
            PlayingFieldGrid.Columns = fieldSize + 1;
            PlayingFieldGrid.Rows = fieldSize + 1;

            for (sbyte y = 0; y < fieldSize; y++)
            {
                for (sbyte x = 0; x < fieldSize; x++)
                {
                    Button button = new Button
                    {
                        Background = Brushes.Blue,
                        Height = Width,
                        Margin = new Thickness(1)
                    };
                    
                    Position pos = new Position(x, y);

                    if (showResult)
                    {
                        ShowBtnResult(button, pos);
                    }
                    else
                    {
                        if (PlayingField.Field.Boats.Any(x => x.WasFound && x.BoatBits.Any(y => y.XYPosition == pos)))
                        {
                            button.Background = Brushes.Black;
                        }
                        button.Click += (s, e) => FieldBtn_Click(pos, s, e);
                    }

                    PlayingFieldGrid.Children.Add(button);
                }

                TextBlock textBlock = new TextBlock
                {
                    Text = PlayingField.Field.YBoatCount(y).ToString(),
                    TextAlignment = TextAlignment.Center
                };
                PlayingFieldGrid.Children.Add(textBlock);
            }

            for (sbyte x = 0; x < fieldSize; x++)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = PlayingField.Field.XBoatCount(x).ToString(),
                    TextAlignment = TextAlignment.Center
                };
                PlayingFieldGrid.Children.Add(textBlock);
            }
        }


        // Button to toggle game from "edit" and "play" mode
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayingField.EditorMode == true)
            {
                CreateButton.Content = "Edit mode";
                PlayingField.EditorMode = false;
                _btnToggleOn = false;
                EditorGrid.Visibility = Visibility.Hidden;
                GeneratePlayingField();
            }
            else
            {
                CreateButton.Content = "Start game";
                PlayingField.EditorMode = true;
                EditorGrid.Visibility = Visibility.Visible;
                GenerateEditPlayingField();
            }

            PlayingField.SelectedPositions.Clear();
        }


        /// <summary>
        /// Function to color button according to the result
        /// </summary>
        /// <param name="button">Button, of whitch the result should be displayed</param>
        /// <param name="position">Buttons position on the field</param>
        /// <returns></returns>
        private bool ShowBtnResult(Button button, Position position)
        {
            // Sets color of button to black, if a ship was found on it's position
            foreach (BoatBit boatBit in PlayingField.Field.Boats.SelectMany(x => x.BoatBits))
            {
                if (boatBit.XYPosition == position)
                {
                    button.Background = Brushes.Black;
                }
            }

            // Sets buttons position to either green or red for the players guessed positions
            foreach (Position pos in PlayingField.SelectedPositions.Where(x => x == position))
            {
                // Checks if the guessed position exists in the boats list
                if (PlayingField.Field.Boats.SelectMany(x => x.BoatBits).Any(y => y.XYPosition == pos))
                {
                    // sets button color to green to indicate the answer was correct
                    button.Background = Brushes.Green;
                    return true;
                }
                else
                {
                    // sets button color to red to indicate the answer was incorrect
                    button.Background = Brushes.Red;
                    return true;
                }
            }

            return false;
        }


        // "Validate" button click function
        private void ShowResultBtn_Click(object sender, RoutedEventArgs e)
        {
            GeneratePlayingField(true);
        }
    }
}
