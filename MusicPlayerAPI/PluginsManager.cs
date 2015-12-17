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
        public bool DoubleClickToOpenItem { get { return PluginInstasnces[Key].DoubleClickToOpenItem; } }

        public void LoadPlugin(DirectoryInfo directory)
        {
            FileInfo dllFile = directory.GetFiles("*.dll").FirstOrDefault();
            FileInfo manifestFile = directory.GetFiles("*.manifest").FirstOrDefault();
            if (dllFile != null && manifestFile != null)
            {
                Assembly assembly = Assembly.LoadFrom(dllFile.FullName);
                string pluginClassName = File.ReadAllText(manifestFile.FullName);
                Type type = assembly.GetType(pluginClassName);
                PluginInstasnces.Add(dllFile.Name.Replace(".dll", string.Empty), Activator.CreateInstance(type) as IPlugin);
            }
            DirectoryInfo[] dirs = directory.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
                LoadPlugin(dir);
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<NavigationItem> GetFavoriteItems()
        {
            return PluginInstasnces[Key].FavoriteItems;
        }

        public bool IsFavorite(NavigationItem item)
        {
            foreach (NavigationItem ni in PluginInstasnces[Key].FavoriteItems)
                if (ni.Path.Equals(item.Path))
                    return true;
            return false;
        }

        public string GetItemButtonImage(NavigationItem item)
        {
            return (IsFavorite(item)) ? PluginInstasnces[Key].DeleteButtonImageSource
                : PluginInstasnces[Key].AddButtonImageSource;
        }

        public Song[] GetSongs()
        {
            return PluginInstasnces[Key].GetSongsList();
        }
    }
}
