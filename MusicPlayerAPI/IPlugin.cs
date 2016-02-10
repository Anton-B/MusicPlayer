using System.Collections.Generic;
using System.Windows.Input;

namespace MusicPlayerAPI
{
    public interface IPlugin
    {
        string Name { get; }
        string[] TabItemHeaders { get; }
        string AddButtonImageSource { get; }
        string DeleteButtonImageSource { get; }
        bool UseDefaultHomeButton { get; }
        bool UseDefaultSearch { get; }
        bool DoubleClickToOpenItem { get; }
        bool SortSearchResults { get; }
        bool UpdatePlaylistWhenFavoritesChanges { get; }
        List<NavigationItem> FavoriteItems { get; }

        List<NavigationItem> GetNavigationItems(string path);
        void AddToFavorites(NavigationItem item);
        void DeleteFromFavorites(NavigationItem item);
        Song[] GetDefaultSongsList();
        Song[] GetSongsList(NavigationItem item);
        Song[] GetSearchResponse(string request);
        Song[] GetHomeButtonSongs();
    }
}
