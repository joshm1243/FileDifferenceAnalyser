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
            //int lastLocatedSimilarity = -1;

            List<LineComponent> fileComponents = new List<LineComponent>();

            List<Block> similarityBlocks = new List<Block>();

            int file1BlockStart = -1;
            int file2BlockStart = -1;
            int file1BlockEnd = -1;
            int file2BlockEnd = -1;
            int blockLength = -1;

            for (int i = 0; i < file1Content.Count; i++)
            {

                ////If the line is also in the second file
                //if (file2Content.Contains(file1Content[i]))
                //{

                //    //If a search for the line has not occurred before
                //    if (!recentOccurences.ContainsKey(file1Content[i]))
                //    {                      
                //        recentOccurences.Add(file1Content[i], -1);
                //    }

                //    //Finding the line position in the second text file
                //    locatedPosition = file2Content.IndexOf(file1Content[i],recentOccurences[file1Content[i]] + 1);

                //    //A corresponding value is inside the file2 list
                //    if (locatedPosition != -1 && locatedPosition > lastLocatedSimilarity)
                //    {
                //        similarLineNumbers.Add(new int[2] { i, locatedPosition});
                //        recentOccurences[file1Content[i]] = locatedPosition;
                //        lastLocatedSimilarity = locatedPosition;
                //    }
                //}

                //If the line is also in the second file
                if (file2Content.Contains(file1Content[i]))
                {

                    //If a search for the line has not occurred before
                    if (!recentOccurences.ContainsKey(file1Content[i]))
                    {
                        recentOccurences.Add(file1Content[i], -1);
                    }

                    //Finding the line position in the second text file
                    locatedPosition = file2Content.IndexOf(file1Content[i], recentOccurences[file1Content[i]] + 1);

                    //A corresponding value is inside the file2 list
                    if (locatedPosition != -1)
                    {
                        recentOccurences[file1Content[i]] = locatedPosition;

                        if (i > 0 && similarityBlocks.Count() > 0)
                        {
                            file1BlockStart = similarityBlocks[similarityBlocks.Count() - 1].File1Start;
                            file2BlockStart = similarityBlocks[similarityBlocks.Count() - 1].File2Start;
                            blockLength = similarityBlocks[similarityBlocks.Count() - 1].BlockLength;
                        }

                        if (i > 0 && file1BlockStart + blockLength == i && file2BlockStart + blockLength == locatedPosition)
                        {
                            similarityBlocks[similarityBlocks.Count() - 1].BlockLength++;

                            //similarLineNumbers[similarLineNumbers.Count() - 1][2]++;
                        }
                        else
                        {

                            Block tempBlock = new Block();
                            tempBlock.File1Start = i;
                            tempBlock.File2Start = locatedPosition;
                            tempBlock.BlockLength = 1;

                            similarityBlocks.Add(tempBlock);

                            //similarLineNumbers.Add(new int[3] { i, locatedPosition, 1});           
                        }
                    }
                }
            }

            //Beginning the algorithm
            Console.WriteLine();

            //Prioritises the similarity blocks that have the highest length
            similarityBlocks = similarityBlocks.OrderBy(Block => Block.BlockLength).ToList();


            int scA;
            int scB;
            int scC;
            int scD;

            for (int i = 0; i < similarityBlocks.Count(); i++)
            {

                blockLength = similarityBlocks[i].BlockLength;
                file1BlockStart = similarityBlocks[i].File1Start;
                file1BlockEnd = (file1BlockStart + blockLength) - 1;
                file2BlockStart = similarityBlocks[i].File2Start;
                file2BlockEnd = (file2BlockStart + blockLength) - 1;

                scA = similarityBlocks.FindIndex(Block => file1BlockEnd < Block.File1Start && !Block.Straddles && Block.File1Start != file1BlockStart);
                scB = similarityBlocks.FindIndex(Block => file2BlockStart > ((Block.File2Start + Block.BlockLength) - 1) && !Block.Straddles && Block.File2Start != file2BlockStart);
                scC = similarityBlocks.FindIndex(Block => file1BlockStart > ((Block.File1Start + Block.BlockLength) - 1) && !Block.Straddles && Block.File1Start != file1BlockStart);
                scD = similarityBlocks.FindIndex(Block => file2BlockEnd < Block.File2Start && !Block.Straddles && Block.File2Start != file2BlockStart);

                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("Starting on new block " + similarityBlocks[i].File1Start + "\t" + similarityBlocks[i].File2Start);

                Console.WriteLine(scA);
                Console.WriteLine(scB);
                Console.WriteLine(scC);
                Console.WriteLine(scD);
                Console.WriteLine("-----------------------------------------------------");

                if ((scA > -1 && scB > -1) || (scC > -1 && scD > -1))
                {
                    if (!(scA > -1 && scB > -1 && scC > -1 && scD > -1))
                    {
                        similarityBlocks[i].Straddles = true;
                        //similarityBlocks.Remove(similarityBlocks[i]);
                    }
                }

            }

            

            similarityBlocks = similarityBlocks.OrderBy(Block => Block.File1Start).ToList();

            similarityBlocks = similarityBlocks.Where(Block => !Block.Straddles).ToList();

            for (int i = 0; i < similarityBlocks.Count(); i++)
            {
                Console.WriteLine(similarityBlocks[i].File1Start + "\t" + similarityBlocks[i].File2Start + "\t" + similarityBlocks[i].BlockLength + "\t" + similarityBlocks[i].Straddles);
            }




            //Program Halt
            Console.ReadKey();


            //for (int i = 0; i < similarLineNumbers.Count(); i++)
            //{



            //    //Top diagonal top to bottom
            //    if (i == 0)
            //    {

            //        for (int j = 0; j < similarLineNumbers[i][1]; j++)
            //        {
            //            LineComponent lc = new LineComponent();
            //            lc.Variant = 1;
            //            lc.Text = file2Content[j];
            //            fileComponents.Add(lc);
            //            //Console.WriteLine(" + " + file2Content[j]);
            //        }

            //        for (int j = 0; j < similarLineNumbers[i][0]; j++)
            //        {
            //            LineComponent lc = new LineComponent();
            //            lc.Variant = -1;
            //            lc.Text = file1Content[j];
            //            fileComponents.Add(lc);
            //            //Console.WriteLine(" - " + file1Content[j]);
            //        }
            //    }
            //    else
            //    {

            //        for (int j = similarLineNumbers[i - 1][1] + 1; j < similarLineNumbers[i][1]; j++)
            //        {
            //            LineComponent lc = new LineComponent();
            //            lc.Variant = 1;
            //            lc.Text = file2Content[j];
            //            fileComponents.Add(lc);
            //            //Console.WriteLine(" + " + file2Content[j]);
            //        }


            //        for (int j = similarLineNumbers[i - 1][0] + 1; j < similarLineNumbers[i][0]; j++)
            //        {
            //            LineComponent lc = new LineComponent();
            //            lc.Variant = -1;
            //            lc.Text = file1Content[j];
            //            fileComponents.Add(lc);
            //            //Console.WriteLine(" - " + file1Content[j]);
            //        }

            //    }

            //    LineComponent c = new LineComponent();
            //    c.Variant = 0;
            //    c.Text = file1Content[similarLineNumbers[i][0]];
            //    fileComponents.Add(c);
            //    //Console.WriteLine(file1Content[similarLineNumbers[i][0]]);
            //}

            //if (similarLineNumbers.Count() == 0)
            //{
            //    similarLineNumbers.Add(new int[2] { -1, -1});
            //}

            //for (int i = similarLineNumbers[similarLineNumbers.Count() - 1][1] + 1; i < file2Content.Count() - 1; i++)
            //{
            //    LineComponent c = new LineComponent();
            //    c.Variant = 1;
            //    c.Text = file2Content[i];
            //    fileComponents.Add(c);
            //    //Console.WriteLine(" + " + file2Content[i]);
            //}

            //for (int i = similarLineNumbers[similarLineNumbers.Count() - 1][0] + 1; i < file1Content.Count() - 1; i++)
            //{
            //    LineComponent c = new LineComponent();
            //    c.Variant = -1;
            //    c.Text = file1Content[i];
            //    fileComponents.Add(c);
            //    //Console.WriteLine(" - " + file1Content[i]);
            //}

            int lastFile1BlockEnd = -1;
            int lastFile2BlockEnd = -1;

            for (int i = 0; i < similarityBlocks.Count(); i++)
            {



                //Top diagonal top to bottom
                if (i == 0)
                {

                    for (int j = 0; j < similarityBlocks[i].File1Start; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = -1;
                        lc.Text = file1Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" - " + file1Content[j]);
                    }

                    for (int j = 0; j < similarityBlocks[i].File2Start; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = 1;
                        lc.Text = file2Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" + " + file2Content[j]);
                    }

                }
                else
                {

                    for (int j = similarityBlocks[i - 1].File1Start + similarityBlocks[i - 1].BlockLength; j < similarityBlocks[i].File1Start; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = -1;
                        lc.Text = file1Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" - " + file1Content[j]);
                    }

                    for (int j = similarityBlocks[i - 1].File2Start + similarityBlocks[i - 1].BlockLength; j < similarityBlocks[i].File2Start; j++)
                    {
                        LineComponent lc = new LineComponent();
                        lc.Variant = 1;
                        lc.Text = file2Content[j];
                        fileComponents.Add(lc);
                        //Console.WriteLine(" + " + file2Content[j]);
                    }
                }

                //Adds the current similarity block
                for (int j = similarityBlocks[i].File1Start; j < similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength; j++)
                {
                    LineComponent c = new LineComponent();
                    c.Variant = 0;
                    c.Text = file1Content[j];
                    fileComponents.Add(c);
                }

                //LineComponent c = new LineComponent();
                //c.Variant = 0;
                //c.Text = file1Content[similarityBlocks[i].File1Start];
                //fileComponents.Add(c);
                //Console.WriteLine(file1Content[similarLineNumbers[i][0]]);
            }

            if (similarityBlocks.Count() == 0)
            {
                similarityBlocks.Add(new Block { File1Start = -1, File2Start = -1, BlockLength = 1 });
            }

            lastFile1BlockEnd = similarityBlocks[similarityBlocks.Count() - 1].File1Start + similarityBlocks[similarityBlocks.Count() - 1].BlockLength;
            lastFile2BlockEnd = similarityBlocks[similarityBlocks.Count() - 1].File2Start + similarityBlocks[similarityBlocks.Count() - 1].BlockLength;

            for (int i = lastFile1BlockEnd; i < file1Content.Count(); i++)
            {
                LineComponent c = new LineComponent();
                c.Variant = -1;
                c.Text = file1Content[i];
                fileComponents.Add(c);
                //Console.WriteLine(" - " + file1Content[i]);
            }

            for (int i = lastFile2BlockEnd; i < file2Content.Count(); i++)
            {
                LineComponent c = new LineComponent();
                c.Variant = 1;
                c.Text = file2Content[i];
                fileComponents.Add(c);
                //Console.WriteLine(" + " + file2Content[i]);
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
