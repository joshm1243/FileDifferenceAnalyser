using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileDifferenceAnalyser
{

    abstract class Repository : IReadable
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }

        //Available for read and write
        public bool Open(string specifiedPath)
        {

            //Checking if a path has been inputted
            if (specifiedPath.Length == 0)
            {
                throw new ArgumentNullException();
            }

            if (Path.IsPathRooted(specifiedPath))
            {
                FilePath = specifiedPath;
            }
            else
            {
                FilePath = Path.GetFullPath(specifiedPath);
            }

            if (File.Exists(FilePath))
            {
                Name = FilePath.Split('\\')[FilePath.Split('\\').Length - 1];
                IsOpen = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Forces derived classes to implement some kind of 'read' method
        public abstract string[] Read();

        //Forces derived classes to implement some kind of 'write' method
        public abstract void Write();

        //Avaiable for read and write methods
        public List<LineComponent> FindDifference(string[] file2Array)
        {

            List<string> file1Content = this.Read().ToList<string>();
            List<string> file2Content = file2Array.ToList<string>();

            //Finding the 'cluster ends' where similar lines are found
            //Everything above the 'cluster end' will be + or - (for that cluster only)

            List<int[]> similarLineNumbers = new List<int[]>();

            Dictionary<string, int> recentOccurences = new Dictionary<string, int>();

            int locatedPosition;
            int lastLocatedSimilarity = -1;

            List<LineComponent> fileComponents = new List<LineComponent>();

            for (int i = 0; i < file1Content.Count; i++)
            {

                //If the line is also in the second file
                if (file2Content.Contains(file1Content[i]))
                {

                    //If a search for the line has not occurred before
                    if (!recentOccurences.ContainsKey(file1Content[i]))
                    {                      
                        recentOccurences.Add(file1Content[i], -1);
                    }

                    //Finding the line position in the second text file
                    locatedPosition = file2Content.IndexOf(file1Content[i],recentOccurences[file1Content[i]] + 1);
                    
                    //A corresponding value is inside the file2 list
                    if (locatedPosition != -1 && locatedPosition > lastLocatedSimilarity)
                    {
                        similarLineNumbers.Add(new int[2] { i, locatedPosition});
                        recentOccurences[file1Content[i]] = locatedPosition;
                        lastLocatedSimilarity = locatedPosition;
                    }
                }
            }

            similarLineNumbers.RemoveAt(similarLineNumbers.Count() - 1);

            //Beginning the algorithm
            Console.WriteLine();


            for (int i = 0; i < similarLineNumbers.Count(); i++)
            {

                

                //Top diagonal top to bottom
                if (i == 0)
                {

                    for (int j = 0; j < similarLineNumbers[i][1]; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = 1;
                        lc.Text = file2Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" + " + file2Content[j]);
                    }

                    for (int j = 0; j < similarLineNumbers[i][0]; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = -1;
                        lc.Text = file1Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" - " + file1Content[j]);
                    }
                }
                else
                {

                    for (int j = similarLineNumbers[i - 1][1] + 1; j < similarLineNumbers[i][1]; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = 1;
                        lc.Text = file2Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" + " + file2Content[j]);
                    }


                    for (int j = similarLineNumbers[i - 1][0] + 1; j < similarLineNumbers[i][0]; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = -1;
                        lc.Text = file1Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" - " + file1Content[j]);
                    }

                }

                LineComponent c = new LineComponent();
                c.Variant = 0;
                c.Text = file1Content[similarLineNumbers[i][0]];
                fileComponents.Add(c);
                //Console.WriteLine(file1Content[similarLineNumbers[i][0]]);
            }

            if (similarLineNumbers.Count() == 0)
            {
                similarLineNumbers.Add(new int[2] { -1, -1});
            }

            for (int i = similarLineNumbers[similarLineNumbers.Count() - 1][1] + 1; i < file2Content.Count() - 1; i++)
            {
                LineComponent c = new LineComponent();
                c.Variant = 1;
                c.Text = file2Content[i];
                fileComponents.Add(c);
                //Console.WriteLine(" + " + file2Content[i]);
            }

            for (int i = similarLineNumbers[similarLineNumbers.Count() - 1][0] + 1; i < file1Content.Count() - 1; i++)
            {
                LineComponent c = new LineComponent();
                c.Variant = -1;
                c.Text = file1Content[i];
                fileComponents.Add(c);
                //Console.WriteLine(" - " + file1Content[i]);
            }



            


            if (!file1Content.SequenceEqual(file2Content))
            {



            }
            else
            {
                //The two files are exactly the same
            }


            return fileComponents;

        }
    }
}
