using System;
using System.Linq;
using System.IO;
using TagLib;
using MusicPlayerAPI;

namespace MusicPlayer
{
    public class SongsManager
    {
        public Song[] GetList(string path) //temporary
        {
            string[] mp3FilesPath = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            Song[] songs = new Song[mp3FilesPath.Length];
            for (int i = 0; i < mp3FilesPath.Length; i++)
            {
                using (FileStream fs = new FileStream(mp3FilesPath[i], FileMode.Open))
                {
                    var tagFile = TagLib.File.Create(new StreamFileAbstraction(mp3FilesPath[i], fs, fs));
                    songs[i] = new Song();
                    songs[i].Path = mp3FilesPath[i];
                    songs[i].Title = (tagFile.Tag.Title == null) ? "[Без имени]" : tagFile.Tag.Title;
                    songs[i].Artist = (tagFile.Tag.FirstPerformer == null) ? "[Без имени]" : tagFile.Tag.FirstPerformer;
                    songs[i].Album = (tagFile.Tag.Album == null) ? "[Без имени]" : tagFile.Tag.Album;
                    songs[i].Duration = TimeFormatter.Format((int)tagFile.Properties.Duration.Minutes) + ":" + TimeFormatter.Format((int)tagFile.Properties.Duration.Seconds);
                }
            }
            return songs;
        }

        public Song[] CreateRandomList(Song[] songs)
        {
            var rand = new Random();
            return songs.OrderBy(f => rand.Next(0, songs.Length)).ToArray();
        }
    }
}
