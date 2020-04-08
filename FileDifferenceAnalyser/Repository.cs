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


        private Block GetLongestSublist(List<string> list1, List<string> list2, int index1Start, int index1End, int index2Start, int index2End)
        {

            Block largestBlock = new Block();

            largestBlock.Length = 0;


            int file1Pos;
            int file2Pos;
            int length;

            for (int i = index1Start; i < index1End; i++)
            {

                for (int j = index2Start; j < index2End; j++)
                {

                    file1Pos = i;
                    file2Pos = j;
                    length = 0;

                    while (file1Pos < index1End && file2Pos < index2End)
                    {
                        if (list1[file1Pos] == list2[file2Pos])
                        {
                            file1Pos++;
                            file2Pos++;
                            length++; ;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (length > largestBlock.Length)
                    {
                        largestBlock.File1Start = i;
                        largestBlock.File2Start = j;
                        largestBlock.Length = length;
                    }
                }
            }

            return largestBlock;
        }

        private List<Block> GetSimilarities(List<string> list1, List<string> list2, int index1Start, int index1End, int index2Start, int index2End, bool recLevel, int granularity)
        {

            List<Block> similarityBlocks = new List<Block>();

            Block currentSimilarity;

            int recentFile1Occurence = -1;
            int recentFile2Occurence = -1;

            List<string> sentence1;
            List<string> sentence2;

            currentSimilarity = GetLongestSublist(list1, list2, index1Start, index1End, index2Start, index2End);


            if (currentSimilarity.Length == 0)
            {

                


                if (!recLevel)
                {


                    for (int i = index1Start; i < index1End; i++)
                    {
                        for (int j = index2Start; j < index2End; j++)
                        {

                            if (i > recentFile1Occurence && j > recentFile2Occurence)
                            {

                                if (granularity == 0)
                                {
                                    sentence1 = list1[i].Split(' ').ToList();
                                    sentence2 = list2[j].Split(' ').ToList();
                                }
                                else
                                {
                                    sentence1 = list1[i].Select(curChar => curChar.ToString()).ToList();
                                    sentence2 = list2[j].Select(curChar => curChar.ToString()).ToList();
                                }


                                List<Block> sentenceSimilarities = GetSimilarities(sentence1,sentence2,true,granularity);


                                if (sentenceSimilarities.Count() > 0)
                                {

                                    Block newSentence = new Block();
                                    newSentence.File1Start = i;
                                    newSentence.File2Start = j;
                                    newSentence.Length = 1;
                                    newSentence.ContainsSubBlock = true;

                                    newSentence.subBlocks = sentenceSimilarities;

                                    similarityBlocks.Add(newSentence);

                                    recentFile1Occurence = i;
                                    recentFile2Occurence = j;

                              
                                }


                            }
                        }
                    }

                }


                return similarityBlocks;
            }


            similarityBlocks.Add(currentSimilarity);

            List<Block> upperHalf;
            List<Block> lowerHalf;

            upperHalf = GetSimilarities(list1, list2, index1Start, currentSimilarity.File1Start, index2Start, currentSimilarity.File2Start,false,granularity);
            lowerHalf = GetSimilarities(list1, list2, currentSimilarity.File1Start + currentSimilarity.Length, index1End, currentSimilarity.File2Start + currentSimilarity.Length, index2End,false,granularity);

            similarityBlocks.AddRange(upperHalf);
            similarityBlocks.AddRange(lowerHalf);

            return similarityBlocks;
                       
        }

        public List<Block> GetSimilarities(List<string> list1, List<string> list2, int granularity)
        {
            return GetSimilarities(list1, list2, 0, list1.Count(), 0, list2.Count(),false, granularity);
        }

        public List<Block> GetSimilarities(List<string> list1, List<string> list2, bool phraseLevel, int granularity)
        {
            return GetSimilarities(list1, list2, 0, list1.Count(), 0, list2.Count(), phraseLevel, granularity);
        }



        public List<Line> GetLines(List<Block> similarityBlocks, List<string> list1, List<string> list2, bool sentenceLevel, int granularity)
        {

            List<Line> lines = new List<Line>();
            
   

            similarityBlocks = similarityBlocks.OrderBy(Block => Block.File1Start).ToList();



            int lastFile1BlockEnd = -1;
            int lastFile2BlockEnd = -1;


            //

            List<string> sentence1;
            List<string> sentence2;

            for (int i = 0; i < similarityBlocks.Count(); i++)
            {



                if (i == 0)
                {


                    for (int j = 0; j < similarityBlocks[i].File1Start; j++)
                    {

                        Line l = new Line();

                        l.File1Number = j;

                        Phrase p = new Phrase();


                        p.Variant = -1;
                        p.Text = list1[j];




                        l.Phrases.Add(p);

                        lines.Add(l);

                    }

                    for (int j = 0; j < similarityBlocks[i].File2Start; j++)
                    {
                        Line l = new Line();

                        l.File2Number = j;

                        Phrase p = new Phrase();

                        p.Text = list2[j];

                        p.Variant = 1;

                        l.Phrases.Add(p);

                        lines.Add(l);

                    }

                }



                if (!sentenceLevel && similarityBlocks[i].ContainsSubBlock)
                {

                    Line l = new Line();

                    l.File1Number = similarityBlocks[i].File1Start;
                    l.File2Number = similarityBlocks[i].File2Start;


                    if (granularity == 0)
                    {
                        sentence1 = list1[similarityBlocks[i].File1Start].Split(' ').ToList();
                        sentence2 = list2[similarityBlocks[i].File2Start].Split(' ').ToList();
                    }
                    else
                    {
                        sentence1 = list1[similarityBlocks[i].File1Start].Select(curChar => curChar.ToString()).ToList();
                        sentence2 = list2[similarityBlocks[i].File2Start].Select(curChar => curChar.ToString()).ToList();
                    }

                    foreach (Line a in GetLines(similarityBlocks[i].subBlocks, sentence1, sentence2, true, granularity))
                    {


                        foreach (Phrase b in a.Phrases)
                        {
                            Phrase tempPhrase = new Phrase();
                            tempPhrase.Variant = b.Variant;
                            tempPhrase.Text = b.Text;



                            if (granularity == 0)
                            {
                                tempPhrase.Text += " ";
                            }

                            l.Phrases.Add(tempPhrase);
                        }
                    }

                    lines.Add(l);

       
                }
                else
                {


                    if (i != 0)
                    {

                        for (int j = similarityBlocks[i - 1].File1Start + similarityBlocks[i - 1].Length; j < similarityBlocks[i].File1Start; j++)
                        {
                            Line l = new Line();

                            l.File1Number = j;

                            Phrase p = new Phrase();

                            p.Variant = -1;
                            p.Text = list1[j];



                            l.Phrases.Add(p);

                            lines.Add(l);


                        }

                        for (int j = similarityBlocks[i - 1].File2Start + similarityBlocks[i - 1].Length; j < similarityBlocks[i].File2Start; j++)
                        {
                            Line l = new Line();

                            l.File1Number = j;

                            Phrase p = new Phrase();

                            p.Variant = 1;
                            p.Text = list2[j];

                            l.Phrases.Add(p);

                            lines.Add(l);

                        }
                    }

                    //Adds the current similarity block
                    for (int j = 0; j < (similarityBlocks[i].File1Start + similarityBlocks[i].Length) - similarityBlocks[i].File1Start; j++)
                    {



                        Line l = new Line();

                        l.File1Number = similarityBlocks[i].File1Start + j;
                        l.File2Number = similarityBlocks[i].File2Start + j;

                        Phrase p = new Phrase();
                        p.Variant = 0;

                       
                        p.Text = list1[similarityBlocks[i].File1Start + j];
                        l.Phrases.Add(p);

                        lines.Add(l);



                    }



                }
            }



            if (similarityBlocks.Count() == 0)
            {
                similarityBlocks.Add(new Block { File1Start = -1, File2Start = -1, Length = 1 });
            }

            lastFile1BlockEnd = similarityBlocks[similarityBlocks.Count() - 1].File1Start + similarityBlocks[similarityBlocks.Count() - 1].Length;
            lastFile2BlockEnd = similarityBlocks[similarityBlocks.Count() - 1].File2Start + similarityBlocks[similarityBlocks.Count() - 1].Length;


            for (int i = lastFile1BlockEnd; i < list1.Count(); i++)
            {

    

                Line l = new Line();

                l.File1Number = i;

                Phrase p = new Phrase();
                p.Variant = -1;
                p.Text = list1[i];
                l.Phrases.Add(p);

                lines.Add(l);

            }

            for (int i = lastFile2BlockEnd; i < list2.Count(); i++)
            {
                Line l = new Line();

                l.File2Number = i;

                Phrase p = new Phrase();
                p.Variant = 1;
                p.Text = list2[i];
                l.Phrases.Add(p);

                lines.Add(l);

            }

            return lines;

        }


        public List<Line> GetLines(List<Block> similarityBlocks, List<string> list1, List<string> list2, int granularity)
        {
            return GetLines(similarityBlocks, list1, list2, false, granularity);
        }


        public List<Line> FindDifference(string[] file2Array, int granularity)
        {

            List<string> file1Content = this.Read().ToList<string>();
            List<string> file2Content = file2Array.ToList<string>();

            List<Block> similarityBlocks = GetSimilarities(file1Content, file2Content, granularity);


            foreach (Block b in similarityBlocks)
            {
                Console.WriteLine(b.File1Start + " : " + b.File2Start + " : " + b.Length);


                if (b.ContainsSubBlock)
                {
                    foreach (Block c in b.subBlocks)
                    {
                        Console.WriteLine("\t" + c.File1Start + " : " + c.File2Start + " : " + c.Length);


                    }
                }
            }



            Console.ReadKey();

            List<Line> lines = GetLines(similarityBlocks, file1Content, file2Content, granularity);

            return lines;

        }
    }
}
