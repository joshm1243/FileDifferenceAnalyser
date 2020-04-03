using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileDifferenceAnalyser
{

    class TXTFile : Repository, IReadable
    {
        //Override for the base class 'Read' method
        public override string[] Read()
        {
            string fileContent = File.ReadAllText(this.FilePath);
            string[] fileContentArray = fileContent.Split('\n');

            for (int i = 0; i < fileContentArray.Length - 1; i++)
            {
                fileContentArray[i] = fileContentArray[i].Remove(fileContentArray[i].Length - 1);
            }

            //Removes the last line containing the \n character
            List<string> temp = new List<string>(fileContentArray);
            temp.RemoveAt(temp.Count() - 1);
            fileContentArray = temp.ToArray();

            return fileContentArray;
        }

        //Override for the base class 'Write' method
        public override void Write()
        {

        }
    }
}
