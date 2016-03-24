using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net;
using MusicPlayerAPI;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace VKPlugin
{
    internal class VKAudio
    {
        private const string clientID = "5193448";
        private const string redirectUrl = "https://oauth.vk.com/blank.html";
        private const string display = "popup";
        private const string scope = "friends,audio,groups,offline";
        private const string responseType = "token";
        private const string APIVersion = "5.50";        
        private const string lang = "ru";
        private const string friendsOrder = "hints";
        private const string friendsFields = "photo_50";
        private const string groupsInfoType = "1";
        private const string autoComplete = "1";
        private const string lyrics = "0";
        private const string performerOnly = "0";
        private const string sort = "2";
        private const string searchOwn = "1";
        private const string count = "300";
        private string userID;
        private string accessToken;
        private const string cacheFolder = @"Plugins\VKPlugin\vkcache\";
        private const string friendsFolder = @"Plugins\VKPlugin\vkcache\friends\";
        private const string groupsFolder = @"Plugins\VKPlugin\vkcache\groups\";
        private const string linksFileName = @"\ids.links";
        private readonly string addButtonImageSource;
        private readonly string deleteButtonImageSource;
        private List<NavigationItem> favorites;
        internal string UserID { get { return userID; } }
        private string AccessToken { get { return accessToken; } }
        internal bool HasAccessData { get { return !string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(AccessToken); } }
        internal string AuthUrl { get { return string.Format(
            "https://oauth.vk.com/authorize?client_id={0}&display={1}&redirect_uri={2}&scope={3}&response_type={4}&v={5}&lang={6}",
            clientID, display, redirectUrl, scope, responseType, APIVersion, lang); } }
        private string FriendsUrl { get { return string.Format(
            "https://api.vk.com/method/friends.get?order={0}&fields={1}&lang={2}&v={3}&access_token={4}",
            friendsOrder, friendsFields, lang, APIVersion, AccessToken); } }
        private string GroupsUrl { get { return string.Format(
            "https://api.vk.com/method/groups.get?extended={0}&lang={1}&v={2}&access_token={3}",
            groupsInfoType, lang, APIVersion, accessToken); } }
        private string PlaylistsUrl { get { return string.Format(
            "https://api.vk.com/method/audio.getAlbums?lang={0}&v={1}&access_token={2}",
            lang, APIVersion, accessToken); } }
        private string AudioListUrl { get { return "https://api.vk.com/method/audio.get?{0}&lang={1}&v={2}&access_token={3}"; } }
        private string AudioSearchUrl { get { return 
        "https://api.vk.com/method/audio.search?q={0}&auto_complete={1}&lyrics={2}&performer_only={3}&sort={4}&search_own={5}&count={6}&lang={7}&v={8}&access_token={9}";
        } }
        private string AddAudioUrl { get { return string.Format(
            "https://api.vk.com/method/audio.add?audio_id={0}&owner_id={1}&v={2}&access_token={3}",
            "{0}", "{1}", APIVersion, accessToken); } }
        private string DeleteAudioUrl { get { return string.Format(
            "https://api.vk.com/method/audio.delete?audio_id={0}&owner_id={1}&v={2}&access_token={3}",
            "{0}", userID, APIVersion, accessToken); } }
        private string AudioLyricsUrl { get { return string.Format(
            "https://api.vk.com/method/audio.getLyrics?lyrics_id={0}&lang={1}&v={2}&access_token={3}",
            "{0}", lang, APIVersion, accessToken); } }
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        internal VKAudio() { }
        internal VKAudio(string addButtonImageSource, string deleteButtonImageSource, List<NavigationItem> favorites)
        {
            this.addButtonImageSource = addButtonImageSource;
            this.deleteButtonImageSource = deleteButtonImageSource;
            this.favorites = favorites;
        }

        private enum RequestedListType { Friends, Groups }

        private class ResponseList
        {
            public string id;
            public string name;
            public string first_name;
            public string last_name;
            public string title;
            public string photo_50;
        }

        private class ResponseAudio
        {
            public string id;
            public string owner_id;
            public string title;
            public string artist;
            public int duration;
            public string url;
            public long date;
            public int lyrics_id;
        }

        internal void GetAccessData(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.Contains("access_token") || !url.Contains("user_id"))
                return;
            const int idIndex = 5;
            const int tokenIndex = 1;
            char[] symbol = { '=', '&' };
            string[] strData = url.Split(symbol);
            userID = strData[idIndex];
            accessToken = strData[tokenIndex];
        }

        internal async Task<List<NavigationItem>> GetFriendsList()
        {
            return await GetList(FriendsUrl, friendsFolder, RequestedListType.Friends);
        }

        internal async Task<List<NavigationItem>> GetGroupsList()
        {
            return await GetList(GroupsUrl, groupsFolder, RequestedListType.Groups);
        }

        internal async Task<List<NavigationItem>> GetPlaylistsList()
        {
            using (var client = new WebClient())
            {
                List<NavigationItem> list = new List<NavigationItem>();
                var responseItems = await GetResponseItems(client, PlaylistsUrl);
                if (responseItems == null)
                    return null;
                foreach (var res in responseItems)
                {
                    var item = new NavigationItem(res.title, "album_id=" + res.id, 50, false, true, null,
                        16, new SolidColorBrush(Color.FromRgb(43, 88, 122)), System.Windows.Input.Cursors.Hand);
                    item.AddRemoveFavoriteImageSource = Environment.CurrentDirectory + "\\" + (favorites.Contains(item) ? deleteButtonImageSource : addButtonImageSource);
                    list.Add(item);
                }                    
                return list;
            }
        }

        private async Task<List<NavigationItem>> GetList(string requestUrl, string cacheFolderPath, RequestedListType listType)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                Dictionary<string, string> idsDict = new Dictionary<string, string>();
                Directory.CreateDirectory(cacheFolderPath);
                using (var streamR = new StreamReader(new FileStream(cacheFolderPath + linksFileName, FileMode.OpenOrCreate)))
                {
                    string[] lines = streamR.ReadToEnd().Split('\n');
                    foreach (string line in lines)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(line))
                                continue;
                            string[] data = line.Split(' ', '\r');
                            idsDict.Add(data[0], data[1]);
                        }
                        catch { }
                    }
                }

                List<NavigationItem> list = new List<NavigationItem>();
                var responseItems = await GetResponseItems(client, requestUrl);
                if (responseItems == null)
                    return null;
                foreach (var res in responseItems)
                {
                    res.id = (listType == RequestedListType.Friends) ? res.id : ("-" + res.id);
                    if (idsDict.ContainsKey(res.id))
                    {
                        if (!res.photo_50.Equals(idsDict[res.id]))
                        {
                            File.Delete(cacheFolderPath + res.id + ".jpg");
                            await client.DownloadFileTaskAsync(res.photo_50, cacheFolderPath + res.id + ".jpg");
                        }
                    }
                    else
                        await client.DownloadFileTaskAsync(res.photo_50, cacheFolderPath + res.id + ".jpg");
                    idsDict[res.id] = res.photo_50;
                    var item = new NavigationItem((listType == RequestedListType.Groups) ? res.name : (res.first_name + " " + res.last_name), "owner_id=" + res.id, 50, false,
                        true, null, 16, new SolidColorBrush(Color.FromRgb(43, 88, 122)), System.Windows.Input.Cursors.Hand, Environment.CurrentDirectory + "\\" + cacheFolderPath + res.id + ".jpg");
                    item.AddRemoveFavoriteImageSource = Environment.CurrentDirectory + "\\" + (favorites.Contains(item) ? deleteButtonImageSource : addButtonImageSource);
                    list.Add(item);
                }

                using (var streamW = new StreamWriter(new FileStream(cacheFolderPath + linksFileName, FileMode.OpenOrCreate)))
                    foreach (var kvp in idsDict)
                        streamW.WriteLine(kvp.Key + " " + kvp.Value);
                return list;
            }
        }

        private async Task<List<ResponseList>> GetResponseItems(WebClient client, string requestUrl)
        {
            try
            {
                var jsonResponseStr = await client.DownloadStringTaskAsync(requestUrl);
                var vkResponse = new { response = new { count = 0, items = new List<ResponseList>() } };
                var resp = JsonConvert.DeserializeAnonymousType(jsonResponseStr, vkResponse);
                return resp.response.items;
            }            
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.", 
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        internal async Task<List<Song>> GetAudioList(NavigationItem item)
        {
            try
            {
                List<Song> songs = new List<Song>();
                if (accessToken == null)
                    return songs;
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string url;
                    if (item == null)
                        url = string.Format(AudioListUrl, string.Empty, lang, APIVersion, accessToken);
                    else
                        url = string.Format(AudioListUrl, item.Path, lang, APIVersion, accessToken);

                    var jsonResponseStr = await client.DownloadStringTaskAsync(url);
                    var vkResponse = new { response = new { count = 0, items = new List<ResponseAudio>() } };
                    var resp = JsonConvert.DeserializeAnonymousType(jsonResponseStr, vkResponse);
                    if (resp == null || resp.response == null || resp.response.count == 0)
                        return songs;
                    var index = resp.response.count - 1;
                    foreach (var res in resp.response.items)
                    {
                        songs.Add(new Song(res.owner_id + "_" + res.id, res.title, res.artist, TimeFormatter.Format(res.duration / 60)
                            + ":" + TimeFormatter.Format(res.duration % 60), res.lyrics_id.ToString(), res.url, index));
                        index--;
                    }                        
                    return songs;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        internal async Task<List<Song>> GetSearchResponse(string request)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    List<Song> songs = new List<Song>();
                    var jsonResponseStr = await client.DownloadStringTaskAsync(string.Format(AudioSearchUrl, request, autoComplete,
                        lyrics, performerOnly, sort, searchOwn, count, lang, APIVersion, accessToken));
                    var vkResponse = new { response = new { count = 0, items = new List<ResponseAudio>() } };
                    var resp = JsonConvert.DeserializeAnonymousType(jsonResponseStr, vkResponse);
                    if (resp == null || resp.response == null || resp.response.count == 0)
                        return songs;
                    foreach (var res in resp.response.items)
                        songs.Add(new Song(res.owner_id + "_" + res.id, res.title, res.artist, TimeFormatter.Format(res.duration / 60)
                            + ":" + TimeFormatter.Format(res.duration % 60), res.lyrics_id.ToString(), res.url, res.date));
                    return songs;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        internal async Task<bool> AddAudio(string songID)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var ids = songID.Split('_');
                    var jsonResponseSte = await client.DownloadStringTaskAsync(string.Format(AddAudioUrl, ids[1], ids[0]));
                    var vkResponse = new { response = 0 };
                    var resp = JsonConvert.DeserializeAnonymousType(jsonResponseSte, vkResponse);
                    return resp.response == 0 ? false : true;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                   "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        internal async Task<bool> DeleteAudio(string songID)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var jsonResopnseStr = await client.DownloadStringTaskAsync(string.Format(DeleteAudioUrl, songID.Split('_')[1]));
                    var vkResponse = new { response = 0 };
                    var resp = JsonConvert.DeserializeAnonymousType(jsonResopnseStr, vkResponse);
                    return resp.response == 1 ? true : false;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                   "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        internal async Task<string> GetAudioLyrics(int lyricsID)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var jsonResponseStr = await client.DownloadStringTaskAsync(string.Format(AudioLyricsUrl, lyricsID));
                    var vkResponse = new { response = new { text = string.Empty } };
                    var resp = JsonConvert.DeserializeAnonymousType(jsonResponseStr, vkResponse);
                    return resp.response == null ? null : resp.response.text;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        internal async Task<bool> DownloadAudio(string audioUrl, string fullFileName)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    await client.DownloadFileTaskAsync(audioUrl, fullFileName);
                    return true;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Ошибка подключения. Пожалуйста, проверьте подключение к интернету.",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        internal void LogOut()
        {
            userID = null;
            accessToken = null;
            if (InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0))
                System.Diagnostics.Process.Start("cmd.exe", "/C RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 255");
        }
    }
}
