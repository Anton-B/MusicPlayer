using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TagLib;
using MusicPlayerAPI;

namespace MusicPlayer
{
    public static class Player
    {
        public static MainWindow mainWindow { get; set; }
        private static Song[] songs { get; set; }
        private static Song[] randSongs { get; set; }
        public static bool IsPlaying { get; set; } = false;
        private static bool IsRandomPlaying { get; set; } = false;
        public static int repeatValue = 0;

        public static Song[] GetSongs()
        {
            if (IsRandomPlaying)
                return randSongs;
            else
                return songs;
        }

        public static void GetList(string path)
        {
            string[] mp3FilesPath = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            songs = new Song[mp3FilesPath.Length];
            for (int i = 0; i < mp3FilesPath.Length; i++)
            {
                FileStream fs = new FileStream(mp3FilesPath[i], FileMode.Open);
                var tagFile = TagLib.File.Create(new StreamFileAbstraction(mp3FilesPath[i], fs, fs));
                songs[i] = new Song();
                songs[i].Path = mp3FilesPath[i];
                songs[i].Title = (tagFile.Tag.Title == null) ? "[Без имени]" : tagFile.Tag.Title;
                songs[i].Artist = (tagFile.Tag.FirstPerformer == null) ? "[Без имени]" : tagFile.Tag.FirstPerformer;
                songs[i].Album = (tagFile.Tag.Album == null) ? "[Без имени]" : tagFile.Tag.Album;
                songs[i].Duration = mainWindow.FormatTime((int)tagFile.Properties.Duration.Minutes) + ":" + mainWindow.FormatTime((int)tagFile.Properties.Duration.Seconds);
                fs.Close();
            }
            ExtractDataToShow(songs);
            mainWindow.CreateMediaList();
        }

        private static void ExtractDataToShow(Song[] songArray)
        {
            var dataToShow = from s in songArray
                             select new { s.Title, s.Artist, s.Duration };
            mainWindow.songsDataGrid.ItemsSource = dataToShow;
        }

        public static void Play()
        {
            mainWindow.mediaElement.Play();
            mainWindow.btPlayPause.Content = "Pause";
            IsPlaying = true;
        }

        public static void Pause()
        {
            mainWindow.mediaElement.Pause();
            mainWindow.btPlayPause.Content = "Play";
            IsPlaying = false;
        }

        public static void Prev()
        {
            if (mainWindow.songsDataGrid.SelectedIndex > 0)
                mainWindow.songsDataGrid.SelectedIndex--;
        }

        public static void Next()
        {
            if (repeatValue == 1)
            {
                mainWindow.mediaElement.Position = new TimeSpan(0, 0, 0, 0);
            }
            else
            {
                if (mainWindow.songsDataGrid.SelectedIndex != mainWindow.songsDataGrid.Items.Count - 1)
                    mainWindow.songsDataGrid.SelectedIndex++;
                else if (repeatValue == 2)
                    mainWindow.songsDataGrid.SelectedIndex = 0;
            }
            Play();     
            mainWindow.songsDataGrid.ScrollIntoView(mainWindow.songsDataGrid.SelectedItem);
        }

        public static void MixSongs()
        {
            IsRandomPlaying = !IsRandomPlaying;
            if (IsRandomPlaying)
            {
                CreateRandomList();                          
                mainWindow.btRand.Content = "+Rand";
            }
            else
                mainWindow.btRand.Content = "-Rand";
            ExtractDataToShow(GetSongs());
            mainWindow.CreateMediaList();            
            mainWindow.songsDataGrid.SelectedIndex++;
            mainWindow.songsDataGrid.SelectedIndex--;
            Pause();
        }

        private static void CreateRandomList()
        {
            randSongs = new Song[songs.Length];
            int[] newIndArray = new int[songs.Length];
            for (int i = 0; i < newIndArray.Length; i++)
                newIndArray[i] = i;
            var rand = new Random();
            newIndArray = (from i in newIndArray
                           orderby rand.Next()
                           select i).ToArray();
            for (int i = 0; i < newIndArray.Length; i++)
                randSongs[i] = songs[newIndArray[i]];
        }
    }
}
