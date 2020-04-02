using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{

    class Program
    {

        static void Main(string[] args)
        {

            IReadable file1 = new TXTFile();
            IReadable file2 = new TXTFile();

            if (file1.Open("../../Files/File1.txt"))
            {
                Console.WriteLine(file1.Name + " has been opened");
            }

            if (file2.Open("../../Files/File2.txt"))
            {
                Console.WriteLine(file2.Name + " has been opened");
            }

            foreach (LineComponent lc in file1.FindDifference(file2.Read()))
            {

                if (lc.Variant == -1)
                {
                    UI.CursorRed();
                    Console.WriteLine(" - " + lc.Text);
                }
                else if (lc.Variant == 1)
                {
                    UI.CursorGreen();
                    Console.WriteLine(" + " + lc.Text);
                }
                else
                {
                    UI.CursorWhite();
                    Console.WriteLine(lc.Text);
                }

                
            }
            
            Console.ReadKey();
            
        }
    }
}
