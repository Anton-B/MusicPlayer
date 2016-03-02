using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net;
using MusicPlayerAPI;
using System.IO;
using System.Threading.Tasks;

namespace VKPlugin
{
    internal class VKAudio
    {
        private const string clientID = "5193448";
        private const string redirectUrl = "https://oauth.vk.com/blank.html";
        private const string display = "popup";
        private const string scope = "friends,audio,groups,offline";
        private const string responseType = "token";
        private const string APIVersion = "5.45";        
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
        private const string cacheFolder = @"vkcache\";
        private const string friendsFolder = @"vkcache\friends\";
        private const string groupsFolder = @"vkcache\groups\";
        private const string linksFileName = @"\ids.links";
        private const string playlistImageSource = @"Plugins\VKPlugin\Images\playlist.png";
        internal string UserID { get { return userID; } }
        internal string AccessToken { get { return accessToken; } }
        internal bool HasAccessData { get { return !string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(AccessToken); } }
        internal string AuthUrl { get { return string.Format(
            "https://oauth.vk.com/authorize?client_id={0}&display={1}&redirect_uri={2}&scope={3}&response_type={4}&v={5}&lang={6}",
            clientID, display, redirectUrl, scope, responseType, APIVersion, lang); } }
        internal string FriendsUrl { get { return string.Format(
            "https://api.vk.com/method/friends.get?order={0}&fields={1}&lang={2}&v={3}&access_token={4}",
            friendsOrder, friendsFields, lang, APIVersion, AccessToken); } }
        internal string GroupsUrl { get { return string.Format(
            "https://api.vk.com/method/groups.get?extended={0}&lang={1}&v={2}&access_token={3}",
            groupsInfoType, lang, APIVersion, accessToken); } }
        internal string PlaylistsUrl { get { return string.Format(
            "https://api.vk.com/method/audio.getAlbums?lang={0}&v={1}&access_token={2}",
            lang, APIVersion, accessToken); } }
        internal string AudioListUrl { get { return "https://api.vk.com/method/audio.get?{0}{1}&lang={2}&v={3}&access_token={4}"; } }
        internal string AudioSearchUrl { get { return 
        "https://api.vk.com/method/audio.search?q={0}&auto_complete={1}&lyrics={2}&performer_only={3}&sort={4}&search_own={5}&count={6}&lang={7}&v={8}&access_token={9}";
        } }
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

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
            public string title;
            public string artist;
            public int duration;
            public string url;
            public long date;
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
                foreach (var res in await GetResponseItems(client, PlaylistsUrl))
                    list.Add(new NavigationItem(res.title, res.id, 50, false, true,
                        16, System.Windows.Input.Cursors.Arrow, playlistImageSource));
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
                foreach (var res in await GetResponseItems(client, requestUrl))
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
                    list.Add(new NavigationItem((listType == RequestedListType.Groups) ? res.name : (res.first_name + " " + res.last_name), res.id, 50, false,
                        true, 16, System.Windows.Input.Cursors.Arrow, cacheFolderPath + res.id + ".jpg"));
                }

                using (var streamW = new StreamWriter(new FileStream(cacheFolderPath + linksFileName, FileMode.OpenOrCreate)))
                    foreach (var kvp in idsDict)
                        streamW.WriteLine(kvp.Key + " " + kvp.Value);
                return list;
            }
        }

        private async Task<List<ResponseList>> GetResponseItems(WebClient client, string requestUrl)
        {
            var jsonResponseStr = await client.DownloadStringTaskAsync(requestUrl);
            var vkResponse = new { response = new { count = 0, items = new List<ResponseList>() } };
            var resp = JsonConvert.DeserializeAnonymousType(jsonResponseStr, vkResponse);
            return resp.response.items;
        }

        internal async Task<List<Song>> GetAudioList(NavigationItem item)
        {
            List<Song> songs = new List<Song>();
            if (accessToken == null)
                return songs;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string url;
                if (item == null)
                    url = string.Format(AudioListUrl, string.Empty, string.Empty, lang, APIVersion, accessToken);
                else
                    url = string.Format(AudioListUrl, (item.ImageSource == playlistImageSource) ? "album_id=" : "owner_id=",
                        item.Path, lang, APIVersion, accessToken);
                var jsonResponseStr = await client.DownloadStringTaskAsync(url);
                var vkResponse = new { response = new { count = 0, items = new List<ResponseAudio>() } };
                var resp = JsonConvert.DeserializeAnonymousType(jsonResponseStr, vkResponse);
                if (resp == null || resp.response == null || resp.response.count == 0)
                    return songs;
                foreach (var res in resp.response.items)
                    songs.Add(new Song(res.title, res.artist, TimeFormatter.Format(res.duration / 60) 
                        + ":" + TimeFormatter.Format(res.duration % 60), res.url, res.date));
                return songs;
            }
        }

        internal async Task<List<Song>> GetSearchResponse(string request)
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
                    songs.Add(new Song(res.title, res.artist, TimeFormatter.Format(res.duration / 60)
                        + ":" + TimeFormatter.Format(res.duration % 60), res.url, res.date));
                return songs;
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
