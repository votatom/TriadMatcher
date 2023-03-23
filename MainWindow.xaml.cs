using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        /// Initializes window, sets main grids height and width
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
            SetGrids();
            SetMainMenu();
            MakeLevelSelectButtons();
            ImageBrush mainMenuBack = GetBackground("MainMenuBackground.png");
            mainMenuBack.Stretch = Stretch.UniformToFill;
            MainCanvas.Background = mainMenuBack;
            LogoCanvas.Background = GetObjectImage("Logo.png");
        }

        public void SetPlayButton(int max)
        {
            PlayBut.Background = GetObjectImage("ButtonPlay.png");
            ((ImageBrush)PlayBut.Background).Stretch = Stretch.Uniform;
            PlayBut.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            PlayBut.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            PlayBut.MouseDown += new MouseButtonEventHandler(this.LevelSelect);
            PlayBut.MaxWidth = max;
            PlayBut.MaxHeight = max;
            PlayBut.Cursor = Cursors.Hand;
        }

        public void SetExitButton(Canvas canvas, int max)
        {
            canvas.Background = GetObjectImage("ButtonExit.png");
            ((ImageBrush)PlayBut.Background).Stretch = Stretch.Uniform;
            canvas.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            canvas.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            canvas.MouseDown += new MouseButtonEventHandler(this.Exit);
            canvas.MaxWidth = max;
            canvas.MaxHeight = max;
            canvas.Cursor = Cursors.Hand;
        }

        public void SetMainMenu()
        {
            int max = 150;
            this.SetPlayButton(max);
            this.SetExitButton(ExitButMain, max - 50);
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
            RenderOptions.SetBitmapScalingMode(ib, BitmapScalingMode.Fant);
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
            MainCanvas.Background = GetBackground("MainMenuBackground.png");
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
            ExitLevelEntrance(sender, e);
            Canvas button = (Canvas)sender;
            LevelSelectorGrid.Visibility = Visibility.Hidden;
            int levelId = Int32.Parse(button.Name.Substring(5,1));
            Level level = SerializationUtility.DeserializeLevel(levelId);
            StartLevel(LevelGrid,level);
            LevelGrid.Visibility = Visibility.Visible;
        }

        private void ExitLevelEntrance(object sender, RoutedEventArgs e)
        {
            Canvas button = (Canvas)sender;
            Grid levelEntrance = (Grid)button.Parent;
            Canvas levelReq = (Canvas)levelEntrance.Parent;
            levelEntrance = (Grid)levelReq.Parent;
            levelReq = (Canvas)levelEntrance.Parent;
            LevelSelectorGrid.Children.Remove(levelReq);
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
            levelGrid.ColumnDefinitions[0].Width = new GridLength(this.Width / 3 + this.Width / 10);
            //levelGrid.ShowGridLines = true;
            Grid grid = new Grid();
            Canvas gameplanCanvas = new Canvas();
            Grid.SetRow(gameplanCanvas, 0);
            Grid.SetColumn(gameplanCanvas, 1);
            Grid.SetColumnSpan(gameplanCanvas, 2);
            Grid.SetRowSpan(gameplanCanvas, 3);
            grid.Width = this.Width*560/1280;
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
            canvas.Width = levelGrid.Width * 3 / 8;
            //canvas.Width = 300;

            canvas.Height = levelGrid.Height/1.3;
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 0);
            Grid.SetRowSpan(canvas, 3);
            level.CreateReqCanvas(ref canvas);
            levelGrid.Children.Add(canvas);
        }

        public void MakeLevelSelectButtons()
        {
            Thickness[] borderThicknesses =
            {
                new Thickness(0, 0, this.Width/16.5, 0),
                new Thickness(0, this.Height/8, this.Width/12, 0),
                new Thickness(this.Width/7, this.Height/9, 0, 0),
                new Thickness(this.Width/10, this.Height/20, 0, 0)
            };
            for(int i = 0; i < 4; i++)
            {
                Canvas button = this.MakeLevelBut(i+1);
                Border buttonBorder = new Border();
                buttonBorder.Child = button;
                buttonBorder.Margin = borderThicknesses[i];
                int row;
                if(i == 1 || i == 2)
                {
                    row = 0;
                }
                else
                {
                    row = 1;
                }
                Grid.SetRow(buttonBorder, row);
                Grid.SetColumn(buttonBorder, i);
                LevelSelectorGrid.Children.Add(buttonBorder);
            }
            int max = 100;
            SetExitButton(ExitButLevelSelect, max);
            Canvas mainMenuBut = this.MakeMainMenuBut();
            mainMenuBut.MaxWidth = max;
            mainMenuBut.MaxHeight = max;
            Grid.SetRow(mainMenuBut, 2);
            Grid.SetColumn(mainMenuBut, 1);
            LevelSelectorGrid.Children.Add(mainMenuBut);
        }

        public Canvas MakeLevelBut(int Id)
        {
            ImageBrush image = GetObjectImage("MainCharacter-Level" + Id + ".png");
            double height = image.ImageSource.Height;
            double width = image.ImageSource.Width;
            double[] AspectRatio = { width, height };
            Canvas button = new Canvas();
            button.Name = "level" + Id;
            button.Height = (this.Height/1.5) * AspectRatio[1] / (AspectRatio[0]+ AspectRatio[1]);
            button.Width = (this.Height/1.5) * AspectRatio[0] / (AspectRatio[0] + AspectRatio[1]);
            button.MouseDown += this.ShowLevelEntrance;
            button.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            button.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            button.Background = image;
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

        public void ShowLevelEntrance(object sender, EventArgs e)
        {
            Canvas button = ((Canvas)sender);
            int levelId = Int32.Parse(button.Name.Substring(5, 1));
            Canvas mainCanvas = this.MakeBlackBackground();
            Grid grid = MakeLevelEntranceGrid(levelId);
            grid.Width = mainCanvas.Width;
            grid.Height = mainCanvas.Height;
            mainCanvas.Children.Add(grid);
            LevelSelectorGrid.Children.Add(mainCanvas);

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
            canvas.Height = this.Height - this.Height / 6;
            canvas.Background = GetBackground("GUI-LevelDone"+this.game.Level.Id+".png");
            canvas.Visibility = Visibility.Visible;
            Canvas nextLevel;
            if(this.game.Level.Id == 4)
            {
                nextLevel = new Canvas();
                nextLevel.Background = GetObjectImage("ButtonNextUnavailable.png");
            }
            else
            {
                nextLevel = MakeNextLevelButton();
            }
            Grid buttons = MakeLevelNavigationButtons(canvas.Width, MakeMainMenuBut(), nextLevel, MakeLevelSelectBut());
            buttons.Width = canvas.Width;
            buttons.Height = canvas.Height;
            canvas.Children.Add(buttons);
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 1);
            Canvas currentWitch = new Canvas();
            currentWitch.Background = GetObjectImage("AfterLevel" + this.game.Level.Id + ".png");
            currentWitch.MaxWidth = canvas.Width - (canvas.Width*2/17); 
            Grid.SetRow(currentWitch, 0);
            Grid.SetColumn(currentWitch, 0);
            grid.Children.Add(canvas);
            grid.Children.Add(currentWitch);
            return grid;
        }

        public Grid MakeLevelEntranceGrid(int levelId)
        {
            Canvas canvas = new Canvas();
            canvas.Width = this.Width / 3;
            canvas.Height = this.Height - this.Height / 6;
            canvas.Background = GetBackground("GUI-entryLevel"+levelId+".png");
            canvas.Visibility = Visibility.Visible;
            Grid buttons = MakeLevelNavigationButtons(canvas.Width, MakeMainMenuLevelEntranceBut(), MakePlayLevelBut(levelId), MakeExitLevelEntranceBut());
            buttons.Width = canvas.Width;
            buttons.Height = canvas.Height;
            canvas.Children.Add(buttons);
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(canvas, 0);
            Grid.SetColumn(canvas, 0);
            grid.Children.Add(canvas);
            return grid;
        }

        public Grid MakeLevelNavigationButtons(double canvasWidth, Canvas button1, Canvas button2, Canvas button3)
        {
            double max = canvasWidth / 5;
            Grid grid = new Grid();
            //grid.ShowGridLines = true;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions[0].Width = new GridLength(canvasWidth /10);
            grid.ColumnDefinitions[4].Width = new GridLength(canvasWidth / 10);
            
            button1.Width = max;
            button1.Height = max;
            Grid.SetColumn(button1, 1);
            Grid.SetRow(button1, 3);
            grid.Children.Add(button1);

            button2.Width = max+max/5;
            button2.Height = max+max / 5;
            Grid.SetColumn(button2, 2);
            Grid.SetRow(button2, 3);
            grid.Children.Add(button2);

            button3.Width = max;
            button3.Height = max;
            Grid.SetColumn(button3, 3);
            Grid.SetRow(button3, 3);
            grid.Children.Add(button3);


            return grid;
        }
        

        private void ReturnToMainMenuEntranceBut(object sender, RoutedEventArgs e)
        {
            this.ExitLevelEntrance(sender, e);
            MainMenuVisible(LevelSelectorGrid);
        }

        public Canvas MakeMainMenuLevelEntranceBut()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.ReturnToMainMenuEntranceBut);
            but.Background = GetObjectImage("ButtonHome.png");
            but.Cursor = Cursors.Hand;
            return but;
        }
        public Canvas MakeExitLevelEntranceBut()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.ExitLevelEntrance);
            but.Background = GetObjectImage("ButtonLevels.png");
            but.Cursor = Cursors.Hand;
            return but;
        }

        public Canvas MakePlayLevelBut(int levelId)
        {
            Canvas but = new Canvas();
            but.Name = "level" + levelId;
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.StartLevelHandler);
            but.Background = GetObjectImage("ButtonPlay.png");
            but.Cursor = Cursors.Hand;
            return but;
        }

        public Canvas MakeMainMenuBut()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.ReturnToMainMenu);
            but.Background = GetObjectImage("ButtonHome.png");
            but.Cursor = Cursors.Hand;
            return but;
        }

        public Canvas MakeLevelSelectBut()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.LevelSelect);
            but.Background = GetObjectImage("ButtonLevels.png");
            but.Cursor = Cursors.Hand;
            return but;
        }

        public Canvas MakeRestartButton()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.Restart);
            but.Background = GetObjectImage("ButtonReset.png");
            but.Cursor = Cursors.Hand;
            return but;
        }

        public Canvas MakeNextLevelButton()
        {
            Canvas but = new Canvas();
            but.MouseEnter += new MouseEventHandler(GameObject.MouseInCanvas);
            but.MouseLeave += new MouseEventHandler(GameObject.MouseOutOfCanvas);
            but.MouseDown += new MouseButtonEventHandler(this.StartNextLevel);
            but.Background = GetObjectImage("ButtonNext.png");
            but.Cursor = Cursors.Hand;
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
