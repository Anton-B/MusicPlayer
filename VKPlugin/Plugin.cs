using System;
using System.Collections.Generic;
using MusicPlayerAPI;
using System.Windows.Input;

namespace VKPlugin
{
    public class Plugin : IPlugin
    {
        public string Name { get; } = "Музыка с vk.com";
        public string[] TabItemHeaders { get; } = { "Выбрать музыку", "Избранное" };
        public string AddButtonImageSource { get; } = @"Plugins\VKPlugin\Images\add.png";
        public string DeleteButtonImageSource { get; } = @"Plugins\VKPlugin\Images\delete.png";
        public List<NavigationItem> FavoriteItems { get; set; } = new List<NavigationItem>();
        public bool DoubleClickToOpenItem { get; }
        private VKAudio vkAudio = new VKAudio();
        private bool userLogged = false;
        private const double itemHeight = 50;
        private const double fontHeight = 14;
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
        private string userAudio = "Мои аудиозаписи";
        private string friendsPath = "Друзья";
        private string groupsPath = "Группы";
        private string playlistsPath = "Плейлисты";

        public List<NavigationItem> GetNavigationItems(string path)
        {
            List<NavigationItem> navigItems = new List<NavigationItem>();
            if (path == null && !userLogged)
            {
                navigItems.Add(new NavigationItem("Вход", loginPath, itemHeight, true, false, loginImageSource, fontHeight, Cursors.Arrow));
            }
            else if (path == loginPath && userLogged)
            {
                if (!isCacheDownloaded)
                {
                    vkAudio.GetFriendsList();
                    vkAudio.GetGroupsList();
                    //TODO: vkAudio.GetPlaylistsList();
                    isCacheDownloaded = true;
                }
                navigItems.Add(new NavigationItem("Мои аудиозаписи", userAudio, itemHeight, false, true, audioImageSource, fontHeight, Cursors.Arrow));
                navigItems.Add(new NavigationItem("Друзья", friendsPath, itemHeight, true, false, friendsImageSource, fontHeight, Cursors.Arrow));
                navigItems.Add(new NavigationItem("Группы", groupsPath, itemHeight, true, false, groupsImageSource, fontHeight, Cursors.Arrow));
                navigItems.Add(new NavigationItem("Плейлисты", playlistsPath, itemHeight, true, false, playlistsImageSource, fontHeight, Cursors.Arrow));
                navigItems.Add(new NavigationItem("Выход", logoutPath, itemHeight, true, false, logoutImageSource, fontHeight, Cursors.Arrow));
            }
            else if (path == loginPath && !userLogged)
            {
                browserWin = new BrowserWindow(vkAudio);
                browserWin.Navigate(vkAudio.AuthUrl);
                bool? result = browserWin.Show();
                if (vkAudio.HasAccessData)
                {
                    userLogged = true;
                    navigItems = GetNavigationItems(loginPath);
                }
                else
                    navigItems = GetNavigationItems(null);
            }
            else if (path == logoutPath)
            {
                vkAudio.LogOut();
                userLogged = false;
                navigItems = GetNavigationItems(null);
            }
            else if (path == friendsPath)
                navigItems = vkAudio.GetFriendsList();
            else if (path == groupsPath)
                navigItems = vkAudio.GetGroupsList();
            else if (path == playlistsPath)
            {

            }
            return navigItems;
        }

        public void AddToFavorites(NavigationItem item)
        {

        }

        public void DeleteFromFavorites(NavigationItem item)
        {

        }

        public Song[] GetSongsList()
        {
            List<Song> songsList = new List<Song>();

            return songsList.ToArray();
        }
    }
}
