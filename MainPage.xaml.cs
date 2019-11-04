using Windows.Foundation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;

namespace LightsOut_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        LightsOutGame game;
        public MainPage()
        {
            this.InitializeComponent();

            // Enable cache
            NavigationCacheMode = NavigationCacheMode.Enabled;

            game = new LightsOutGame();
            game.NewGame();
            CreateGrid();
        }

        private void CreateGrid()
        {
            // Remove all previously-existing rectangles
            CanvasBoard.Children.Clear();
            int rectSize = (int)CanvasBoard.Width / game.GridSize;

            // Turn entire grid on and create rectangles to represent it
            for (int r = 0; r < game.GridSize; r++)
            {
                for (int c = 0; c < game.GridSize; c++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    rect.Width = rectSize + 1;
                    rect.Height = rect.Width + 1;
                    rect.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);

                    // Store each row and col as a Point
                    rect.Tag = new Point(r, c);
                    rect.PointerPressed += Rect_PointerPressed;

                    // Position rectangle
                    int x = c * rectSize;
                    int y = r * rectSize;
                    Canvas.SetTop(rect, y);
                    Canvas.SetLeft(rect, x);

                    // Add the new rectangle to the canvas' children
                    CanvasBoard.Children.Add(rect);
                }
            }

            DrawGrid();
        }

        private void DrawGrid()
        {
            int index = 0;

            // Set the colors of the rectangles
            for (int r = 0; r < game.GridSize; r++)
            {
                for (int c = 0; c < game.GridSize; c++)
                {
                    Rectangle rect = CanvasBoard.Children[index] as Rectangle;
                    index++;

                    if (game.GetGridValue(r, c))
                    {
                        // On
                        rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                        rect.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                    }
                    else
                    {
                        // Off
                        rect.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                        rect.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
                    }
                }
            }
        }

        private void Rect_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get row and column from Rectangle's Tag
            Rectangle rect = sender as Rectangle;

            if (rect != null)
            {
                var rowCol = (Point)rect.Tag;
                int row = (int)rowCol.X;
                int col = (int)rowCol.Y;
                game.Move(row, col);
            }
           
            // Redraw the board
            DrawGrid();
            NotifyVictory();
        }

        private async void NotifyVictory()
        {
            if (game.IsGameOver())
            {
                MessageDialog messageDialog = new MessageDialog("Congratulations!  You've won!", "Lights Out!");
                // Add an OK button
                messageDialog.Commands.Add(new UICommand("OK"));

                // Show the message box and wait for a button press
                await messageDialog.ShowAsync();
            }
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            game.NewGame();
            DrawGrid();
        }

    }
}
