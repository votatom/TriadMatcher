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
using Triad_Matcher.objects;
using Triad_Matcher.utility;

namespace Triad_Matcher
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game game { get; set; }

        /// <summary>
        /// Initializes window, sets main grids height and width, and creates levels (only one use, deletes after)
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetGrids();
            //Main.MakeLevels();
            MainCanvas.Background = GetBackground("MainMenuBackground.png");
            LogoCanvas.Background = getObjectImage("Logo.png");
            SetPlayButton();
            //StartButton.Content = getObjectImage("PlayButton.png");
        }

        public void SetPlayButton()
        {
            PlayBut.Background = getObjectImage("PlayButton.png");
            ((ImageBrush)PlayBut.Background).Stretch = Stretch.Uniform;
            //PlayBut.MouseEnter += new MouseEventHandler(this.BiggerPlayBut);
            //PlayBut.MouseLeave += new MouseEventHandler(this.SmallerPlayBut);
            PlayBut.MouseDown += new MouseButtonEventHandler(this.LevelSelect);
            int max = 150;
            PlayBut.MaxWidth = max;
            PlayBut.MaxHeight = max;
            PlayBut.Cursor = Cursors.Hand;
        }

        private void BiggerPlayBut(Object sender, EventArgs e)
        {
            ImageBrush image = (ImageBrush)PlayBut.Background;
            PlayBut.Height = PlayBut.Height + 20;
            PlayBut.Width = PlayBut.Width + 20;
            PlayBut.Background = image;
            
        }

        private void SmallerPlayBut(Object sender, EventArgs e)
        {
            PlayBut.Height = PlayBut.Height - 20;
            PlayBut.Width = PlayBut.Width - 20;
        }

        /// <summary>
        /// Sets width and height for every main grid in application
        /// </summary>
        private void SetGrids()
        {
            foreach(Grid grid in Descendants<Grid>(MainCanvas)){
                grid.Width = MainStage.Width;
                grid.Height = MainStage.Height;
                //grid.ShowGridLines = true;
            }
        }

        /// <summary>
        /// Closes window
        /// </summary>
        /// <param name="sender">
        /// What elements send a request
        /// </param>
        /// <param name="e">
        /// RoutedEventArgs
        /// </param>
        private void Exit(object sender, RoutedEventArgs e)
        {
            MainStage.Close();
        }

        /// <summary>
        /// Changes canvas to LevelSelect canvas (grid)
        /// </summary>
        /// <param name="sender">
        /// What elements send a request
        /// </param>
        /// <param name="e">
        /// RoutedEventArgs
        /// </param>
        private void LevelSelect(object sender, EventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            LevelSelectorGrid.Visibility = Visibility.Visible;
            LevelGrid.Visibility = Visibility.Hidden;
            MainCanvas.Background = GetBackground("test_background.jpg");

        }

        /// <summary>
        /// Creates ImageBrush to be used in a canvas background image
        /// </summary>
        /// <param name="filename">
        /// Filename of image
        /// </param>
        /// <returns>
        /// ImagBrush
        /// </returns>
        public static ImageBrush GetBackground(string filename)
        {
            ImageBrush ib = new ImageBrush();
            string path = "../../../images/backgrounds/{0}";
            ib.ImageSource = new BitmapImage(new Uri(String.Format(path,filename), UriKind.Relative));
            ib.Stretch = Stretch.UniformToFill;
            return ib;
        }

        /// <summary>
        /// Sets background Image for Level Grid background (how many GameObjects are in Gameplan
        /// </summary>
        /// <param name="amount">
        /// Amount of GameObjects in one line
        /// </param>
        /// <param name="canvas">
        /// Canvas to be edited
        /// </param>
        public static void getGridBackground(int amount, ref Canvas canvas)
        {
            string path = "../../../images/grids/{0}";
            string file = amount + "x" + amount + ".png";
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(String.Format(path,file), UriKind.Relative));
            canvas.Background = ib;

        }

        /// <summary>
        /// Gets ImageBrush object of a specific objet with associated filename
        /// </summary>
        /// <param name="filename">
        /// Name of image file
        /// </param>
        /// <returns>
        /// ImageBrush
        /// </returns>
        public static ImageBrush getObjectImage(string filename)
        {
            ImageBrush ib = new ImageBrush();
            string path = "../../../images/objects/{0}";
            ib.ImageSource = new BitmapImage(new Uri(String.Format(path, filename), UriKind.Relative));
            ib.Stretch = Stretch.Uniform;
            return ib;
        }



        /// <summary>
        /// Changes Canvas to MainMenu canvas (grid)
        /// </summary>
        /// <param name="sender">
        /// What elements send a request
        /// </param>
        /// <param name="e">
        /// RoutedEventArgs
        /// </param>
        private void ReturnToMainMenu(object sender, RoutedEventArgs e)
        {
            MainMenuVisible((Grid)((Button)sender).Parent);
            LevelGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Sets visibility of main menu Grid to visible and a sender grid visibility to hidden
        /// </summary>
        /// <param name="grid">
        /// Grid to be hidden
        /// </param>
        private void MainMenuVisible(Grid grid)
        {
            grid.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
            MainCanvas.Background = GetBackground("MainMenuBackground.png");
        }


        /// <summary>
        /// Starts level 1
        /// </summary>
        /// <param name="sender">
        /// What elements send a request
        /// </param>
        /// <param name="e">
        /// RoutedEventArgs
        /// </param>
        private void StartLevel1(object sender, RoutedEventArgs e)
        {
            ((Grid)((Button)sender).Parent).Visibility = Visibility.Hidden;
            Level level = SerializationUtility.DeserializeLevel(1);
            StartLevel(LevelGrid,level);
            LevelGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Starts level 2
        /// </summary>
        /// <param name="sender">
        /// What elements send a request
        /// </param>
        /// <param name="e">
        /// RoutedEventArgs
        /// </param>
        private void StartLevel2(object sender, RoutedEventArgs e)
        {
            ((Grid)((Button)sender).Parent).Visibility = Visibility.Hidden;
            Level level = SerializationUtility.DeserializeLevel(2);
            StartLevel(LevelGrid, level);
            LevelGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Base function to starting levels gives main grid where are all the elements of one level shown in GUI
        /// </summary>
        /// <param name="levelGrid">
        /// Grid where Level is shown
        /// </param>
        /// <param name="level">
        /// Level to be loaded
        /// </param>
        private void StartLevel(Grid levelGrid, Level level)
        {
            levelGrid.Children.Clear();
            Grid grid = new Grid();
            Canvas gameplanCanvas = new Canvas();
            Grid.SetRow(gameplanCanvas, 0);
            Grid.SetColumn(gameplanCanvas, 1);
            Grid.SetColumnSpan(gameplanCanvas, 2);
            Grid.SetRowSpan(gameplanCanvas, 3);
            grid.Width = levelGrid.ColumnDefinitions[0].ActualWidth + 100;
            grid.Height = grid.Width;
            gameplanCanvas.Width = grid.Width;
            gameplanCanvas.Height = grid.Width;
            getGridBackground(level.GamePlan.Count, ref gameplanCanvas);
            grid.Visibility = Visibility.Visible;
            //grid.ShowGridLines = true;
            Game thisgame = new Game(ref grid, this);
            thisgame.AddLevel(level);
            this.game = thisgame;
            level.CreateGridPlan(ref grid, ref thisgame);
            MainCanvas.Background = level.CreateBackground();
            gameplanCanvas.Children.Add(grid);
            gameplanCanvas.Width += 0;
            gameplanCanvas.Height += 5;
            levelGrid.Children.Add(gameplanCanvas);

            Canvas canvas = new Canvas();
            canvas.Background = GetBackground("req_background.jpg");
            canvas.Width = 300;
            canvas.Height = 600;
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 0);
            Grid.SetRowSpan(canvas, 3);
            level.CreateReqCanvas(ref canvas);
            levelGrid.Children.Add(canvas);

            Button pause = new Button();
            TextBlock pauseText = new TextBlock();
            pauseText.Text = "Pause";
            pause.Content = pauseText;
            pause.Width = 150;
            pause.Height = 50;
            pause.VerticalAlignment = VerticalAlignment.Center;
            pause.HorizontalAlignment = HorizontalAlignment.Center;
            pause.Click += Pause;
            Grid.SetColumn(pause, 0);
            Grid.SetRow(pause, 0);
            levelGrid.Children.Add(pause);
        }

        /// <summary>
        /// Creates canvas using provided Uri for image
        /// </summary>
        /// <param name="uri">
        /// Where is image located
        /// </param>
        /// <returns>
        /// Canvas with set background image
        /// </returns>
        public static Canvas makeObjectImage(Uri uri)
        {
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(uri);
            Canvas canvas = new Canvas();
            canvas.Background = image;
            return canvas;
        }

        public void ShowWinState()
        {
            Canvas mainCanvas = this.MakeBlackBackground();
            Grid grid = MakeWinGrid();
            grid.Width = mainCanvas.Width;
            grid.Height = mainCanvas.Height;
            mainCanvas.Children.Add(grid);
            LevelGrid.Children.Add(mainCanvas);
        }

        public void Resume(object sender, RoutedEventArgs a)
        {
            DestroyBlackBackground();
        }

        public void Pause(object sender, RoutedEventArgs a)
        {
            ShowPauseState();
        }

        public void ShowPauseState()
        {
            Canvas mainCanvas = this.MakeBlackBackground();
            Grid grid = MakePauseGrid();
            grid.Width = mainCanvas.Width;
            grid.Height = mainCanvas.Height;
            mainCanvas.Children.Add(grid);
            LevelGrid.Children.Add(mainCanvas);
        }

        public Canvas MakeBlackBackground()
        {
            Canvas mainCanvas = new Canvas();
            mainCanvas.Width = this.Width;
            mainCanvas.Height = this.Height;
            mainCanvas.Visibility = Visibility.Visible;
            SolidColorBrush color = new SolidColorBrush(Colors.Black);
            color.Opacity = 0.75;
            mainCanvas.Background = color;
            return mainCanvas;
        }

        public Grid MakeWinGrid()
        {
            Canvas canvas = new Canvas();
            canvas.Width = this.Width / 3;
            canvas.Height = this.Height - this.Height / 5;
            canvas.Background = new SolidColorBrush(Colors.White);
            canvas.Visibility = Visibility.Visible;
            Grid buttons = MakePauseButtons();
            buttons.Width = canvas.Width;
            buttons.Height = canvas.Height;
            TextBlock text = new TextBlock();
            text.Text = "You win";
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.FontSize = 50;
            text.FontWeight = FontWeights.Bold;
            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 0);
            buttons.Children.Add(text);
            canvas.Children.Add(buttons);
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 0);
            grid.Children.Add(canvas);
            return grid;
        }

        public Grid MakePauseGrid()
        {
            Canvas canvas = new Canvas();
            canvas.Width = this.Width / 3;
            canvas.Height = this.Height - this.Height / 5;
            canvas.Background = new SolidColorBrush(Colors.White);
            canvas.Visibility = Visibility.Visible;
            Grid buttons = MakePauseButtons();
            buttons.Width = canvas.Width;
            buttons.Height = canvas.Height;


            Button button = new Button();
            button.Click += Resume;
            TextBlock text = new TextBlock();
            text.Text = "Resume";
            button.Content = text;
            button.Width = 150;
            button.Height = 50;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 0);
            buttons.Children.Add(button);

            canvas.Children.Add(buttons);
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 0);
            grid.Children.Add(canvas);
            return grid;
        }

        public Grid MakePauseButtons()
        {
            Grid grid = new Grid();
            //grid.ShowGridLines = true;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < 4; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            Button button = new Button();
            button.Content = new Button();
            button.Click += Restart;
            TextBlock text = new TextBlock();
            text.Text = "Restart";
            button.Content = text;
            button.Width = 150;
            button.Height = 50;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 1);
            grid.Children.Add(button);

            button = new Button();
            button.Click += LevelSelect;
            text = new TextBlock();
            text.Text = "Level Select";
            button.Content = text;
            button.Width = 150;
            button.Height = 50;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 2);
            grid.Children.Add(button);

            button = new Button();
            button.Click += ReturnToMainMenu;
            text = new TextBlock();
            text.TextWrapping = TextWrapping.Wrap;
            text.Text = "Return to main menu";
            button.Content = text;
            button.Width = 150;
            button.Height = 50;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 3);
            grid.Children.Add(button);

            return grid;
        }

        public void Restart(object sender, RoutedEventArgs a)
        {
            Level level = SerializationUtility.DeserializeLevel(this.game.Level.Id);
            StartLevel(LevelGrid, level);
        }

        public void DestroyBlackBackground()
        {
            LevelGrid.Children.RemoveAt(LevelGrid.Children.Count - 1);
        }
        
        /// <summary>
        /// Gives a list of all elements from main grid 
        /// </summary>
        /// <typeparam name="T">
        /// Type of object to list
        /// </typeparam>
        /// <param name="dependencyItem">
        /// Where should be objects be located
        /// </param>
        /// <returns>
        /// List of T object
        /// </returns>
        public static IEnumerable<T> Descendants<T>(DependencyObject dependencyItem) where T : DependencyObject
        {
            if (dependencyItem != null)
            {
                for (var index = 0; index < VisualTreeHelper.GetChildrenCount(dependencyItem); index++)
                {
                    var child = VisualTreeHelper.GetChild(dependencyItem, index);
                    if (child is T dependencyObject)
                    {
                        yield return dependencyObject;
                    }

                    foreach (var childOfChild in Descendants<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
