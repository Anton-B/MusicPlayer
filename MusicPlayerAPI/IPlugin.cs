using System.Collections.Generic;

namespace MusicPlayerAPI
{
    public interface IPlugin
    {
        string Name { get; }
        List<string> SelectedMusicPaths { get; set; }

        List<NavigationItem> GetItems(string path);
        Song[] GetSongsList();
    }
}
