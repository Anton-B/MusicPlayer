using System.Collections.Generic;
using System.Linq;
using System.IO;
using MusicPlayerAPI;
using TagLib;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media;
using System;
using System.Windows;

namespace FileSystemPlugin
{
    public class FSPlugin : IPlugin
    {
        public string Name { get; } = "Музыка с компьютера";
        public string[] TabItemHeaders { get; } = { "Выбор музыки", "Выбранные папки" };
        public string AddButtonImageSource { get; } = @"Plugins\FileSystemPlugin\Images\add.png";
        public string DeleteButtonImageSource { get; } = @"Plugins\FileSystemPlugin\Images\delete.png";
        public int OpenedTabIndex { get; set; }
        public bool SupportsSongMenuButton { get { return true; } }
        public bool UseDefaultHomeButton { get { return false; } }
        public bool UseDefaultSearch { get { return true; } }
        public bool DoubleClickToOpenItem { get { return true; } }
        public bool SortSearchResults { get { return true; } }
        public bool UpdatePlaylistWhenFavoritesChanges { get { return true; } }
        public List<NavigationItem> FavoriteItems { get; private set; } = new List<NavigationItem>();
        private const string deleteSongMenuItem = "Удалить аудиозапись с компьютера";
        private const double itemHeight = 24;
        private const double fontHeight = 12;
        private const string diskImageSource = @"Plugins\FileSystemPlugin\Images\disk.png";
        private const string folderImageSource = @"Plugins\FileSystemPlugin\Images\folder.png";
        private const string favoriteImageSource = @"Plugins\FileSystemPlugin\Images\favorite_folder.png";
        private const string audioImageSource = @"Plugins\FileSystemPlugin\Images\audio.png";
        private const string parentFolderImageSource = @"Plugins\FileSystemPlugin\Images\parent_folder.png";
        private Song[] lastLoadedSongs;

        public void SetThemeSettings(bool darkThemeIsUsing) { }

        public async Task<List<NavigationItem>> GetNavigationItems(string path)
        {
            try
            {
                List<NavigationItem> navigItems = new List<NavigationItem>();
                if (path == null)
                {
                    var drives = DriveInfo.GetDrives();
                    foreach (var drive in drives)
                        navigItems.Add(new NavigationItem((drive.IsReady) ? drive.Name : (drive.Name + " [Отсутствует]"), drive.Name,
                            itemHeight, true, false, null, fontHeight, Cursors.Arrow, Environment.CurrentDirectory + "\\" + diskImageSource));
                }
                else
                {
                    var parent = Directory.GetParent(path);
                    navigItems.Add(new NavigationItem("[Назад]", parent?.FullName, itemHeight,
                        true, false, null, fontHeight, Cursors.Arrow, Environment.CurrentDirectory + "\\" + parentFolderImageSource));
                    DirectoryInfo di = new DirectoryInfo(path);
                    var dirs = di.GetDirectories();
                    foreach (var dir in dirs)
                    {
                        var item = new NavigationItem(dir.Name, dir.FullName, itemHeight,
                            true, true, null, fontHeight, Cursors.Arrow, Environment.CurrentDirectory + "\\" + folderImageSource);
                        item.AddRemoveFavoriteImageSource = Environment.CurrentDirectory + "\\" + (FavoriteItems.Contains(item) ? DeleteButtonImageSource : AddButtonImageSource);
                        navigItems.Add(item);
                    }

                    var files = di.GetFiles("*.mp3", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                        navigItems.Add(new NavigationItem(file.Name.Replace(".mp3", string.Empty), file.FullName, itemHeight,
                            false, false, null, fontHeight, Cursors.Arrow, Environment.CurrentDirectory + "\\" + audioImageSource));
                }
                return navigItems;
            }
            catch
            {
                MessageBox.Show("Ошибка доступа к директории", "", MessageBoxButton.OK, MessageBoxImage.Error);
                var list = new List<NavigationItem>();
                list.Add(new NavigationItem("[Назад]", Directory.GetParent(path)?.FullName, itemHeight,
                        true, false, null, fontHeight, Cursors.Arrow, Environment.CurrentDirectory + "\\" + parentFolderImageSource));
                return list;
            }
        }

        public void AddToFavorites(NavigationItem item)
        {
            item.ImageSource = Environment.CurrentDirectory + "\\" + favoriteImageSource;
            item.AddRemoveFavoriteImageSource = Environment.CurrentDirectory + "\\" + DeleteButtonImageSource;
            FavoriteItems.Add(item);
        }

        public void DeleteFromFavorites(NavigationItem item)
        {
            item.AddRemoveFavoriteImageSource = Environment.CurrentDirectory + "\\" + AddButtonImageSource;
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
                                Song song = new Song(mp3FilePaths[i], (tagFile.Tag.Title == null) ? "[Без имени]" : tagFile.Tag.Title,
                                                     (tagFile.Tag.FirstPerformer == null) ? "[Без имени]" : tagFile.Tag.FirstPerformer,
                                                     TimeFormatter.Format((int)tagFile.Properties.Duration.Minutes)
                                                     + ":" + TimeFormatter.Format((int)tagFile.Properties.Duration.Seconds),
                                                     tagFile.Tag.Lyrics,
                                                     mp3FilePaths[i],
                                                     new FileInfo(mp3FilePaths[i]).CreationTime.Ticks);                                
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

        public async Task<Song[]> GetMyMusicSongs()
        {
            return lastLoadedSongs;
        }

        public async Task<List<string>> GetSongMenuItems(Song song)
        {
            return new List<string>() { deleteSongMenuItem };
        }

        public async Task<UpdateBehavior> HandleMenuItemClick(string itemText, Song song)
        {
            if (itemText == deleteSongMenuItem)
            {
                var answer = MessageBox.Show(string.Format("Вы уверены, что хотите безвозвратно удалить аудиозапись \"{0}\" с вашего компьютера?",
                    new FileInfo(song.Path).Name), "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (answer == MessageBoxResult.Yes)
                {
                    try
                    {
                        await Task.Run(() => System.IO.File.Delete(song.Path));
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("Возникла ошибка при попытке удаления файла.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return UpdateBehavior.NoUpdate;
                    }
                    var songs = lastLoadedSongs.ToList();
                    songs.Remove(song);
                    lastLoadedSongs = songs.ToArray();
                    MessageBox.Show("Аудиозапись успешно удалена с компьютера!", "", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return UpdateBehavior.DeleteSongAndUpdateAll;
                }
            }
            return UpdateBehavior.NoUpdate;
        }
    }
}
