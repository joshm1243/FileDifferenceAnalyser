using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class Block
    {


        public List<Block> subBlocks { get; set; }
        
        public bool ContainsSubBlock { get; set; }

        public int File1Start { get; set; }

        public int File2Start { get; set; }

        public int Length { get; set; }

    }
}
