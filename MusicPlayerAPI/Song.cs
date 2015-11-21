using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerAPI
{
    public class Song
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Duration { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Artist, Title);
        }
    }
}
