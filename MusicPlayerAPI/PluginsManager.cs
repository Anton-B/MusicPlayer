using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MusicPlayerAPI
{
    public class PluginsManager
    {
        public Dictionary<string, IPlugin> PluginInstasnces { get; set; } = new Dictionary<string, IPlugin>();
        public string Key { get; set; }
        public int OpenedTabIndex { get { return (Key == null) ? 0 : PluginInstasnces[Key].OpenedTabIndex; } set { PluginInstasnces[Key].OpenedTabIndex = value; } }
        public bool UseDefaultHomeButton { get { return PluginInstasnces[Key].UseDefaultHomeButton; } }
        public bool UseDefaultSearch { get { return PluginInstasnces[Key].UseDefaultSearch; } }
        public bool DoubleClickToOpenItem { get { return PluginInstasnces[Key].DoubleClickToOpenItem; } }
        public bool SortSearchResults { get { return PluginInstasnces[Key].SortSearchResults; } }
        public bool UpdatePlaylistWhenFavoritesChanges { get { return PluginInstasnces[Key].UpdatePlaylistWhenFavoritesChanges; } }        

        public void LoadPlugin(DirectoryInfo directory)
        {
            try
            {
                var manifestFile = directory.GetFiles("*.manifest").FirstOrDefault();
                if (manifestFile != null)
                {
                    string[] manifestContent = File.ReadAllText(manifestFile.FullName).Split(',');
                    string assemblyName = manifestContent[0].Trim();
                    string pluginClassName = manifestContent[1].Trim();
                    Assembly assembly = Assembly.LoadFrom(Directory.GetFiles(manifestFile.DirectoryName, assemblyName + ".dll").FirstOrDefault());
                    Type type = assembly.GetType(pluginClassName);
                    PluginInstasnces.Add(assemblyName, Activator.CreateInstance(type) as IPlugin);
                }
                var dirs = directory.GetDirectories();
                foreach (var dir in dirs)
                    LoadPlugin(dir);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddToFavorites(NavigationItem item)
        {
            PluginInstasnces[Key].AddToFavorites(item);
        }

        public void DeleteFromFavorites(NavigationItem item)
        {
            PluginInstasnces[Key].DeleteFromFavorites(item);
        }

        public string GetHeader(int index)
        {
            return PluginInstasnces[Key].TabItemHeaders[index];
        }

        public async Task<List<NavigationItem>> GetNavigationItems(string path)
        {
            try
            {
                return await PluginInstasnces[Key].GetNavigationItems(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<NavigationItem> GetFavoriteItems()
        {
            return PluginInstasnces[Key].FavoriteItems;
        }

        public bool IsFavorite(NavigationItem item)
        {
            foreach (var ni in PluginInstasnces[Key].FavoriteItems)
                if (ni.Equals(item))
                    return true;
            return false;
        }

        public string GetItemButtonImage(NavigationItem item)
        {
            return (IsFavorite(item)) ? PluginInstasnces[Key].DeleteButtonImageSource
                : PluginInstasnces[Key].AddButtonImageSource;
        }

        public async Task<Song[]> GetDefaultSongsList()
        {
            return await PluginInstasnces[Key].GetDefaultSongsList();
        }

        public async Task<Song[]> GetSongsList(NavigationItem item)
        {
            return await PluginInstasnces[Key].GetSongsList(item);
        }

        public async Task<Song[]> GetSearchResponse(string request)
        {
            return await PluginInstasnces[Key].GetSearchResponse(request);
        }

        public async Task<Song[]> GetHomeButtonSongs()
        {
            return await PluginInstasnces[Key].GetHomeButtonSongs();
        }
    }
}
