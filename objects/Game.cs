using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Triad_Matcher.objects;

namespace Triad_Matcher
{
    public class Game
    {
        public Grid GamePlanGrid { get; init; }
        public bool FirstChosen { get; set; }
        public Coordinates? FirstObject { get; set; }
        public Level? Level { get; private set; }
        public MainWindow MainWindow { get; init; }
        private bool Playable { get; set; }

        private bool Cascade { get; set; }

        public Game(ref Grid grid, MainWindow mainWindow)
        {
            this.FirstChosen = false;
            this.FirstObject = null;
            this.GamePlanGrid = grid;
            this.MainWindow = mainWindow;
            this.Resume();
        }

        public void AddLevel(Level level) 
        {
            this.Level = level;
        }

        public async void Choose(object sender,EventArgs arg)
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
                        throw new WrongWPFElementException("Element is not canvas");
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
                            while (!this.Cascade)
                            {
                                await Task.Delay(100);
                            }
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
                            timer.Tick += new EventHandler(
                                this.MainWindow.ShowWinState
                            );
                            timer.Start();
                        }
                    }
                    else
                    {
                        throw new WrongWPFElementException("Element is not canvas");
                    }

                }
            }
            
        }

        private Canvas GetCanvas(int row, int col)
        {
            try
            {
                Canvas canvas = this.GamePlanGrid.Children.Cast<Canvas>().First(e =>
                                Grid.GetRow(e) == row &&
                                Grid.GetColumn(e) == col);
                return canvas;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async void SwapEm(Coordinates first, Coordinates second)
        {
            if (first.Compare(second))
            {
                Swap(first, second);
                if(this.Level.SwapEm(first, second))
                {
                    this.Cascade = false;
                    DeleteObjectCanvases();
                    List<List<Coordinates>> toDo = new List<List<Coordinates>>();
                    Dictionary<Coordinates, Coordinates> whatToMove = this.Level.WhatToMove();
                    MoveObjects(whatToMove);
                    toDo.Add(Game.ValuesToList(whatToMove));
                    int index = 0;
                    do
                    {
                        foreach(Coordinates e in toDo[index])
                        {
                            if (this.Level.Cascade(e))
                            {
                                while (!this.Cascade)
                                {
                                    await Task.Delay(25);
                                }
                                this.Cascade = false;
                                DispatcherTimer timer = new DispatcherTimer();
                                timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                                timer.Tick += delegate {
                                    timer.Stop();
                                    DeleteObjectCanvases();
                                    Dictionary<Coordinates, Coordinates> whatToMoveCascade = this.Level.WhatToMove();
                                    MoveObjects(whatToMoveCascade);
                                    toDo.Add(ValuesToList(whatToMoveCascade));
                                };
                                timer.Start();
                                
                            }
                        }
                        index++;
                    }while(index < toDo.Count);
                }
                else
                { 
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = new TimeSpan(0,0,0,0,500);
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

        private static List<Coordinates> ValuesToList(Dictionary<Coordinates,Coordinates> dictionary)
        {
            List<Coordinates> list = new List<Coordinates>();
            foreach(Coordinates e in dictionary.Values)
            {
                list.Add(e);
            }
            return list;
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
                this.Cascade = true;
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
                    this.GamePlanGrid.Children.Remove(canvas);
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
