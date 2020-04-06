using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class Block
    {

        public static int GetBlockGroupLength(List<Block> similarityBlocks, int startBlock, int endBlock)
        {

            int blockGroupTotal = 0;

            for (int i = startBlock; i <= endBlock; i++)
            {
                if (!similarityBlocks[i].Straddles)
                {
                    blockGroupTotal += similarityBlocks[i].BlockLength;
                }
            }

            return blockGroupTotal;
        }



        public static int GetBlockForIndex(List<Block> similarityBlocks, int fileToSearch, int index)
        {
            for (int i = 0; i < similarityBlocks.Count(); i++)
            {
                if (!similarityBlocks[i].Removed)
                {
                    if (fileToSearch == 1)
                    {
                        if (index >= similarityBlocks[i].File1Start && index < similarityBlocks[i].File1Start + similarityBlocks[i].BlockLength)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        if (index >= similarityBlocks[i].File2Start && index < similarityBlocks[i].File2Start + similarityBlocks[i].BlockLength)
                        {
                            return i;
                        }
                    }

                }
            }

            return -1;
        }

  


        private int _file1start = -1;
        public int File1Start { get; set; }

        private int _file2start = -1;
        public int File2Start { get; set; }

        private int _blockLength = -1;
        public int BlockLength { get; set; }

        private bool _straddles = false;
        public bool Straddles { get; set; }

        public bool Removed { get; set; }

        public int Key { get; set; }


    }
}
