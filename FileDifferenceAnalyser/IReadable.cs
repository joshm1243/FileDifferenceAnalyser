using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    interface IReadable
    {

        string Name { get; }
        bool Open(string path);
        string[] Read();
        List<LineComponent> FindDifference(string[] secondFile);
    }
}
