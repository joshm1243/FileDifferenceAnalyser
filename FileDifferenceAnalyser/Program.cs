using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{


    class Program
    {

     
       


        static IReadable RENAME(string file1Type)
        {
            IReadable file1;

            if (file1Type == "csv")
            {
                file1 = new CSVFile();

            }
            else if (file1Type == "tsv")
            {
                file1 = new TSVFile();

            }
            else
            {

                file1 = new TXTFile();

                if (file1Type != "txt")
                {
                    Console.WriteLine("The analyser doesn't currently support '." + file1Type + "' types.");
                    Console.WriteLine("Parameter 1 will be treated as a .txt, this may lead to unwanted results.");
                }
            }

            return file1;
            
        }

        static void Main()
        {

            bool stopProgram = false;
            string defaultInputPath = Repository.GetAbsolutePath("../../Input Files/");
            string defaultOutputPath = Repository.GetAbsolutePath("../../Output Files/");
            string file1Type;
            string file2Type;
            int granularity;



            Console.WriteLine("File Difference Analyser");
            Console.WriteLine("Type 'diff <file1> <file2>' to view the differences between two files");
            Console.WriteLine("By default, characters are analysed. To view word differences, use the '\\w' argument");
            Console.WriteLine("To change the default directory type 'setpath <path>' where <path> is rooted at C://");
            Console.WriteLine("Type 'help' for a list of commands");

           


            //While the user does not choose to terminate the program
            while (!stopProgram)
            {

                UI.Break();
                 

                UserInput input = UI.InterpretUserInput(UI.GetUserInput());


                
                if (input.Error == "")
                {

   
                    if (!input.CommandIsNull)
                    {

                        if (input.Command == "diff")
                        {

                            UI.Break();
          

                            file1Type = Repository.GetFileExtention(input.Parameters[0]);
                            file2Type = Repository.GetFileExtention(input.Parameters[1]);



                            IReadable file1 = RENAME(file1Type);
                            IReadable file2 = RENAME(file2Type);


                            if (!file1.Open(defaultInputPath + input.Parameters[0]))
                            {
                                Console.WriteLine("Parameter 1: '" + file1.Name + "' could not be opened");
                            }

                            if (!file2.Open(defaultInputPath + input.Parameters[1]))
                            {
                                Console.WriteLine("Parameter 2: '" + file2.Name + "' could not be opened");
                            }




                            granularity = input.Arguments.Contains("/l") ? 0 : 2;
                            granularity = input.Arguments.Contains("/w") ? 1 : granularity;


                            IWritable outputFile;

                            if (input.Arguments.Contains("/c"))
                            {
                                outputFile = new CSVFile();
                            }
                            else if (input.Arguments.Contains("/t"))
                            {
                                outputFile = new TSVFile();
                            }
                            else
                            {
                                outputFile = new TXTFile();
                            }


                            outputFile.Create(defaultOutputPath);

                            List<string> file1Content = file1.Read();
                            List<string> file2Content = file2.Read();

                            if (file1Content.SequenceEqual(file2Content))
                            {
                                string[] fileContent = outputFile.CreateDiffForFile(granularity, file1.Name, file2.Name) ;

                                outputFile.Write(fileContent);

                                Console.WriteLine("The two files were identical.");
                            }
                            else
                            {
                                List<Line> lines = file1.FindDifference(file2.Read(), granularity);

                                string[] fileContent = outputFile.CreateDiffForFile(granularity, file1.Name, file2.Name, lines);

                                UI.DisplayDiffOutput(lines);

                                outputFile.Write(fileContent);
                            }

                            UI.Break();
                            Console.WriteLine("A summary file was placed inside:");
                            Console.WriteLine(defaultOutputPath);






                        }
                        else if (input.Command == "inputdir")
                        {
                            UI.Break();
                            defaultInputPath = UI.MakePathLegal(input.Parameters[0]);
                            Console.WriteLine("The input directory for files has been changed");
                            
                        }
                        else if (input.Command == "outputdir")
                        {
                            UI.Break();
                            defaultOutputPath = UI.MakePathLegal(input.Parameters[0]);
                            

                            Console.WriteLine("The output directory for files has been changed");
                            
                        }
                        else if (input.Command == "showinputdir")
                        {
                            UI.Break();
                            Console.WriteLine(defaultInputPath);

                        }
                        else if (input.Command == "showoutputdir")
                        {
                            UI.Break();
                            Console.WriteLine(defaultOutputPath);

                        }
                        else if (input.Command == "help")
                        {

                            UI.Break();
                            UI.DisplayHelp();

                        }
                        else if (input.Command == "quit")
                        {
                            stopProgram = true;
                        }
                        else
                        {
                            UI.Break();
                            Console.WriteLine("'" + input.Command + "' is not a valid command");
                            Console.WriteLine("Type 'help' to view all commands");
                        }
                    }
                }
                else
                {
                    UI.Break();
                    Console.WriteLine(input.Error);
                }
            }
        }
    }
}
