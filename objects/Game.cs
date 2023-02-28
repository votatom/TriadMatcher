using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Triad_Matcher.objects;

namespace Triad_Matcher
{
    public class Game
    {
        public Grid Grid { get; init; }
        public bool FirstChosen { get; set; }
        public Coordinates FirstObject { get; set; }
        public Level Level { get; private set; }
        public MainWindow MainWindow { get; init; }

        public Game(ref Grid grid, MainWindow mainWindow)
        {
            this.FirstChosen = false;
            this.FirstObject = null;
            this.Grid = grid;
            this.MainWindow = mainWindow;
        }

        public void AddLevel(Level level) 
        {
            this.Level = level;
        }

        public void Choose(object sender,EventArgs arg)
        {
            if (!this.FirstChosen)
            {
                if(sender.GetType() == typeof(Canvas))
                {
                    this.FirstChosen = true;
                    this.FirstObject = new Coordinates { row = Grid.GetRow((Canvas)sender), col = Grid.GetColumn((Canvas)sender) };
                }
                else
                {
                    throw new Exception("Oh yeah im not canvas");
                }
            }
            else
            {
                if (sender.GetType() == typeof(Canvas))
                {
                    Coordinates second = new Coordinates { row = Grid.GetRow((Canvas)sender), col = Grid.GetColumn((Canvas)sender) };
                    SwapEm(this.FirstObject, second);
                    this.FirstChosen = false;
                    this.FirstObject = null;
                    if (this.Level.IsWon())
                    {
                        this.MainWindow.ShowWinState();
                    }
                }
                else
                {
                    throw new Exception("Oh yeah im not canvas");
                }
                
            }
        }

        private Canvas GetCanvas(int row, int col)
        {
            try
            {
                Canvas canvas = this.Grid.Children.Cast<Canvas>().First(e =>
                                Grid.GetRow(e) == row &&
                                Grid.GetColumn(e) == col);
                return canvas;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void SwapEm(Coordinates first, Coordinates second)
        {
            if (first.Compare(second))
            {
                Swap(first, second);
                if(this.Level.SwapEm(first, second))
                {
                    DeleteObjectCanvases();
                    MoveObjects(this.Level.WhatToMove());
                }
                else
                {
                    Swap(first, second);
                }
                
                
            }
        }

        private void Swap(Coordinates first, Coordinates second)
        {
            Canvas firstCan = GetCanvas(first.row, first.col);
            Canvas secondCan = GetCanvas(second.row, second.col);
            Brush background = firstCan.Background;
            firstCan.Background = secondCan.Background;
            secondCan.Background = background;
        }

        

        private void MoveObjects(Dictionary<Coordinates, Coordinates> fromTo)
        {
            foreach(Coordinates coord in fromTo.Keys)
            {
                Canvas gameObject = GetCanvas(coord.row, coord.col);
                if(gameObject != null)
                {
                    Grid.SetRow(gameObject, fromTo[coord].row);
                    Grid.SetColumn(gameObject, fromTo[coord].col);
                }
                
            }
        }

        private void DeleteObjectCanvases()
        {
            List<List<GameObject>> gameplan = this.Level.GamePlan;
            for(int x = 0; x < gameplan.Count; x++)
            {
                for(int y = 0; y < gameplan[x].Count; y++)
                {
                    if (gameplan[x][y] == null)
                    {
                        Canvas canvas = this.GetCanvas(x, y);
                        if(canvas != null)
                        {
                            this.Grid.Children.Remove(canvas);
                        }
                    }
                }
            }
        }
    }
}
