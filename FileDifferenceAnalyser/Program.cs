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

            

            foreach (Line l in file1.FindDifference(file2.Read(), 1))
            {
                //Console.WriteLine();

  
                //if (l.File1Number == l.File2Number)
                //{
                //    Console.WriteLine("Line: " + (l.File1Number + 1));
                //}
                //else
                //{
                //    Console.WriteLine("Line: " + l.File1Number + ":" + l.File2Number);
                //}




                foreach (Phrase p in l.Phrases)
                {

                    Console.BackgroundColor = ConsoleColor.Black;

                    if (p.Variant == -1)
                    {
                        
                        if (p.Text == " ")
                        {
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        }
                        else
                        {
                            UI.CursorRed();
                        }
                    }
                    else if (p.Variant == 1)
                    {

                        if (p.Text == " ")
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            UI.CursorGreen();
                        }

                   


                    }
                    else
                    {

                        if (p.Text == " ")
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            UI.CursorWhite();
                        }

                    }

                    Console.Write(p.Text);



                }

                Console.WriteLine();

            }
            
            Console.ReadKey();
            
        }
    }
}
