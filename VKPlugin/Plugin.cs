using System;
using System.Collections.Generic;
using MusicPlayerAPI;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VKPlugin
{
    public class Plugin : IPlugin
    {
        public string Name { get; } = "Музыка из ВКонтакте";
        public string[] TabItemHeaders { get; } = { "Выбор музыки", "Избранное" };
        public string AddButtonImageSource { get; } = @"Plugins\VKPlugin\Images\add.png";
        public string DeleteButtonImageSource { get; } = @"Plugins\VKPlugin\Images\delete.png";
        public int OpenedTabIndex { get; set; }
        public bool UseDefaultHomeButton { get { return true; } }
        public bool UseDefaultSearch { get { return false; } }
        public bool DoubleClickToOpenItem { get { return false; } }
        public bool SortSearchResults { get { return false; } }
        public bool UpdatePlaylistWhenFavoritesChanges { get { return false; } }
        public List<NavigationItem> FavoriteItems { get; private set; } = new List<NavigationItem>();
        private VKAudio vkAudio;
        private bool userLogged = false;
        private const double itemHeight = 50;
        private const double fontHeight = 14;
        private Brush foreground = new SolidColorBrush(Color.FromRgb(43, 88, 122));
        private const string loginImageSource = @"Plugins\VKPlugin\Images\login.png";
        private const string logoutImageSource = @"Plugins\VKPlugin\Images\logout.png";
        private const string audioImageSource = @"Plugins\VKPlugin\Images\audio.png";
        private const string friendsImageSource = @"Plugins\VKPlugin\Images\friends.png";
        private const string groupsImageSource = @"Plugins\VKPlugin\Images\groups.png";
        private const string playlistsImageSource = @"Plugins\VKPlugin\Images\playlists.png";
        private BrowserWindow browserWin;
        private bool isCacheDownloaded;
        private string loginPath = "Вход";
        private string logoutPath = "Выход";
        private string friendsPath = "Друзья";
        private string groupsPath = "Группы";
        private string playlistsPath = "Плейлисты";

        public Plugin() { vkAudio = new VKAudio(AddButtonImageSource, DeleteButtonImageSource, FavoriteItems); }

        public async Task<List<NavigationItem>> GetNavigationItems(string path)
        {
            List<NavigationItem> navigItems = new List<NavigationItem>();
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
                navigItems.Add(new NavigationItem("Вход", null, itemHeight, true, false, null, fontHeight, foreground, Cursors.Hand));
            }
            else if (path == null && userLogged)
            {
                if (!isCacheDownloaded)
                {
                    if (FavoriteItems.Count > 0 && FavoriteItems[0].Name != "Мои аудиозаписи" || FavoriteItems.Count == 0)
                        FavoriteItems.Add(new NavigationItem("Мои аудиозаписи", vkAudio.UserID, itemHeight, false, false, null, fontHeight, foreground, Cursors.Hand));
                    await vkAudio.GetFriendsList();
                    await vkAudio.GetGroupsList();
                    isCacheDownloaded = true;
                }
                navigItems.Add(new NavigationItem("Мои аудиозаписи", vkAudio.UserID, itemHeight, false, false, null, fontHeight, foreground, Cursors.Hand));
                navigItems.Add(new NavigationItem("Друзья", friendsPath, itemHeight, true, false, null, fontHeight, foreground, Cursors.Hand));
                navigItems.Add(new NavigationItem("Группы", groupsPath, itemHeight, true, false, null, fontHeight, foreground, Cursors.Hand));
                navigItems.Add(new NavigationItem("Плейлисты", playlistsPath, itemHeight, true, false, null, fontHeight, foreground, Cursors.Hand));
                navigItems.Add(new NavigationItem("Выход", logoutPath, itemHeight, true, false, null, fontHeight, foreground, Cursors.Hand, true, "Вы уверены что хотите выйти из своего аккаунта?"));
            }
            else if (path == logoutPath)
            {
                vkAudio.LogOut();
                userLogged = false;
                navigItems = await GetNavigationItems(loginPath);
            }
            else
            {
                navigItems.Add(new NavigationItem("[Назад]", null, 50, true, false, null, 16, foreground, Cursors.Hand));
                if (path == friendsPath)
                    navigItems.AddRange(await vkAudio.GetFriendsList());
                else if (path == groupsPath)
                    navigItems.AddRange(await vkAudio.GetGroupsList());
                else if (path == playlistsPath)
                    navigItems.AddRange(await vkAudio.GetPlaylistsList());
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
            return (await vkAudio.GetAudioList(null)).ToArray();
        }

        public async Task<Song[]> GetSongsList(NavigationItem item)
        {
            return (await vkAudio.GetAudioList(item)).ToArray();
        }

        public async Task<Song[]> GetSearchResponse(string request)
        {
            return (await vkAudio.GetSearchResponse(request)).ToArray();
        }

        public async Task<Song[]> GetHomeButtonSongs()
        {
            return new Song[0];
        }
    }
}
