﻿using System.Collections.Generic;
using System.Linq;
using System.IO;
using MusicPlayerAPI;
using TagLib;
using System.Windows.Input;
using System.Threading.Tasks;

namespace FileSystemPlugin
{
    public class FSPlugin : IPlugin
    {
        public string Name { get; } = "Музыка с компьютера";
        public string[] TabItemHeaders { get; } = { "Выбрать музыку", "Выбранное" };
        public string AddButtonImageSource { get; } = @"Plugins\FileSystemPlugin\Images\add.png";
        public string DeleteButtonImageSource { get; } = @"Plugins\FileSystemPlugin\Images\delete.png";
        public int OpenedTabIndex { get; set; }
        public bool UseDefaultHomeButton { get { return false; } }
        public bool UseDefaultSearch { get { return true; } }
        public bool DoubleClickToOpenItem { get { return true; } }
        public bool SortSearchResults { get { return true; } }
        public bool UpdatePlaylistWhenFavoritesChanges { get { return true; } }
        public List<NavigationItem> FavoriteItems { get; private set; } = new List<NavigationItem>();
        private const double itemHeight = 24;
        private const double fontHeight = 12;
        private const string diskImageSource = @"Plugins\FileSystemPlugin\Images\disk.png";
        private const string folderImageSource = @"Plugins\FileSystemPlugin\Images\folder.png";
        private const string favoriteImageSource = @"Plugins\FileSystemPlugin\Images\favorite_folder.png";
        private const string audioImageSource = @"Plugins\FileSystemPlugin\Images\audio.png";
        private const string parentFolderImageSource = @"Plugins\FileSystemPlugin\Images\parent_folder.png";
        private Song[] lastLoadedSongs;

        public async Task<List<NavigationItem>> GetNavigationItems(string path)
        {
            List<NavigationItem> navigItems = new List<NavigationItem>();
            if (path == null)
            {
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                    navigItems.Add(new NavigationItem((drive.IsReady) ? drive.Name : (drive.Name + " [Отсутствует]"), drive.Name,
                        itemHeight, true, false, fontHeight, Cursors.Arrow, diskImageSource));
            }
            else
            {
                var parent = Directory.GetParent(path);
                navigItems.Add(new NavigationItem("[Назад]", parent?.FullName, itemHeight, true, false, fontHeight, Cursors.Arrow, parentFolderImageSource));
                DirectoryInfo di = new DirectoryInfo(path);
                var dirs = di.GetDirectories();
                foreach (var dir in dirs)
                    navigItems.Add(new NavigationItem(dir.Name, dir.FullName, itemHeight, true, true, fontHeight, Cursors.Arrow, folderImageSource));
                var files = di.GetFiles("*.mp3", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                    navigItems.Add(new NavigationItem(file.Name.Replace(".mp3", string.Empty), file.FullName, itemHeight, false, false, fontHeight, Cursors.Arrow, audioImageSource));
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

        public async Task<Song[]> GetDefaultSongsList()
        {
            List<Song> allSongsList = new List<Song>();
            try
            {
                foreach (var ni in FavoriteItems)
                {
                    var mp3FilePaths = await Task.Run(() => GetFiles(ni.Path, "*.mp3").ToArray());
                    List<Song> songs = new List<Song>();
                    for (int i = 0; i < mp3FilePaths.Length; i++)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(mp3FilePaths[i], FileMode.Open))
                            {
                                var tagFile = TagLib.File.Create(new StreamFileAbstraction(mp3FilePaths[i], fs, fs));
                                Song song = new Song();
                                song.Path = mp3FilePaths[i];
                                song.Title = (tagFile.Tag.Title == null) ? "[Без имени]" : tagFile.Tag.Title;
                                song.Artist = (tagFile.Tag.FirstPerformer == null) ? "[Без имени]" : tagFile.Tag.FirstPerformer;
                                song.Duration = TimeFormatter.Format((int)tagFile.Properties.Duration.Minutes)
                                    + ":" + TimeFormatter.Format((int)tagFile.Properties.Duration.Seconds);
                                FileInfo file = new FileInfo(mp3FilePaths[i]);
                                song.CreationTime = file.CreationTime.Ticks;
                                songs.Add(song);
                            }
                        }
                        catch { }
                    }
                    allSongsList.AddRange(songs);
                }
            }
            catch { }
            IEqualityComparer<Song> comparer = new Song() as IEqualityComparer<Song>;
            var songsList = allSongsList.Distinct(comparer);
            lastLoadedSongs = songsList.ToArray();
            return lastLoadedSongs;
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
            catch { }
            return files;
        }

        public async Task<Song[]> GetSongsList(NavigationItem item)
        {
            return new Song[0];
        }

        public async Task<Song[]> GetSearchResponse(string request)
        {
            return new Song[0];
        }

        public async Task<Song[]> GetHomeButtonSongs()
        {
            return lastLoadedSongs;
        }
    }
}
