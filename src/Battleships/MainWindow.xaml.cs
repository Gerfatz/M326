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

namespace Battleships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _fieldSize = 5;

        public MainWindow()
        {
            InitializeComponent();
            FieldSizeBox.Text = "5";
            GeneratePlayingField();
        }

        private void FieldSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sizeText = FieldSizeBox.Text;
            if (int.TryParse(sizeText, out _fieldSize))
            {
                FieldSizeBox.Foreground = Brushes.Black;
                GeneratePlayingField();
            }
            else
            {
                FieldSizeBox.Foreground = Brushes.Red;
            }
        }
        
        private void GeneratePlayingField()
        {
            PlayingFieldGrid.Children.Clear();
            PlayingFieldGrid.Columns = _fieldSize;
            PlayingFieldGrid.Rows = _fieldSize;

            int fieldAmnt = _fieldSize * _fieldSize;

            for (int i = 0; i < fieldAmnt; i++)
            {
                Button button = new Button
                {
                    Background = Brushes.Black,
                    Content = "i",
                    Height = Width
                };
                PlayingFieldGrid.Children.Add(button);
            }
        }
    }
}
