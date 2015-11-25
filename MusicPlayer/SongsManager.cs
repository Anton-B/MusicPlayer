using System;
using System.Linq;
using System.IO;
using TagLib;
using MusicPlayerAPI;

namespace MusicPlayer
{
    public class SongsManager
    {
        public MainWindow MainWindow { get; set; }
        private Song[] songs { get; set; }
        private Song[] randSongs { get; set; }
        public Song[] Songs { get { return isMixed ? randSongs : songs; } }
        private bool isMixed { get; set; }

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
            ExtractDataToShow(songs);
            MainWindow.CreateMediaList();
        }

        public void ExtractDataToShow(Song[] songArray)
        {
            var dataToShow = from s in songArray
                             select new { s.Title, s.Artist, s.Duration };
            MainWindow.songsDataGrid.ItemsSource = dataToShow;
        }

        public void MixSongs()
        {
            isMixed = !isMixed;
            if (isMixed)
            {
                CreateRandomList();
                MainWindow.btRand.Content = "+Rand";
            }
            else
                MainWindow.btRand.Content = "-Rand";
            ExtractDataToShow(Songs);
            MainWindow.CreateMediaList();
        }

        private void CreateRandomList()
        {
            var rand = new Random();
            randSongs = songs.OrderBy(f => rand.Next(0, songs.Length)).ToArray();
        }
    }
}
