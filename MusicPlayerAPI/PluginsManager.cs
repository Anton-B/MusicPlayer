using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace MusicPlayerAPI
{
    public class PluginsManager
    {
        public Dictionary<string, IPlugin> PluginInstasnces { get; set; } = new Dictionary<string, IPlugin>();
        public string Key { get; set; }
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

        public List<NavigationItem> GetNavigationItems(string path)
        {
            try
            {
                return PluginInstasnces[Key].GetNavigationItems(path);
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

        public Song[] GetDefaultSongsList()
        {
            return PluginInstasnces[Key].GetDefaultSongsList();
        }

        public Song[] GetSongsList(NavigationItem item)
        {
            return PluginInstasnces[Key].GetSongsList(item);
        }

        public Song[] GetSearchResponse(string request)
        {
            return PluginInstasnces[Key].GetSearchResponse(request);
        }

        public Song[] GetHomeButtonSongs()
        {
            return PluginInstasnces[Key].GetHomeButtonSongs();
        }
    }
}
