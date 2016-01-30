using System.Collections.Generic;
using System.Linq;
using System.IO;
using MusicPlayerAPI;
using TagLib;
using System.Windows.Input;
using System.Windows;

namespace FileSystemPlugin
{
    public class FSPlugin : IPlugin
    {
        public string Name { get; } = "Музыка с компьютера";
        public string[] TabItemHeaders { get; } = { "Выбрать музыку", "Выбранное" };
        public string AddButtonImageSource { get; } = @"Plugins\FileSystemPlugin\Images\add.png";
        public string DeleteButtonImageSource { get; } = @"Plugins\FileSystemPlugin\Images\delete.png";
        public bool DoubleClickToOpenItem { get; } = true;
        public List<NavigationItem> FavoriteItems { get; set; } = new List<NavigationItem>();
        private const double itemHeight = 24;
        private const double fontHeight = 12;
        private const string diskImageSource = @"Plugins\FileSystemPlugin\Images\disk.png";
        private const string folderImageSource = @"Plugins\FileSystemPlugin\Images\folder.png";
        private const string favoriteImageSource = @"Plugins\FileSystemPlugin\Images\favorite_folder.png";
        private const string audioImageSource = @"Plugins\FileSystemPlugin\Images\audio.png";
        private const string parentFolderImageSource = @"Plugins\FileSystemPlugin\Images\parent_folder.png";

        public List<NavigationItem> GetNavigationItems(string path)
        {
            List<NavigationItem> navigItems = new List<NavigationItem>();
            try
            {
                if (path == null)
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    foreach (DriveInfo drive in drives)
                        navigItems.Add(new NavigationItem((drive.IsReady) ? drive.Name : (drive.Name + " [Отсутствует]"), drive.Name,
                            itemHeight, true, true, diskImageSource, fontHeight, Cursors.Arrow));
                }
                else
                {
                    DirectoryInfo parent = Directory.GetParent(path);
                    navigItems.Add(new NavigationItem("[Назад]", parent?.FullName, itemHeight, true, false, parentFolderImageSource, fontHeight, Cursors.Arrow));
                    DirectoryInfo di = new DirectoryInfo(path);
                    DirectoryInfo[] dirs = di.GetDirectories();
                    foreach (DirectoryInfo dir in dirs)
                        navigItems.Add(new NavigationItem(dir.Name, dir.FullName, itemHeight, true, true, folderImageSource, fontHeight, Cursors.Arrow));
                    FileInfo[] files = di.GetFiles("*.mp3", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo file in files)
                        navigItems.Add(new NavigationItem(file.Name.Replace(".mp3", string.Empty), file.FullName, itemHeight, false, false, audioImageSource, fontHeight, Cursors.Arrow));
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return navigItems;
        }

        public void AddToFavorites(NavigationItem item)
        {
            item.ImageSource = favoriteImageSource;
            FavoriteItems.Add(item);
        }

        public void DeleteFromFavorites(NavigationItem item)
        {
            FavoriteItems.Remove(item);
        }

        public Song[] GetSongsList()
        {
            List<Song> allSongsList = new List<Song>();
            try
            {
                foreach (NavigationItem ni in FavoriteItems)
                {
                    string[] mp3FilePaths = GetFiles(ni.Path, "*.mp3").ToArray();
                    Song[] songs = new Song[mp3FilePaths.Length];
                    for (int i = 0; i < mp3FilePaths.Length; i++)
                    {
                        using (FileStream fs = new FileStream(mp3FilePaths[i], FileMode.Open))
                        {
                            var tagFile = TagLib.File.Create(new StreamFileAbstraction(mp3FilePaths[i], fs, fs));
                            songs[i] = new Song();
                            songs[i].Path = mp3FilePaths[i];
                            songs[i].Title = (tagFile.Tag.Title == null) ? "[Без имени]" : tagFile.Tag.Title;
                            songs[i].Artist = (tagFile.Tag.FirstPerformer == null) ? "[Без имени]" : tagFile.Tag.FirstPerformer;
                            songs[i].Album = (tagFile.Tag.Album == null) ? "[Без имени]" : tagFile.Tag.Album;
                            songs[i].Duration = TimeFormatter.Format((int)tagFile.Properties.Duration.Minutes)
                                + ":" + TimeFormatter.Format((int)tagFile.Properties.Duration.Seconds);
                            FileInfo file = new FileInfo(mp3FilePaths[i]);
                            songs[i].CreationTime = file.CreationTime.Ticks;
                        }
                    }
                    allSongsList.AddRange(songs);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            IEqualityComparer<Song> comparer = new Song() as IEqualityComparer<Song>;
            var songsList = allSongsList.Distinct(comparer);
            return songsList.ToArray();
        }

        private List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return files;
        }
    }
}
