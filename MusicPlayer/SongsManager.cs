using System;
using System.Linq;
using System.IO;
using TagLib;
using MusicPlayerAPI;

namespace MusicPlayer
{
    public class SongsManager
    {
        private Song[] songs { get; set; }
        private Song[] randSongs { get; set; }
        public Song[] Songs { get { return IsMixed ? randSongs : songs; } }
        public bool IsMixed { get; set; }

        public void GetList(string path) //temporary
        {
            string[] mp3FilesPath = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            songs = new Song[mp3FilesPath.Length];
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
        }

        public void CreateRandomList()
        {
            var rand = new Random();
            randSongs = songs.OrderBy(f => rand.Next(0, songs.Length)).ToArray();
        }
    }
}
