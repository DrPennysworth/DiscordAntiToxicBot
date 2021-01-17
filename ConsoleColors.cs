using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAntiToxicBot
{
    public static class ConsoleColors
    {
        public static void ColorOut(string str,  ConsoleColor fgColor, ConsoleColor bgColor)
        {
            var origBG = Console.BackgroundColor;
            var origFG = Console.ForegroundColor;

            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            Console.WriteLine(str);
            Console.BackgroundColor = origBG;
            Console.ForegroundColor = origFG;
        }
        public static void ColorOut(string str, ConsoleColor fgColor)
        {
            var origBG = Console.BackgroundColor;
            var origFG = Console.ForegroundColor;

            Console.ForegroundColor = fgColor;
            Console.WriteLine(str);
            Console.ForegroundColor = origFG;
        }
    }
}
