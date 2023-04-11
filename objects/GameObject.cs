using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;

namespace Triad_Matcher
{
    /// <summary>
    /// GameObject
    /// Used to save all the information about GameObject
    /// Only saves symbol of GameObject and works with it in some functions
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Symbol
        /// Saves Symbol of enum type Symbols
        /// </summary>
        public Symbols Symbol { get; init; }

        /// <summary>
        /// ImagePath
        /// Determinates a path (filename) of an png image file in images/objects from a Symbol
        /// </summary>
        private static Dictionary<Symbols, string> ImagePaths = new Dictionary<Symbols, string>() {
            //Level 1
            {Symbols.ph, "PinkHat" },
            {Symbols.gh, "GreenHat" },
            {Symbols.vh, "VioletHat" },
            //Level 2
            {Symbols.bq, "BlueQuartz" },
            {Symbols.e, "Emerald" },
            {Symbols.a, "Amethyst" },
            {Symbols.rq, "RoseQuartz" },
            {Symbols.yq, "YellowQuartz" },
            //Level 3
            {Symbols.c, "Candles" },
            {Symbols.tc, "TarotCards" },
            {Symbols.bi, "BottleItem" },
            {Symbols.mb, "MagicBall" },
            {Symbols.m, "Map" },
            //Level 4
            {Symbols.d, "Dandelion" },
            {Symbols.wf, "WhiteFlower" },
            {Symbols.l, "Leaf" },
            {Symbols.mt, "MushroomToadstool" },
            {Symbols.pf, "PinkFlower" },
            {Symbols.bb, "BlueBerry" }


        };

        /// <summary>
        /// BasePath
        /// A base path where images of objects are used
        /// </summary>
        private static String BasePath = "images/{0}/{1}.png";

        /// <summary>
        /// Creates instance of class GameObject
        /// </summary>
        /// <param name="symbol">
        /// String that is later parsed to an enum type Symbols
        /// </param>
        public GameObject(string symbol)
        {
            this.Symbol = GetSymbol(symbol);
        }

        /// <summary>
        /// Creates Uri for an image file of a specific GameObject
        /// </summary>
        /// <returns>
        /// Uri (relative) of an image file
        /// Uri 
        /// </returns>
        public Uri GetImagePath()
        {
            String? fileName; 
            ImagePaths.TryGetValue(this.Symbol,out fileName);
            String path = String.Format(BasePath,"objects",fileName);
            return new Uri(path, UriKind.Relative);
        }

        /// <summary>
        /// Creates canvas to be used for a GUI represantation of a GameObject
        /// </summary>
        /// <returns>
        /// Canvas with backkground image of a GameObject
        /// Canvas
        /// </returns>
        public Canvas CreateImage(ref Game game, int rowColAmount)
        {
            double width = game.GamePlanGrid.Width;
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(this.GetImagePath());
            image.Stretch = Stretch.Uniform;
            Canvas canvas = new Canvas();
            canvas.Margin = new Thickness((width*3.25/28) /rowColAmount);
            canvas.Visibility = System.Windows.Visibility.Visible;
            canvas.Background = image;
            canvas.Focusable = true;
            canvas.MouseDown += new MouseButtonEventHandler(game.Choose);
            canvas.MouseEnter += new MouseEventHandler(MouseInCanvas);
            canvas.MouseLeave += new MouseEventHandler(MouseOutOfCanvas);
            canvas.Cursor = Cursors.Hand;
            return canvas;
        }
        public static void MouseInCanvas(Object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Canvas))
            {
                SelectedCanvas((Canvas)sender, 0.9);
            }
            else
            {
                throw new InvalidWPFElementException("Element is not canvas");
            }
        }

        public static void MouseOutOfCanvas(Object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Canvas))
            {
                DeselectedCanvas((Canvas)sender);
            }
            else
            {
                throw new InvalidWPFElementException("Element is not canvas");
            }

        }

        public static void SelectedCanvas(Canvas canvas, double opacityValue)
        {
            canvas.Background.Opacity = opacityValue;
        }

        public static void DeselectedCanvas(Canvas canvas)
        {
            canvas.Background.Opacity = 1;
        }

        /// <summary>
        /// Gets enum type Symbols froma string
        /// </summary>
        /// <param name="symbolString">
        /// Symbol in string representation
        /// </param>
        /// <returns>
        /// Specific Symbol in enum type Symbols
        /// </returns>
        public static Symbols GetSymbol(String symbolString)
        {
            Symbols symbol;
            if (Enum.TryParse(symbolString, out symbol))
            {
                return symbol;
            }
            return Symbols.failed;
        }

        /// <summary>
        /// Used to format GameObject to be serialized in a file
        /// </summary>
        /// <returns>
        /// String in a specific format (only symbol) to be saved in a file
        /// String
        /// </returns>
        public String Serialize()
        {
            return this.Symbol.ToString();
        }


    }
}
