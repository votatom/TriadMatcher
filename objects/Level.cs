﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Triad_Matcher.objects
{
    /// <summary>
    /// Class Level
    /// Saves all the information it needs to generate level
    /// </summary>
    public class Level 
    {
        /// <summary>
        /// GamePlan
        /// Saves a big list of lists of GameObjects used in level in a specific order. Is used to generate grid for a level and to get requirements for a level.
        /// </summary>
        public List<List<GameObject>> GamePlan { get; init; }

        /// <summary>
        /// Background
        /// Saves information about what image to use during generating background for a grid.
        /// </summary>
        public String Background { get; init; }

        /// <summary>
        /// Id
        /// Identification for a level - is determined by static BaseId value or value that was written in a file of level.
        /// </summary>
        public int Id { get; init; }

        private int ToDestroy { get; init; }
        private int Destroyed { get; set; }
        private TextBlock DestroyedText { get; set; }

        /// <summary>
        /// BasePath
        /// Determines where are certain components of level (images) located, functions are used to point to an exact locations.
        /// </summary>
        public static String BasePath = "../../../images/{0}/{1}";

        /// <summary>
        /// BaseId
        /// Is used to generate an Id for a specific level.
        /// </summary>
        public static int BaseId = 1;

        /// <summary>
        /// Creates instance of class Level, without a specific id from a file.
        /// </summary>
        /// <param name="objectsInPlan">
        /// Array of instances of class GameObject.
        /// Square root of amount of GameObjects must be an integer not a double or float.
        /// The order of GameObjects is important. It defines property GamePlan.
        /// </param>
        /// <param name="background">
        /// Defines property Background.
        /// Filename of background image in format "[name].[jpg/png]"
        /// </param>
        public Level(GameObject[] objectsInPlan, string background)
        {
            this.GamePlan = MakeLevelPlan(objectsInPlan);
            this.Background = background;
            this.Id = BaseId;
            BaseId++;

        }

        /// <summary>
        /// Creates instance of class Level with a specific id froma file.
        /// </summary>
        /// <param name="objectsInPlan">
        /// Array of instances of class GameObject.
        /// Square root of amount of GameObjects must be an integer not a double or float.
        /// The order of GameObjects is important. It defines property GamePlan.
        /// </param>
        /// <param name="background">
        /// Defines property Background.
        /// Filename of background image in format "[name].[jpg/png]"
        /// </param>
        /// <param name="Id">
        /// Specific Id from a file.
        /// Defines property Id
        /// </param>
        public Level(GameObject[] objectsInPlan, string background, int Id)
        {
            this.GamePlan = MakeLevelPlan(objectsInPlan);
            this.Background = background;
            this.ToDestroy = objectsInPlan.Length;
            this.Destroyed = 0;
            this.Id = Id;
        }

        /// <summary>
        /// Used to create a dictionary of GameObject : integer where integer is amount of a specific GameObject in property Gameplan
        /// </summary>
        /// <returns>
        /// Dictionary of GameObjects : integers
        /// Dictionary<GameObject,int>
        /// </returns>
        public Dictionary<GameObject,int> MakeLevelReq()
        {
            Dictionary<GameObject, int> req = new Dictionary<GameObject, int>();
            foreach (List<GameObject> objects in this.GamePlan)
            {
                foreach (GameObject gameObject in objects)
                {
                    if (req.ContainsKey(gameObject))
                    {
                        req[gameObject] += 1;
                    }
                    else
                    {
                        req.Add(gameObject, 1);
                    }
                }
            }
            return req;
        }

        /// <summary>
        /// Used to create List of lists of GameObjects from array of GameObjects to create a representation of Level gameplan.
        /// </summary>
        /// <param name="objects">
        /// Array of GameObjects
        /// </param>
        /// <returns>
        /// List of lists of GameObjects
        /// List<List<GameObject>>
        /// </returns>
        private static List<List<GameObject>> MakeLevelPlan(GameObject[] objects)
        {
            int arrLenght = (int)Math.Sqrt(objects.Length);
            List<List<GameObject>> plan = new List<List<GameObject>>();
            List<GameObject> row = new List<GameObject>();
            foreach (GameObject obj in objects)
            {
                row.Add(obj);
                if (row.Count == (int)Math.Sqrt(objects.Length))
                {
                    plan.Add(row);
                    row = new List<GameObject>();
                }
            }
            return plan;
        }

        /// <summary>
        /// Used to get a background image URI (relative) from properties BasePath and Background
        /// </summary>
        /// <returns>
        /// Uri of a background of level
        /// Uri
        /// </returns>
        public Uri GetBackgroundPath()
        {
            string path = String.Format(BasePath, "backgrounds", this.Background);
            return new Uri(path, UriKind.Relative);
        }

        /// <summary>
        /// Creates an ImageBrush to be used to set background for canvas.
        /// Sets a background of a level
        /// </summary>
        /// <returns>
        /// ImageBrush of a background of a level
        /// ImageBrush
        /// </returns>
        public ImageBrush CreateBackground()
        {
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(this.GetBackgroundPath());
            image.Stretch = Stretch.UniformToFill;
            return image;
        }

        /// <summary>
        /// Creates grid plan from property Gameplan. 
        /// Grid is used to show gameplan in GUI
        /// </summary>
        /// <param name="grid">
        /// Reference to a grid used in GUI
        /// </param>
        public void CreateGridPlan(ref Grid grid, ref Game game)
        {
            grid.Name = "GamePlan";
            for(int i = 0; i < this.GamePlan.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            int row = 0;
            foreach (List<GameObject> objects in this.GamePlan)
            {
                int col = 0;
                foreach (GameObject obj in objects)
                {
                    Canvas canvas = obj.CreateImage(ref game);
                    Grid.SetRow(canvas, row);
                    Grid.SetColumn(canvas, col);
                    grid.Children.Add(canvas);
                    col++;
                }
                row++;
            }
        }

        /// <summary>
        /// Edits a canvas for requiremets for a level.
        /// Fills it with images of GameObject, already collected amount of that object and amount to win
        /// </summary>
        /// <param name="canvas">
        /// Reference to a canvas that is later used to be shown in GUI
        /// </param>
        public void CreateReqCanvas(ref Canvas canvas)
        {
            canvas.Name = "Requirements";
            Grid grid = new Grid();
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            for (int i = 0; i < 2; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
            }
            grid.RowDefinitions[0].Height = new System.Windows.GridLength(canvas.Height / 3 * 2);
            grid.Width = canvas.Width;
            grid.Height = canvas.Height;
            Canvas backpack = CreateBackPack();
            Grid.SetRow(backpack, 0);
            Grid.SetColumn(backpack, 0);
            Grid.SetColumnSpan(backpack, 2);
            TextBlock current = CreateTextBlock("CurrentAmount", this.Destroyed.ToString());
            Grid.SetRow(current, 1);
            Grid.SetColumn(current, 0);
            TextBlock totalAmount = CreateTextBlock("TotalAmount", this.ToDestroy.ToString());
            Grid.SetRow(totalAmount, 1);
            Grid.SetColumn(totalAmount, 1);
            this.DestroyedText = current;
            grid.Children.Add(current);
            grid.Children.Add(totalAmount);
            grid.Children.Add(backpack);
            canvas.Children.Add(grid);
        }

        private TextBlock CreateTextBlock(String name, String text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Name = name;
            textBlock.Text = text;
            textBlock.FontSize = 40;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            return textBlock;
        }

        private Canvas CreateBackPack()
        {
            Canvas canvas = new Canvas();
            canvas.Name = "Backpack";
            ImageBrush image = new ImageBrush();
            image.Stretch = Stretch.Uniform;
            Uri uri = new Uri("../../../images/objects/Backpack.png", UriKind.Relative);
            image.ImageSource = new BitmapImage(uri);
            canvas.Background = image;
            return canvas;
        }

        public bool SwapEm(Coordinates firstCoord, Coordinates secondCoord)
        {
            GameObject firstGameObject = this.GamePlan[firstCoord.row][firstCoord.col];
            this.GamePlan[firstCoord.row][firstCoord.col] = this.GamePlan[secondCoord.row][secondCoord.col];
            this.GamePlan[secondCoord.row][secondCoord.col] = firstGameObject;
            bool first = this.Match(firstCoord);
            bool second = this.Match(secondCoord);
            if (!(first || second))
            {
                this.GamePlan[secondCoord.row][secondCoord.col] = this.GamePlan[firstCoord.row][firstCoord.col];
                this.GamePlan[firstCoord.row][firstCoord.col] = firstGameObject;
                return false;
            }
            return true;
        }

        public Dictionary<Coordinates, Coordinates> WhatToMove()
        {
            Dictionary<Coordinates, Coordinates> toMove = new Dictionary<Coordinates, Coordinates>();
            for(int x = this.GamePlan.Count-1; x >= 0; x--)
            {
                for(int y = 0; y < this.GamePlan.Count; y++)
                {
                    if(this.GamePlan[x][y] == null)
                    {
                        for(int xNotNull = x; xNotNull>=0; xNotNull--)
                        {
                            if(this.GamePlan[xNotNull][y] != null)
                            {
                                toMove.Add(new Coordinates { row = xNotNull, col = y }, new Coordinates { row = x, col = y });
                                GameObject gameObject = this.GamePlan[xNotNull][y];
                                this.GamePlan[xNotNull][y] = null;
                                this.GamePlan[x][y] = gameObject;
                                break;
                            }
                        }
                    }
                }
            }
            return toMove;
        }

        /// <summary>
        /// Creates a string to be saved in a file where all properties of a level is saved
        /// </summary>
        /// <returns>
        /// String
        /// </returns>
        public String Serialize()
        {
            String text = "level" + this.Id + ".txt\n";
            foreach(List<GameObject> objects in this.GamePlan)
            {
                foreach(GameObject obj in objects)
                {
                    text+=obj.serialize()+"|";
                }
                text=text.Substring(0, text.Length - 1);
                text += "\n";
            }
            text += this.Background + "\n";
            return text;
        }

        private bool Match(Coordinates coord) 
        {
            List<Coordinates> matchesX = new List<Coordinates>();
            List<Coordinates> matchesY = new List<Coordinates>();
            GameObject gameObject = this.GamePlan[coord.row][coord.col];
            matchesX.Add(coord);
            matchesY.Add(coord);
            this.FindMatches(gameObject, coord, 1, 0, ref matchesX);
            this.FindMatches(gameObject, coord, -1, 0, ref matchesX);
            this.FindMatches(gameObject, coord, 0, 1, ref matchesY);
            this.FindMatches(gameObject, coord, 0, -1, ref matchesY);
            if(matchesX.Count > 2 && matchesY.Count > 2)
            {
                EraseGameObjects(matchesX);
                EraseGameObjects(matchesY);
                this.Destroyed += matchesX.Count + matchesY.Count - 1;
            }
            else if (matchesX.Count > 2)
            {
                EraseGameObjects(matchesX);
                this.Destroyed += matchesX.Count;
            }
            else if(matchesY.Count > 2)
            {
                EraseGameObjects(matchesY);
                this.Destroyed += matchesY.Count;
            }
            if (matchesX.Count > 2 || matchesY.Count > 2)
            {
                this.DestroyedText.Text = this.Destroyed.ToString();
                return true;
            }
            return false;
        }

        private void FindMatches(GameObject gameObject, Coordinates coord, int stepX, int stepY, ref List<Coordinates> coordinates)
        {
            int x = coord.row;
            int y = coord.col;
            while (x >= 0 && x < this.GamePlan.Count && y >= 0 && y < this.GamePlan[0].Count && this.GamePlan[x][y] == gameObject)
            {
                x += stepX;
                y += stepY;
                if(x >= 0 && x < this.GamePlan.Count && y >= 0 && y < this.GamePlan[0].Count && this.GamePlan[x][y] == gameObject)
                {
                    coordinates.Add(new Coordinates { row = x, col = y });
                }
            }
        }

        private void EraseGameObjects(List<Coordinates> coords)
        {
            foreach (Coordinates item in coords)
            {
                this.GamePlan[item.row][item.col] = null;
            }
        }

        public bool IsWon()
        {
            if(this.Destroyed == this.ToDestroy)
            {
                return true;
            }
            return false;
        }
    }
}