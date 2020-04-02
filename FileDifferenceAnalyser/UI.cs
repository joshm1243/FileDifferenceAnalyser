using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class UI
    {

        //Sets the console cursor colour to red
        static public void CursorRed()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }

        //Sets the console cursor colour to green
        static public void CursorGreen()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        //Sets the console cursor colour to white
        static public void CursorWhite()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
