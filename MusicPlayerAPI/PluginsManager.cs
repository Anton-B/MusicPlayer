using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace MusicPlayerAPI
{
    public class PluginsManager
    {
        public Dictionary<string, IPlugin> PluginInstasnces { get; set; } = new Dictionary<string, IPlugin>();

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
        public List<NavigationItem> GetItems(string key, string path)
        {
            IPlugin ins;
            PluginInstasnces.TryGetValue(key, out ins);
            return ins.GetItems(path);
        }

        public Song[] GetSongs(string key)
        {
            IPlugin ins;
            PluginInstasnces.TryGetValue(key, out ins);
            return ins.GetSongsList();
        }
    }
}
