﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayerAPI
{
    public interface IPlugin
    {
        string Name { get; }
        string[] TabItemHeaders { get; }
        string AddButtonImageSource { get; }
        string DeleteButtonImageSource { get; }
        int OpenedTabIndex { get; set; }
        bool UseDefaultHomeButton { get; }
        bool UseDefaultSearch { get; }
        bool DoubleClickToOpenItem { get; }
        bool SortSearchResults { get; }
        bool UpdatePlaylistWhenFavoritesChanges { get; }
        List<NavigationItem> FavoriteItems { get; }

        Task<List<NavigationItem>> GetNavigationItems(string path);
        void AddToFavorites(NavigationItem item);
        void DeleteFromFavorites(NavigationItem item);
        Task<Song[]> GetDefaultSongsList();
        Task<Song[]> GetSongsList(NavigationItem item);
        Task<Song[]> GetSearchResponse(string request);
        Task<Song[]> GetHomeButtonSongs();
    }
}
