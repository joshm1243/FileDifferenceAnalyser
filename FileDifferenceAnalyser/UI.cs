using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class UI
    {

        //Creates a line break on the console
        static public void Break()
        {
            Console.WriteLine();
        }

        //Resets the UI to a black background and white text
        static public void Reset()
        {
            CursorWhite();
        }

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

        //Sets the console background colour to red
        static public void BackgroundBlack()
        {
            Console.BackgroundColor = ConsoleColor.Black;
        }

        //Sets the console background colour to green
        static public void BackgroundGreen()
        {
            Console.BackgroundColor = ConsoleColor.Green;
        }

        //Sets the console background colour to red
        static public void BackgroundRed()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
        }

        static public void DisplayHelp()
        {

            Console.WriteLine("Command:     'diff [file1.txt] [file2.txt] {/w}'");
            Console.WriteLine("Description: Analyses the character differences between two files");
            Console.WriteLine("Arguments: ");
            Console.WriteLine("\t      /c   Saves the output as a .csv file");
            Console.WriteLine("\t           To preserve Excel formatting, the follwing symbols will be used:");
            Console.WriteLine("\t           '^' Inside both files | '<' Inside left file | '>' Inside right file");
            Console.WriteLine("\t      /t   Saves the output as a .tsv file");
            Console.WriteLine("\t      /w   The differences between words are analysed");
            Console.WriteLine("\t      /l   The differences between lines are analysed");

            UI.Break();

            Console.WriteLine("Command:     'inputdir [C://filepath/]'");
            Console.WriteLine("Description: Sets the default directory to look for files");
            Console.WriteLine("Example: 'inputdir C://...'");

            UI.Break();

            Console.WriteLine("Command:     'outputdir [C://filepath/]'");
            Console.WriteLine("Description: Sets the default directory to output file summaries");
            Console.WriteLine("Example: 'outputdir C://...'");

        }

        static public string MakePathLegal(string filePath)
        {
            string legalFilePath = filePath;
            List<char> filePathList = new List<char>();

            filePathList = filePath.ToList();
            if (filePathList[filePathList.Count() - 1] != '\\')
            {
                filePathList.Add('\\');
                legalFilePath = string.Join("", filePathList);

            }

            return legalFilePath;
        }

        //Physically displays the
        static public void DisplayDiffOutput(List<Line> lines)
        {
            foreach (Line l in lines)
            {
                Console.WriteLine();


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
                            BackgroundRed();
                        }
                        else
                        {
                            CursorRed();
                        }
                    }
                    else if (p.Variant == 1)
                    {

                        if (p.Text == " ")
                        {
                            BackgroundGreen();
                        }
                        else
                        {
                            CursorGreen();
                        }
                    }
                    else
                    {
                        if (p.Text == " ")
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            UI.CursorWhite();
                        }

                    }

                    Console.Write(p.Text);


                }
            }

        }


        static public string[] GetUserInput()
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim();
            return input.Split(' ');
        }

        static public UserInput InterpretUserInput(string[] inputArray)
        {

            UserInput commandStructure = new UserInput();


            try
            {
                if (inputArray[0] == "")
                {
                    commandStructure.CommandIsNull = true;
                } 
                else
                {
                    commandStructure.Command = inputArray[0];
                }
            }
            catch (IndexOutOfRangeException)
            {
                commandStructure.Error = " ";
            }
           

            if (!commandStructure.CommandIsNull) 
            {

                if (commandStructure.Command == "diff")
                {

                    try
                    {

                        commandStructure.Parameters = new string[]
                        {
                            inputArray[1],
                            inputArray[2]
                        };

                    }
                    catch (IndexOutOfRangeException)
                    {
                        commandStructure.Error = "The 'diff' command accepts a minimum of 2 parameters.";
                    }

                    try
                    {
                        commandStructure.Arguments = new string[]
                        {
                            inputArray[3]
                        };
                    }
                    catch (IndexOutOfRangeException)
                    {
                        commandStructure.Arguments = new string[]
                        {
                            string.Empty
                        };
                    }
                }
                else if (commandStructure.Command == "inputdir" || commandStructure.Command == "outputdir")
                {

                    try
                    {
                        commandStructure.Parameters = new string[]
                        {
                            inputArray[1]
                        };
                    }
                    catch (IndexOutOfRangeException)
                    {
                        commandStructure.Error = "'" + commandStructure.Command + "' must have at least 1 parameter.";
                    }

                }
                else
                {
                    commandStructure.Command = inputArray[0];
                }

            }

            return commandStructure;

        }
    }
}
