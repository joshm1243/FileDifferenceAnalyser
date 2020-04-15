using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    interface IWritable
    {
        string FilePath { get; set; }
        bool Create(string directoryPath);
        string[] CreateDiffForFile(int granularity, string file1Name, string file2Name, List<Line> lines = null);
        bool Write(string[] fileContents);
        List<Line> FindDifference(List<string> secondFile, int granularity);
    }
}
