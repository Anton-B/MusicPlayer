using System;
using System.Linq;
using MusicPlayerAPI;

namespace MusicPlayer
{
    public class SongsManager
    {
        public Song[] CreateRandomList(Song[] songs)
        {
            var rand = new Random();
            return songs.OrderBy(f => rand.Next(0, songs.Length)).ToArray();
        }

        public Song[] SortSongs(Song[] songs, int index)
        {
            switch (index)
            {
                case 0:
                    return songs.OrderByDescending(s => s.CreationTime).ToArray();
                case 1:
                    return songs.OrderBy(s => s.Title).ToArray();
                case 2:
                    return songs.OrderBy(s => s.Artist).ToArray();
            }
            return null;
        }
    }
}
