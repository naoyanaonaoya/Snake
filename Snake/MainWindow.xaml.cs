using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Dictionary<GridValue, ImageSource> gridValueToImage = new()
    {
        {GridValue.Empty, Images.Empty },
        {GridValue.Snake, Images.Body },
        {GridValue.Food, Images.Food }
    };

    private readonly int _rows = 15, _cols = 15;

    private readonly Image[,] gridImages;

    private GameState _gameState;

    private bool _gameRunning;

    public MainWindow()
    {
        InitializeComponent();
        gridImages = SetupGrid();
        _gameState = new GameState(_rows, _cols);
    }

    private async Task RunGame()
    {
        Draw();
        Overlay.Visibility = Visibility.Hidden;
        await GameLoop();
    }

    private async Task GameLoop()
    {
        while (!_gameState.GameOver)
        {
            await Task.Delay(100);
            _gameState.Move();
            Draw();
        }
    }

    private async void Window_PreviewKeydown(object sender, KeyEventArgs e)
    {
        if (Overlay.Visibility == Visibility.Visible)
        {
            e.Handled = true;
        }

        if (!_gameRunning)
        {
            _gameRunning = true;
            await RunGame();
            _gameRunning = false;
        }
    }

    private void Window_Keydown(object sender, KeyEventArgs e)
    {
        if (_gameState.GameOver)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
                _gameState.ChangeDirection(Direction.Left);
                break;
            case Key.Right:
                _gameState.ChangeDirection(Direction.Right);
                break;
            case Key.Up:
                _gameState.ChangeDirection(Direction.Up);
                break;
            case Key.Down:
                _gameState.ChangeDirection(Direction.Down);
                break;
        }
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

    private void Draw()
    {
        DrawGrid();
        ScoreText.Text = $"SCORE {_gameState.Score}";
    }

    private void DrawGrid()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                GridValue gridValue = _gameState.Grid[row, col];
                gridImages[row, col].Source = gridValueToImage[gridValue];
            }
        }
    }
}