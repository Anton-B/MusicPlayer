using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private const double itemHeight = 36;
        private const double fontHeight = 14;
        private const string friendsImageSource = @"Plugins\VKPlugin\Images\friends.png";
        private const string groupsImageSource = @"Plugins\VKPlugin\Images\groups.png";
        private const string playlistsImageSource = @"Plugins\VKPlugin\Images\playlists.png";
        private string friendsPath;
        private string groupsPath;
        private string playlistsPath;

        public List<NavigationItem> GetNavigationItems(string path)
        {
            List<NavigationItem> navigItems = new List<NavigationItem>();
            if (path == null)
            {
                navigItems.Add(new NavigationItem("Друзья", friendsPath, itemHeight, true, false, friendsImageSource, fontHeight, Cursors.Hand));
                navigItems.Add(new NavigationItem("Группы", groupsPath, itemHeight, true, false, groupsImageSource, fontHeight, Cursors.Hand));
                navigItems.Add(new NavigationItem("Плейлисты", playlistsPath, itemHeight, true, false, playlistsImageSource, fontHeight, Cursors.Hand));
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
