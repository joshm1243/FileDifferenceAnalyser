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

            Dictionary<string, int> file1RecentOccurences = new Dictionary<string, int>();
            Dictionary<string, int> file2RecentOccurences = new Dictionary<string, int>();

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

                //If the line is also in the second file
                if (file2Content.Contains(file1Content[i]))
                {

                    //If a search for the line has not occurred before
                    if (!file2RecentOccurences.ContainsKey(file1Content[i]))
                    {
                        file2RecentOccurences.Add(file1Content[i], -1);
                    }

                    //Finding the line position in the second text file
                    locatedPosition = file2Content.IndexOf(file1Content[i], file2RecentOccurences[file1Content[i]] + 1);

                    //A corresponding value is inside the file2 list
                    if (locatedPosition != -1)
                    {
                        file2RecentOccurences[file1Content[i]] = locatedPosition;

                        if (i > 0 && similarityBlocks.Count() > 0)
                        {
                            file1BlockStart = similarityBlocks[similarityBlocks.Count() - 1].File1Start;
                            file2BlockStart = similarityBlocks[similarityBlocks.Count() - 1].File2Start;
                            blockLength = similarityBlocks[similarityBlocks.Count() - 1].BlockLength;
                        }

                        if (i > 0 && similarityBlocks.Count() > 0 && file1BlockStart + blockLength == i && file2BlockStart + blockLength == locatedPosition)
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


            for (int i = 0; i < similarityBlocks.Count(); i++)
            {
                Console.WriteLine(similarityBlocks[i].File1Start + "\t" + similarityBlocks[i].File2Start + "\t" + similarityBlocks[i].BlockLength + "\t" + similarityBlocks[i].Removed);
            }



            //2
            //Working out whether mutliple copies are best placed elsewhere


            int possibleFile1Start = -1;
            int possibleFile2Start = -1;
            int possibleFile1End = -1;
            int possibleFile2End = -1;

            int additionalLength = -1;
            int newStateLength = -1;


            int file1CurrentBlockPosition = -1;
            int file2CurrentBlockPosiiton = -1;


            int lengthToAdd = -1;

            if (similarityBlocks.Count() > 0)
            {

                for (int i = 0; i < similarityBlocks.Count(); i++)
                {

                    if (!similarityBlocks[i].Removed)
                    {

                        additionalLength = 0;
                    

                        possibleFile1Start = similarityBlocks[i].File1Start;
                        possibleFile2Start = similarityBlocks[i].File2Start;

                        while (possibleFile1Start > 0 && possibleFile2Start > 0)
                        {
                            if (file1Content[possibleFile1Start - 1] == file2Content[possibleFile2Start - 1])
                            {
                                possibleFile1Start--;
                                possibleFile2Start--;
                                additionalLength++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        possibleFile1End = (similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength) - 1;
                        possibleFile2End = (similarityBlocks[i].File2Start + similarityBlocks[i].BlockLength) - 1;

                        while (possibleFile1End < file1Content.Count() - 1 && possibleFile2End < file2Content.Count() - 1)
                        {

                            if (file1Content[possibleFile1End + 1] == file2Content[possibleFile2End + 1])
                            {
                                additionalLength++;
                                possibleFile1End++;
                                possibleFile2End++;
                            }
                            else
                            {
                                break;
                            }
                        }

    
                        if (additionalLength > 0)
                        {

                            newStateLength = similarityBlocks[i].BlockLength + additionalLength;



                            lengthToAdd = 0;

                            //Looping over each of the items above the block
                            for (int j = 0; j < similarityBlocks[i].File1Start - possibleFile1Start; j++)
                            {

                                file1CurrentBlockPosition = Block.GetBlockForIndex(similarityBlocks, 1, j + possibleFile1Start);
                                file2CurrentBlockPosiiton = Block.GetBlockForIndex(similarityBlocks, 2, j + possibleFile2Start);
             
                                if (file1CurrentBlockPosition > -1)
                                {
                                    if (similarityBlocks[file1CurrentBlockPosition].BlockLength <= newStateLength)
                                    {
                                        if (j + possibleFile1Start == similarityBlocks[file1CurrentBlockPosition].File1Start)
                                        {
                                            similarityBlocks[file1CurrentBlockPosition].File1Start++;
                                            similarityBlocks[file1CurrentBlockPosition].File2Start++;
                                            similarityBlocks[file1CurrentBlockPosition].BlockLength--;
                                        } 
                                        else if (j + possibleFile1Start != similarityBlocks[file1CurrentBlockPosition].File1Start)
                                        {
                                            similarityBlocks[file1CurrentBlockPosition].BlockLength -= (similarityBlocks[file1CurrentBlockPosition].File2Start - (j + possibleFile1Start)) + 1;
                                        }

                                        if (similarityBlocks[file1CurrentBlockPosition].BlockLength < 2)
                                        {
                                            similarityBlocks[file1CurrentBlockPosition].Removed = true;
                                        }
                                    }
                                } 

                                if (file2CurrentBlockPosiiton > -1)
                                {
                                    if (similarityBlocks[file2CurrentBlockPosiiton].BlockLength <= newStateLength)
                                    {
                                        if (j + possibleFile2Start == similarityBlocks[file2CurrentBlockPosiiton].File2Start)
                                        {
                                            similarityBlocks[file2CurrentBlockPosiiton].File1Start++;
                                            similarityBlocks[file2CurrentBlockPosiiton].File2Start++;
                                            similarityBlocks[file2CurrentBlockPosiiton].BlockLength--;
                                        } 
                                        else if (j + possibleFile2Start != similarityBlocks[file2CurrentBlockPosiiton].File2Start)
                                        {
                                            similarityBlocks[file2CurrentBlockPosiiton].BlockLength -= (similarityBlocks[file2CurrentBlockPosiiton].File2Start - (j + possibleFile2Start)) + 1;
                                        }

                                        if (similarityBlocks[file2CurrentBlockPosiiton].BlockLength < 2)
                                        {
                                            similarityBlocks[file2CurrentBlockPosiiton].Removed = true;
                                        }
                                    }
                                }

                                lengthToAdd++;

                            }

                            if (lengthToAdd > 0)
                            {
                                similarityBlocks[i].File1Start -= lengthToAdd;
                                similarityBlocks[i].File2Start -= lengthToAdd;
                                similarityBlocks[i].BlockLength += lengthToAdd;
                            }

                            lengthToAdd = 0;

                            for (int j = 0; j < possibleFile1End - ((similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength) - 1); j++)
                            {

                                file1CurrentBlockPosition = Block.GetBlockForIndex(similarityBlocks, 1, j + (similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength));
                                file2CurrentBlockPosiiton = Block.GetBlockForIndex(similarityBlocks, 2, j + (similarityBlocks[i].File2Start + similarityBlocks[i].BlockLength));

                                if (file1CurrentBlockPosition > -1)
                                {

                                    if (similarityBlocks[file1CurrentBlockPosition].BlockLength <= newStateLength)
                                    {

                                        if (j + (similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength) == similarityBlocks[file1CurrentBlockPosition].File1Start)
                                        {

                                            similarityBlocks[file2CurrentBlockPosiiton].File1Start++;
                                            similarityBlocks[file2CurrentBlockPosiiton].File2Start++;
                                            similarityBlocks[file1CurrentBlockPosition].BlockLength--;
                                            
                                        } 
                                        else if (j + (similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength) != similarityBlocks[file1CurrentBlockPosition].File1Start)
                                        {
                                            similarityBlocks[file1CurrentBlockPosition].BlockLength -= (similarityBlocks[file1CurrentBlockPosition].File2Start - (j + possibleFile1Start)) + 1;
                                        }

                                        if (similarityBlocks[file1CurrentBlockPosition].BlockLength < 2)
                                        {
                                            similarityBlocks[file1CurrentBlockPosition].Removed = true;
                                        }
                                    }
                                }


                                if (file2CurrentBlockPosiiton > -1)
                                {

                                    if (similarityBlocks[file2CurrentBlockPosiiton].BlockLength <= newStateLength)
                                    {

                                        if (j + (similarityBlocks[i].File2Start + similarityBlocks[i].BlockLength) == similarityBlocks[file2CurrentBlockPosiiton].File2Start)
                                        {
                                            similarityBlocks[file2CurrentBlockPosiiton].File1Start++;
                                            similarityBlocks[file2CurrentBlockPosiiton].File2Start++;
                                            similarityBlocks[file2CurrentBlockPosiiton].BlockLength--;
                                        } 
                                        else if (j + (similarityBlocks[i].File2Start + similarityBlocks[i].BlockLength) != similarityBlocks[file2CurrentBlockPosiiton].File2Start)
                                        {
                                            similarityBlocks[file2CurrentBlockPosiiton].BlockLength -= (similarityBlocks[file2CurrentBlockPosiiton].File2Start - (j + possibleFile2Start)) + 1;
                                        }

                                        if (similarityBlocks[file2CurrentBlockPosiiton].BlockLength < 2)
                                        {
                                            similarityBlocks[file2CurrentBlockPosiiton].Removed = true;
                                        }
                                    }
                                }

                                lengthToAdd++;
  
                            }

                            if (lengthToAdd > 0)
                            {
                                similarityBlocks[i].BlockLength += lengthToAdd;
                            }
                        }
                    }
                }
            }


         


            similarityBlocks = similarityBlocks.Where(Block => !Block.Removed).ToList();

            //Prioritises the similarity blocks that have the highest length
            similarityBlocks = similarityBlocks.OrderByDescending(Block => Block.BlockLength).ToList();

            //3
            //Prioritising blocks of a larger length



            for (int i = 0; i < similarityBlocks.Count(); i++)
            {

                blockLength = similarityBlocks[i].BlockLength;
                file1BlockStart = similarityBlocks[i].File1Start;
                file1BlockEnd = (file1BlockStart + blockLength) - 1;
                file2BlockStart = similarityBlocks[i].File2Start;
                file2BlockEnd = (file2BlockStart + blockLength) - 1;


                if (!similarityBlocks[i].Straddles)
                {

                    for (int j = 0; j < similarityBlocks.Count(); j++)
                    {

                        if (i != j && !similarityBlocks[j].Straddles)
                        {

                            if (similarityBlocks[j].File1Start < similarityBlocks[i].File1Start)
                            {
                                if (similarityBlocks[j].File2Start > similarityBlocks[i].File2Start)
                                {
                                    similarityBlocks[j].Straddles = true;
                                }
                            }

                            if (similarityBlocks[j].File2Start < similarityBlocks[i].File2Start)
                            {
                                if (similarityBlocks[j].File1Start > similarityBlocks[i].File1Start)
                                {
                                    similarityBlocks[j].Straddles = true;
                                }
                            }
                        }
                    }
                }
            }

            similarityBlocks = similarityBlocks.OrderBy(Block => Block.File1Start).ToList();
            similarityBlocks = similarityBlocks.Where(Block => !Block.Straddles).ToList();

            Console.ReadKey();



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
            }



            return fileComponents;

        }
    }
}
