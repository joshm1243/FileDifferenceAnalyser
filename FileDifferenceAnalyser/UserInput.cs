using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class UserInput
    {
        public bool CommandIsNull { get; set; }

        public string Command { get; set; }
        
        public string[] Parameters { get; set; }

        public string[] Arguments { get; set; }

        public string Error { get; set; } = "";

    }
}
