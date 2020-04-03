using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class Block
    {
        private int _file1start = -1;
        public int File1Start { get; set; }

        private int _file2start = -1;
        public int File2Start { get; set; }

        private int _blockLength = -1;
        public int BlockLength { get; set; }

        private bool _straddles = false;
        public bool Straddles { get; set; }


    }
}
