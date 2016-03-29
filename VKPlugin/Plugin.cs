using System;
using System.Collections.Generic;
using MusicPlayerAPI;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Net;
using System.Windows;
using System.IO;

namespace VKPlugin
{
    public class Plugin : IPlugin
    {
        public string Name { get; } = "Музыка из ВКонтакте";
        public string[] TabItemHeaders { get; } = { "Выбор музыки", "Избранное" };
        public string AddButtonImageSource { get; } = @"Plugins\VKPlugin\Images\add.png";
        public string DeleteButtonImageSource { get; } = @"Plugins\VKPlugin\Images\delete.png";
        public int OpenedTabIndex { get; set; }
        public bool SupportsSongMenuButton { get { return true; } }
        public bool UseDefaultHomeButton { get { return true; } }
        public bool UseDefaultSearch { get { return false; } }
        public bool DoubleClickToOpenItem { get { return false; } }
        public bool SortSearchResults { get { return false; } }
        public bool UpdatePlaylistWhenFavoritesChanges { get { return false; } }
        public List<NavigationItem> FavoriteItems { get; private set; } = new List<NavigationItem>();
        private VKAudio vkAudio;
        private bool userLogged = false;
        private const string addSongMenuItem = "Добавить в мои аудиозаписи";
        private const string deleteSongMenuItem = "Удалить из моих аудиозаписей";
        private const string lyricsMenuItem = "Просмотреть текст песни";
        private const string downloadSongMenuItem = "Скачать аудиозапись";
        private const double itemHeight = 50;
        private const double fontHeight = 14;
        private BrowserWindow browserWin;
        private bool isCacheDownloaded;
        private string loginPath = "Вход";
        private string logoutPath = "Выполните вход";
        private string friendsPath = "Мои друзья";
        private string groupsPath = "Мои группы";
        private string playlistsPath = "Мои плейлисты";
        private Brush lyricsWindowBackground = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
        private Brush lyricsWindowForeground = (Brush)new BrushConverter().ConvertFromString("#FF000000");

        public Plugin() { vkAudio = new VKAudio(AddButtonImageSource, DeleteButtonImageSource, FavoriteItems); }

        public void SetThemeSettings(bool darkThemeIsUsing)
        {
            if (darkThemeIsUsing == true)
            {
                lyricsWindowBackground = (Brush)new BrushConverter().ConvertFromString("#FF1E1E1E");
                lyricsWindowForeground = (Brush)new BrushConverter().ConvertFromString("#FFE6E6E6");
            }
            else
            {
                lyricsWindowBackground = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
                lyricsWindowForeground = (Brush)new BrushConverter().ConvertFromString("#FF000000");
            }                
        }

        public async Task<List<NavigationItem>> GetNavigationItems(string path)
        {
            List<NavigationItem> navigItems = new List<NavigationItem>();
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadString("http://google.com");
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                navigItems.Add(new NavigationItem("Вход", null, itemHeight, true, false, null, fontHeight, Cursors.Hand));
                return navigItems;
            }
            if (path == null && !userLogged)
            {
                browserWin = new BrowserWindow(vkAudio);
                browserWin.Navigate(vkAudio.AuthUrl);
                bool? result = browserWin.Show();
                if (vkAudio.HasAccessData)
                {
                    userLogged = true;
                    navigItems = await GetNavigationItems(null);
                }
                else
                    navigItems = await GetNavigationItems(loginPath);
            }
            else if (path == loginPath)
            {
                navigItems.Add(new NavigationItem("Вход", null, itemHeight, true, false, null, fontHeight, Cursors.Hand));
            }
            else if (path == null && userLogged)
            {
                if (!isCacheDownloaded)
                {
                    if (FavoriteItems.Count > 0 && FavoriteItems[0].Name != "Мои аудиозаписи" || FavoriteItems.Count == 0)
                        FavoriteItems.Add(new NavigationItem("Мои аудиозаписи", vkAudio.UserID, itemHeight, false, false, null, fontHeight, Cursors.Hand));
                    await vkAudio.GetFriendsList();
                    await vkAudio.GetGroupsList();
                    isCacheDownloaded = true;
                }
                navigItems.Add(new NavigationItem("Мои аудиозаписи", vkAudio.UserID, itemHeight, false, false, null, fontHeight, Cursors.Hand));
                navigItems.Add(new NavigationItem("Мои друзья", friendsPath, itemHeight, true, false, null, fontHeight, Cursors.Hand));
                navigItems.Add(new NavigationItem("Мои группы", groupsPath, itemHeight, true, false, null, fontHeight, Cursors.Hand));
                navigItems.Add(new NavigationItem("Мои плейлисты", playlistsPath, itemHeight, true, false, null, fontHeight, Cursors.Hand));
                navigItems.Add(new NavigationItem("Выход", logoutPath, itemHeight, true, false, null, fontHeight, Cursors.Hand, true, "Вы уверены что хотите выйти из своего аккаунта?"));
            }
            else if (path == logoutPath)
            {
                vkAudio.LogOut();
                var cleanedFavorites = new List<NavigationItem>();
                cleanedFavorites.Add(FavoriteItems[0]);
                FavoriteItems = cleanedFavorites;
                userLogged = false;
                navigItems = await GetNavigationItems(loginPath);
            }
            else
            {
                navigItems.Add(new NavigationItem("[Назад]", null, 50, true, false, null, 16, Cursors.Hand));
                var resultList = new List<NavigationItem>();
                if (path == friendsPath)
                    resultList = await vkAudio.GetFriendsList();
                else if (path == groupsPath)
                    resultList = await vkAudio.GetGroupsList();
                else if (path == playlistsPath)
                    resultList = await vkAudio.GetPlaylistsList();
                if (resultList == null)
                    return null;
                navigItems.AddRange(resultList);
            }
            return navigItems;
        }

