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
using Triad_Matcher.objects;
using Triad_Matcher.utility;

namespace Triad_Matcher
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game? game { get; set; }

        /// <summary>
        /// Initializes window, sets main grids height and width, and creates levels (only one use, deletes after)
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetGrids();
            SetPlayButton();
            //Main.MakeLevels();
            MakeLevelSelectButtons();
            ImageBrush mainMenuBack = GetBackground("MainMenuBackground-Crop.jpg");
            mainMenuBack.Stretch = Stretch.UniformToFill;
            MainCanvas.Background = mainMenuBack;
            LogoCanvas.Background = GetObjectImage("Logo.png");
        }

        public void SetPlayButton()
        {
            PlayBut.Background = GetObjectImage("ButtonPlay.png");
            ((ImageBrush)PlayBut.Background).Stretch = Stretch.Uniform;
            PlayBut.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            PlayBut.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            PlayBut.MouseDown += new MouseButtonEventHandler(this.LevelSelect);
            int max = 150;
            PlayBut.MaxWidth = max;
            PlayBut.MaxHeight = max;
            PlayBut.Cursor = Cursors.Hand;
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
            MainCanvas.Background = GetBackground("SelectLevelBackground.png");

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
        public static void GetGridBackground(int amount, ref Canvas canvas)
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
        public static ImageBrush GetObjectImage(string filename)
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
            object parent = ((Canvas)sender).Parent;
            while (parent.GetType() != typeof(Grid))
            {
                parent = ((Border)parent).Parent;
            }
            Grid grid = (Grid)parent;
            MainMenuVisible(grid);
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
            MainCanvas.Background = GetBackground("MainMenuBackground-Crop.jpg");
        }


        /// <summary>
        /// Starts specific level
        /// </summary>
        /// <param name="sender">
        /// What elements send a request
        /// </param>
        /// <param name="e">
        /// RoutedEventArgs
        /// </param>
        private void StartLevelHandler(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ((Grid)(button.Parent)).Visibility = Visibility.Hidden;
            int levelId = Int32.Parse(button.Name.Substring(5,1));
            Level level = SerializationUtility.DeserializeLevel(levelId);
            StartLevel(LevelGrid,level);
            LevelGrid.Visibility = Visibility.Visible;
        }

        private void StartNextLevel(object sender, RoutedEventArgs e)
        {
            Level level = SerializationUtility.DeserializeLevel(this.game.Level.Id + 1);
            StartLevel(LevelGrid, level);
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
            level.AddMainWindow(this);
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
            GetGridBackground(level.GamePlan.Count, ref gameplanCanvas);
            grid.Visibility = Visibility.Visible;
            //grid.ShowGridLines = true;
            Game thisgame = new Game(ref grid, this);
            thisgame.AddLevel(level);
            this.game = thisgame;
            level.CreateGridPlan(ref grid, ref thisgame);
            //grid.ShowGridLines = true;
            MainCanvas.Background = level.CreateBackground();
            gameplanCanvas.Children.Add(grid);
            gameplanCanvas.Width += 0;
            gameplanCanvas.Height += 5;
            levelGrid.Children.Add(gameplanCanvas);

            Canvas canvas = new Canvas();
            ImageBrush reqBack = GetBackground("GUI-inLevel" + level.Id + ".png");
            reqBack.Stretch = Stretch.Uniform;
            canvas.Background = reqBack;
            canvas.Width = 300;
            canvas.Height = 600;
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 0);
            Grid.SetRowSpan(canvas, 3);
            level.CreateReqCanvas(ref canvas);
            levelGrid.Children.Add(canvas);
        }

        public void MakeLevelSelectButtons()
        {
            for(int i = 0; i < 4; i++)
            {
                Button button = this.MakeLevelBut(i+1);
                Grid.SetRow(button, 1);
                Grid.SetColumn(button, i);
                LevelSelectorGrid.Children.Add(button);
            }   
        }

        public Button MakeLevelBut(int Id)
        {
            Button button = new Button();
            button.Name = "level" + Id;
            button.Click += StartLevelHandler;
            button.Height = 70;
            button.Width = 150;
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = 30;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.Text = "Level " + Id;
            button.Content = textBlock;
            return button;
        }

        public void ShowWinState(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            Canvas mainCanvas = this.MakeBlackBackground();
            Grid grid = MakeWinGrid();
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
            Grid buttons = MakeWinButtons();
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

        public Grid MakeWinButtons()
        {
            Grid grid = new Grid();
            //grid.ShowGridLines = true;
            int offset = 0;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < 3; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            Canvas button;
            if(this.game.Level.Id != 4)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                offset = 1;
                button = this.MakeNextLevelButton();
                Grid.SetColumn(button, 0);
                Grid.SetRow(button, 1);
                grid.Children.Add(button);

            }
            button = this.MakeLevelSelectBut();
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 1+offset);
            grid.Children.Add(button);

            button = this.MakeMainMenuBut();
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 2+offset);
            grid.Children.Add(button);

            return grid;
        }

        public Canvas MakeMainMenuBut()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.ReturnToMainMenu);
            but.Background = GetObjectImage("ButtonHome.png");
            return but;
        }

        public Canvas MakeLevelSelectBut()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.LevelSelect);
            but.Background = GetObjectImage("ButtonLevels.png");
            return but;
        }

        public Canvas MakeRestartButton()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.Restart);
            but.Background = GetObjectImage("ButtonReset.png");
            return but;
        }

        public Canvas MakeNextLevelButton()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.StartNextLevel);
            but.Background = GetObjectImage("ButtonNext.png");
            return but;
        }

        public void Restart(object sender, RoutedEventArgs a)
        {
            Level level = SerializationUtility.DeserializeLevel(this.game.Level.Id);
            StartLevel(LevelGrid, level);
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
