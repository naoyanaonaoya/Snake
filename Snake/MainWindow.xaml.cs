﻿using System.Windows;
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

    private readonly Dictionary<Direction, int> _dirToRotation = new()
    {
        { Direction.Up, 0 },
        { Direction.Right, 90 },
        { Direction.Down, 180 },
        { Direction.Left, 270 }
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
        await ShowCountDown();
        Overlay.Visibility = Visibility.Hidden;
        await GameLoop();
        await ShowGameOver();
        _gameState = new GameState(_rows, _cols);
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
                    Source = Images.Empty,
                    RenderTransformOrigin = new Point(0.5, 0.5)
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
        DrawSnakeHead();
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
                gridImages[row, col].RenderTransform = Transform.Identity;
            }
        }
    }

    private void DrawSnakeHead()
    {
        Position headPos = _gameState.HeadPosition();
        Image headImage = gridImages[headPos.Row, headPos.Col];
        headImage.Source = Images.Head;

        int rotation = _dirToRotation[_gameState.Dir];
        headImage.RenderTransform = new RotateTransform(rotation);
    }

    private async Task DrawDeadSnake()
    {
        List<Position> snakePosistions = new List<Position>(_gameState.SnakePositions());

        for (int i = 0; i < snakePosistions.Count; i++)
        {
            Position pos = snakePosistions[i];
            ImageSource imageSource = (i == 0) ? Images.DeadHead : Images.DeadBody;
            gridImages[pos.Row, pos.Col].Source = imageSource;
            await Task.Delay(50);
        }
    }

    private async Task ShowCountDown()
    {
        for (int i = 3; i >= 1; i--)
        {
            OverlayText.Text = i.ToString();
            await Task.Delay(500);
        }
    }

    private async Task ShowGameOver()
    {
        await DrawDeadSnake();
        await Task.Delay(50);
        Overlay.Visibility = Visibility.Visible;
        OverlayText.Text = "PRESS ANY KEY TO START";
    }
}