using System.Windows;
using System.Windows.Controls;

namespace Snake;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly int _rows = 15, _cols = 15;

    private readonly Image[,] gridImages;

    public MainWindow()
    {
        InitializeComponent();
        gridImages = SetupGrid();
    }

    private Image[,] SetupGrid()
    {
        Image[,] images = new Image[_rows, _cols];
        GameGrid.Rows = _rows;
        GameGrid.Columns = _cols;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                Image image = new Image
                {
                    Source = Images.Empty
                };

                images[row, col] = image;
                GameGrid.Children.Add(image);
            }
        }

        return images;
    }
}