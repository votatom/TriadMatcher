using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Triad_Matcher.objects;

namespace Triad_Matcher
{
    public class Game
    {
        public Grid Grid { get; init; }
        public bool FirstChosen { get; set; }
        public Coordinates? FirstObject { get; set; }
        public Level? Level { get; private set; }
        public MainWindow MainWindow { get; init; }
        private bool Playable { get; set; }

        public Game(ref Grid grid, MainWindow mainWindow)
        {
            this.FirstChosen = false;
            this.FirstObject = null;
            this.Grid = grid;
            this.MainWindow = mainWindow;
            this.Resume();
        }

        public void AddLevel(Level level) 
        {
            this.Level = level;
        }

        public void Choose(object sender,EventArgs arg)
        {
            if (this.Playable)
            {
                if (!this.FirstChosen)
                {
                    if (sender.GetType() == typeof(Canvas))
                    {
                        this.FirstChosen = true;
                        this.FirstObject = new Coordinates { row = Grid.GetRow((Canvas)sender), col = Grid.GetColumn((Canvas)sender) };
                    }
                    else
                    {
                        throw new WrongWPFElementException("Oh yeah im not canvas");
                    }
                }
                else
                {
                    if (sender.GetType() == typeof(Canvas))
                    {
                        Coordinates second = new Coordinates { row = Grid.GetRow((Canvas)sender), col = Grid.GetColumn((Canvas)sender) };
                        if (second.IsSame(this.FirstObject))
                        {
                            this.FirstChosen = false;
                            this.FirstObject = null;
                            return;
                        }
                        this.Pause();
                        SwapEm(this.FirstObject, second);
                        this.FirstChosen = false;
                        this.FirstObject = null;
                        if (this.Level.IsWon())
                        {
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                            timer.Tick += new EventHandler(
                                this.MainWindow.ShowWinState
                            );
                            timer.Start();
                        }
                    }
                    else
                    {
                        throw new WrongWPFElementException("Oh yeah im not canvas");
                    }

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
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = new TimeSpan(0,0,0,0,750);
                    timer.Tick += delegate { 
                        timer.Stop(); 
                        Swap(first, second);
                        this.Resume();
                    };
                    timer.Start();
                }
                
                
            }
            else
            {
                this.Resume();
            }
        }

        public void Swap(Coordinates first, Coordinates second)
        {
            Canvas firstCan = GetCanvas(first.row, first.col);
            Canvas secondCan = GetCanvas(second.row, second.col);
            System.Windows.Controls.Grid.SetColumn(firstCan, second.col);
            System.Windows.Controls.Grid.SetRow(firstCan, second.row);
            System.Windows.Controls.Grid.SetColumn(secondCan, first.col);
            System.Windows.Controls.Grid.SetRow(secondCan, first.row);
            //Brush background = firstCan.Background;
            //firstCan.Background = secondCan.Background;
            //secondCan.Background = background;
        }

        

        private void MoveObjects(Dictionary<Coordinates, Coordinates> fromTo)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Tick += delegate { timer.Stop();
                foreach (Coordinates coord in fromTo.Keys)
                {
                    Canvas gameObject = GetCanvas(coord.row, coord.col);
                    if (gameObject != null)
                    {
                        Grid.SetRow(gameObject, fromTo[coord].row);
                        Grid.SetColumn(gameObject, fromTo[coord].col);
                    }

                }
                this.Resume();
            };
            timer.Start();
        }
        private void DeleteObjectCanvases()
        {
            List<List<GameObject>> gameplan = this.Level.GamePlan;
            List<Canvas> toDestroy = new List<Canvas>();
            for(int x = 0; x < gameplan.Count; x++)
            {
                for(int y = 0; y < gameplan[x].Count; y++)
                {
                    if (gameplan[x][y] == null)
                    {
                        Canvas canvas = this.GetCanvas(x, y);
                        if(canvas != null)
                        {
                            toDestroy.Add(canvas);
                        }
                    }
                }
            }
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 75);
            timer.Tick += delegate
            {
                timer.Stop();
                foreach(Canvas canvas in toDestroy)
                {
                    this.Grid.Children.Remove(canvas);
                }
                this.Level.FillItem();
            };
            timer.Start();
        }

        public void Pause()
        {
            this.Playable = false;
        }

        public void Resume()
        {
            this.Playable = true;
        }
    }
}
