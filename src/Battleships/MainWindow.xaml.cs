using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        // Window constructor
        public MainWindow()
        {
            InitializeComponent();
            PlayingField = new PlayingFieldViewModel();
            PlayingField.PropertyChanged += OnFieldChange;
            DataContext = PlayingField;
        }


        public void OnFieldChange(object obj, PropertyChangedEventArgs args)
        {
            if((args.PropertyName == "Field" || args.PropertyName == "ShowResult" || args.PropertyName == "EditMode") 
                && PlayingField.Field != null)
            {
                if (PlayingField.EditMode)
                {
                    GenerateEditPlayingField();
                }
                else
                {
                    GeneratePlayingField(PlayingField.ShowResult);
                }
            }
        }


        /// <summary>
        /// On click function for the grid buttons
        /// </summary>
        /// <param name="position">Position in the UniformGrid</param>
        /// <param name="sender">Button that called the function</param>
        private void FieldBtn_Click(Position position, object sender)
        {
            // Checks if delete mode is active and if the playing field is in editor mode
            if (PlayingField.DeleteState == true && PlayingField.EditMode)
            {
                // Deletes the boat on the clicked buttons position
                PlayingField.Field.DeleteBoat(position);
                GenerateEditPlayingField();
                _btnToggleOn = false;
            }
            else if (!PlayingField.EditMode) // If playing field is not in edit mode
            {
                BtnGameClick((Button)sender, position);
            }
            else
            {
                Button button = (Button)sender;
                BtnCreateBoat(button, position);
            }
        }


        /// <summary>
        /// Button click method for when the game is not in edit mode
        /// </summary>
        /// <param name="button"></param>
        /// <param name="position"></param>
        private void BtnGameClick(Button button, Position position)
        {
            // If button was already clicked
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


        /// <summary>
        /// Button click method for creating a boat in edit mode
        /// </summary>
        /// <param name="button"></param>
        /// <param name="position"></param>
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


        /// <summary>
        /// Method that generates the playing field in edit mode
        /// </summary>
        private void GenerateEditPlayingField()
        {
            PlayingFieldGrid.Children.Clear(); // Clearing previous buttons

            int fieldSize = PlayingField.Field.SideLength;
            
            PlayingFieldGrid.Columns = fieldSize;
            PlayingFieldGrid.Rows = fieldSize;

            // Loop to add X and Y positions of grid
            for (sbyte y = 0; y < fieldSize; y++)
            {
                for (sbyte x = 0; x < fieldSize; x++)
                {
                    // New button is created and added to the grid
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

                    PlayingFieldGrid.Children.Add(button); // Adds button to grid
                    button.Click += (s, e) => FieldBtn_Click(pos, s); // Assigns click function to button
                }
            }
        }


        /// <summary>
        /// Generates playing field for when it is not in edit mode
        /// </summary>
        /// <param name="showResult">Shows results to game when set to true</param>
        private void GeneratePlayingField(bool showResult = false)
        {
            int fieldSize = PlayingField.Field.SideLength;
            PlayingFieldGrid.Children.Clear();

            // 1 is added to the grid to display BoatBit counts
            PlayingFieldGrid.Columns = fieldSize + 1;
            PlayingFieldGrid.Rows = fieldSize + 1;

            // Loop to add X and Y positions of grid
            for (sbyte y = 0; y < fieldSize; y++)
            {
                for (sbyte x = 0; x < fieldSize; x++)
                {
                    // New button is created and added to the grid
                    Button button = new Button
                    {
                        Background = Brushes.Blue,
                        Height = Width,
                        Margin = new Thickness(1)
                    };

                    
                    Position pos = new Position(x, y);

                    // Checks if result should be shown
                    if (showResult)
                    {
                        ShowBtnResult(button, pos);
                    }
                    else
                    {
                        if (PlayingField.Field.Boats.Any(x => x.WasFound && x.BoatBits.Any(y => y.XYPosition == pos)))
                        {
                            button.Background = Brushes.Black;
                            PlayingField.SelectedPositions.Add(pos);
                        }
                        button.Click += (s, e) => FieldBtn_Click(pos, s); // Click will only work when results aren't shown
                    }

                    PlayingFieldGrid.Children.Add(button); // Button is added to grid
                }

                // Adds TextBlock showing count of BoatBits in row
                TextBlock textBlock = new TextBlock
                {
                    Text = PlayingField.Field.YBoatCount(y).ToString(),
                    TextAlignment = TextAlignment.Center
                };
                PlayingFieldGrid.Children.Add(textBlock);
            }

            // Counts of BoatBits are added in the last row
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
            ShowResultBtn.Visibility = Visibility.Hidden;
        }


        // "Generate" button click function
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            PlayingField.Field.GenerateBoats();
            EnablePlayMode();
        }


        /// <summary>
        /// Enables playing mode/disables editor mode
        /// </summary>
        private void EnablePlayMode()
        {
            PlayingField.SelectedPositions.Clear();

            _btnToggleOn = false;
            EditorGrid.Visibility = Visibility.Hidden;
            ShowResultBtn.Visibility = Visibility.Visible;
            GeneratePlayingField();
        }


        /// <summary>
        /// Enables editor mode/disables playing mode
        /// </summary>
        private void EnableEditorMode()
        {
            PlayingField.SelectedPositions.Clear();

            EditorGrid.Visibility = Visibility.Visible;
            ShowResultBtn.Visibility = Visibility.Hidden;
            GenerateEditPlayingField();
        }
    }
}
