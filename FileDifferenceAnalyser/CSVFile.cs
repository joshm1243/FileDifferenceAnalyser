using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileDifferenceAnalyser
{
    class CSVFile : Repository, IReadable
    {

        //Override for the base class 'Read' method
        public override string[] Read()
        {
            string fileContent = File.ReadAllText(this.FilePath);
            string[] fileContentArray = fileContent.Split(',');
            return fileContentArray;
        }

        //Override for the base class 'Write' method
        public override void Write()
        {

        }
    }
}
