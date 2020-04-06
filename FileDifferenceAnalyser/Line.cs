using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDifferenceAnalyser
{
    class Line
    {

        private string _rawText;

        public List<Phrase> Phrases = new List<Phrase>();

        public string GetRawText()
        {

            for (int i = 0; i < Phrases.Count() - 1; i++)
            {
                _rawText += Phrases[i].Text + " ";
            }

            _rawText += Phrases[Phrases.Count() - 1].Text;

            return _rawText;

        }

    }
}
