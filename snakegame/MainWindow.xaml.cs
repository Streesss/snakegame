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
using System.Windows.Threading;

namespace snakegame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValtoImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.up, 0 },
            { Direction.right, 90 },
            { Direction.down, 180 },
            { Direction.left, 270 },
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] GridImages;
        private GameState gameState;
        private bool gameRunning;
        private List<int> highscores = new();
        private int BoostSpeed = 0;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            GridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Hidden)
            {
                e.Handled = true;
            }

            if (gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.down);
                    break;
                case Key.Space:
                    BoostSpeed = (BoostSpeed == 0) ? GameSettings.BoostSpeed : 0;
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100 - BoostSpeed);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE: {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c <= cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    GridImages[r, c].Source = gridValtoImage[gridVal];
                    GridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private async Task ShowGameOver()
        {
            GameSettings.BoostSpeed = 0;
            ShakeWindow(GameSettings.ShakeDuration);
            // Audio.GameOver.Play();
            MediaPlayer sou = new();
            //sou.Open(new Uri($"Assets/game-over.wav", UriKind.Relative));
            //sou.Volume = 1;
            //sou.Play();
            
            await DrawDeadSnake();
            await Task.Delay(1000);
            OverlayText.Text = "PRESS ANY KEY TO START";
            UpdateLead();
            OverlayText.Visibility = Visibility.Visible;
        }

        private void UpdateLead()
        {
            highscores.Add(gameState.Score);
            highscores.Sort();
            highscores.Reverse();

            if (highscores.Count > 5)
            {
                highscores.RemoveAt(5);
            }
            foreach (var score in highscores)
            {
                OverlayText.Text += $"\n{score}";
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = GridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.snakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                GridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i++)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShakeWindow(int durationms)
        {
            var oLeft = this.Left;
            var oTop = this.Top;
            var shaketimer = new DispatcherTimer(DispatcherPriority.Send);

            shaketimer.Tick += async (sender, EventArgs) =>
            {
                this.Left = oLeft + random.Next(-10, 11);
                this.Top = oTop + random.Next(10, 11);

                shaketimer.Interval = TimeSpan.FromMilliseconds(200);
                shaketimer.Start();

                await Task.Delay(durationms);
                shaketimer.Stop();
            };
        }



    }
}