        public void AddToFavorites(NavigationItem item)
        {
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
            var list = await vkAudio.GetAudioList(null);
            if (list == null)
                return null;
            return list.ToArray();
        }

        public async Task<Song[]> GetSongsList(NavigationItem item)
        {
            var list = await vkAudio.GetAudioList(item);
            if (list == null)
                return null;
            return list.ToArray();
        }

        public async Task<Song[]> GetSearchResponse(string request)
        {
            var list = await vkAudio.GetSearchResponse(request);
            if (list == null)
                return null;
            return list.ToArray();
        }

        public async Task<Song[]> GetMyMusicSongs()
        {
            return await GetDefaultSongsList();
        }

        public async Task<List<string>> GetSongMenuItems(Song song)
        {
            var items = new List<string>();
            var list = await vkAudio.GetAudioList(null);
            if (list == null)
                return null;
            foreach (var s in list)
                if (s.Path == song.Path)
                {
                    items.Add(deleteSongMenuItem);
                    break;
                }
            if (items.Count == 0)
                items.Add(addSongMenuItem);
            if (song.Lyrics != "0" && song.Lyrics != null)
                items.Add(lyricsMenuItem);
            items.Add(downloadSongMenuItem);
            return items;
        }

        public async Task<UpdateBehavior> HandleMenuItemClick(string itemText, Song song)
        {
            switch (itemText)
            {
                case addSongMenuItem:
                    if (await vkAudio.AddAudio(song.ID))
                        MessageBox.Show("Аудиозапись добавлена.", "",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case deleteSongMenuItem:
                    var answer = MessageBox.Show(string.Format("Вы уверены, что хотите удалить аудиозапись \"{0}\"?",
                    string.Format("{0} - {1}", song.Artist, song.Title)), "", MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.No);
                    if (answer == MessageBoxResult.Yes)
                    {
                        if (await vkAudio.DeleteAudio(song.ID))
                            MessageBox.Show("Аудиозапись удалена.", "",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case lyricsMenuItem:
                    ShowLyricsWindow(string.Format("{0} - {1}", song.Artist, song.Title),
                        await vkAudio.GetAudioLyrics(Convert.ToInt32(song.Lyrics)));
                    break;
                case downloadSongMenuItem:
                    var parentDir = Directory.GetParent(Environment.CurrentDirectory);
                    var dir = Directory.CreateDirectory(parentDir.FullName + @"\Загрузки MusicPlayer");
                    if (await vkAudio.DownloadAudio(song.Path,
                        string.Format(@"{0}\{1} - {2}.mp3", dir.FullName, song.Artist, song.Title)))
                        MessageBox.Show(string.Format("Аудиофайл успешно загружен в папку\n{0}", dir.FullName), "",
                             MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Возникла ошибка при загрузке файла.",
                            "", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
            return UpdateBehavior.NoUpdate;
        }

        private void ShowLyricsWindow(string title, string lyrics)
        {
            var lyricsWindow = new Window();
            lyricsWindow.Width = 360;
            lyricsWindow.Height = 660;
            lyricsWindow.ResizeMode = ResizeMode.NoResize;
            lyricsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            lyricsWindow.Title = title;
            lyricsWindow.Background = lyricsWindowBackground;
            lyricsWindow.Icon = new System.Windows.Media.Imaging.BitmapImage(
                new Uri(Environment.CurrentDirectory + @"\Plugins\VKPlugin\Images\faviconnew.ico"));
            var sw = new System.Windows.Controls.ScrollViewer();
            sw.Margin = new Thickness(0, 0, 0, 10);
            var textBlock = new System.Windows.Controls.TextBlock();
            textBlock.Margin = new Thickness(20, 20, 0, 20);
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.Text = lyrics;
            textBlock.Background = lyricsWindow.Background;
            textBlock.Foreground = lyricsWindowForeground;
            sw.Content = textBlock;
            lyricsWindow.Content = sw;
            lyricsWindow.Show();
        }
    }
}
