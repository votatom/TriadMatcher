using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triad_Matcher.utility
{
    public class TestingUtility
    {
        public static string Path = "../../../testing/test.txt";

        public static void WriteText<T>(T obj)
        {
            File.WriteAllText(Path, "\n\n");
            File.WriteAllText(Path, obj.ToString());
        }
    }
}
