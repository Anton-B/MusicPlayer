using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net;
using MusicPlayerAPI;

namespace VKPlugin
{
    internal class VKAudio
    {
        private const string clientID = "5193448";
        private const string redirectUri = "https://oauth.vk.com/blank.html";
        private const string display = "popup";
        private const string scope = "friends,audio,groups,offline";
        private const string responseType = "token";
        private const string APIVersion = "5.44";
        private const string friendsOrder = "hints";
        private const string friendsFields = "photo_50";
        private const string lang = "ru";
        private string userID;
        private string accessToken;
        internal string UserID { get { return userID; } }
        internal string AccessToken { get { return accessToken; } }
        internal bool HasAccessData { get { return !string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(AccessToken); } }
        internal string AuthUrl { get { return string.Format(
            "https://oauth.vk.com/authorize?client_id={0}&display={1}&redirect_uri={2}&scope={3}&response_type={4}&v={5}",
            clientID, display, redirectUri, scope, responseType, APIVersion); } }
        internal string FriendsUrl { get { return string.Format(
            "https://api.vk.com/method/friends.get?order={0}&fields={1}&lang={2}&v={3}&access_token={4}",
            friendsOrder, friendsFields, lang, APIVersion, AccessToken); } }
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private class Response
        {
            public string id;
            public string first_name;
            public string last_name;
            public string photo_50;
            public int online;
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

        internal List<NavigationItem> GetFriendsList()
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string friendsStr = client.DownloadString(FriendsUrl);                
                var vkResponseList = new { response = new { count = 0, items = new List<Response>() } };
                var friends = JsonConvert.DeserializeAnonymousType(friendsStr, vkResponseList);
                List<NavigationItem> list = new List<NavigationItem>();
                list.Add(new NavigationItem("Назад", "Вход", 50, true, false, null, 16, System.Windows.Input.Cursors.Hand));
                foreach (Response res in friends.response.items)
                    list.Add(new NavigationItem(res.first_name + " " + res.last_name, res.id, 50, false, 
                        true, res.photo_50, 16, System.Windows.Input.Cursors.Hand));
                return list;
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
