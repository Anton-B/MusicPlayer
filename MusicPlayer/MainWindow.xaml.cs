using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Reflection;
using System.Xml;
using System.Configuration;
using MusicPlayerAPI;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SongsManager songsManager = new SongsManager();
        private DispatcherTimer timer = new DispatcherTimer();
        private PluginsManager pluginsManager = new PluginsManager();
        private DirectoryInfo pluginsDirectory = new DirectoryInfo(@"Plugins\");
        private double currentSliderValue = -1;
        private bool isPlaying;
        private DataGrid busyDataGrid
        {
            get
            {
                if (playlist1DataGrid.SelectedIndex == -1 && playlist2DataGrid.SelectedIndex == -1)
                    return visibDataGrid;
                return (playlist1DataGrid.SelectedIndex != -1) ? playlist1DataGrid : playlist2DataGrid;
            }
        }
        private DataGrid freeDataGrid { get { return (playlist1DataGrid.SelectedIndex == -1) ? playlist1DataGrid : playlist2DataGrid; } }
        private DataGrid visibDataGrid { get { return (playlist1DataGrid.Visibility == Visibility.Visible) ? playlist1DataGrid : playlist2DataGrid; } }
        private int busyDGSortIndex;
        private Brush busyDGRandMode = Brushes.LightGray;
        private RepeatType repeatType;
        private ListView currentListView { get { return (navigTabControl.SelectedIndex == 0) ? navigListView : favoritesListView; } }

        #region WINDOW
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            pluginsManager.LoadPlugin(pluginsDirectory);
            SetModeComboBoxItems();
            try { LoadSettings(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            if (modeComboBox.SelectedIndex == -1 && modeComboBox.Items.Count > 0)
                modeComboBox.SelectedIndex = 0;
            //modeComboBox.SelectedIndex = 0;
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }
        #endregion

        #region PLAYER CORE
        private void Timer_Tick(object sender, EventArgs e)
        {
            musicTimelineSlider.Value = (currentSliderValue == -1) ? mediaElement.Position.TotalMilliseconds : currentSliderValue;
            currTimelinePosLabel.Content = TimeFormatter.Format((int)mediaElement.Position.TotalMinutes)
                + ":" + TimeFormatter.Format(mediaElement.Position.Seconds);
        }

        private void MusicTimelineSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(currentSliderValue);
            currentSliderValue = -1;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                musicTimelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                Song currentSong = busyDataGrid.SelectedItem as Song;
                if (currentSong != null)
                {
                    artistLabel.Content = currentSong.Artist;
                    titleLabel.Content = "-  " + currentSong.Title;
                    maxTimelinePosLabel.Content = currentSong.Duration;
                }
            }
            catch { }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
                Pause();
            else
                Play();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            RepeatType temp = repeatType;
            repeatType = RepeatType.NoRepeat;
            Next();
            repeatType = temp;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
                mediaElement.Volume = volumeSlider.Value;
        }

        private void MusicTimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicTimelineSlider.IsMouseCaptureWithin)
                currentSliderValue = musicTimelineSlider.Value;
            if (currentSliderValue == -1)
                mediaElement.Position = TimeSpan.FromMilliseconds(musicTimelineSlider.Value);
        }

        private void Rand_Click(object sender, RoutedEventArgs e)
        {
            randLabel.Foreground = (randLabel.Foreground == Brushes.LightGray) ? Brushes.White : Brushes.LightGray;
            if (visibDataGrid == busyDataGrid)
                busyDGRandMode = randLabel.Foreground;
            Song[] songs = (Song[])visibDataGrid.ItemsSource;
            SetSongsList(songs, true, true, false);
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            switch (repeatType)
            {
                case RepeatType.NoRepeat:
                    repeatType = RepeatType.RepeatSong;
                    repeatLabel.Foreground = Brushes.White;
                    repeatLabel.Content = "🔂";
                    break;
                case RepeatType.RepeatSong:
                    repeatType = RepeatType.RepeatList;
                    repeatLabel.Foreground = Brushes.White;
                    repeatLabel.Content = "🔁";
                    break;
                case RepeatType.RepeatList:
                    repeatType = RepeatType.NoRepeat;
                    repeatLabel.Foreground = Brushes.LightGray;
                    repeatLabel.Content = "🔁";
                    break;
            }
        }

        private void InitializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Play()
        {
            mediaElement.Play();
            playPauseLabel.Content = "⏸";
            isPlaying = true;
        }

        private void Pause()
        {
            mediaElement.Pause();
            playPauseLabel.Content = "⏵";
            isPlaying = false;
        }

        private void Prev()
        {
            if (busyDataGrid.SelectedIndex > 0)
                busyDataGrid.SelectedIndex--;
            if (busyDataGrid.SelectedItem != null)
                busyDataGrid.ScrollIntoView(busyDataGrid.SelectedItem);
        }

        private void Next()
        {
            if (busyDataGrid.SelectedItem == null)
                return;
            if (repeatType == RepeatType.RepeatSong)
                mediaElement.Position = new TimeSpan(0, 0, 0, 0);
            else
            {
                if (busyDataGrid.SelectedIndex != busyDataGrid.Items.Count - 1)
                    busyDataGrid.SelectedIndex++;
                else if (repeatType == RepeatType.RepeatList)
                    busyDataGrid.SelectedIndex = 0;
            }
            Play();
            busyDataGrid.ScrollIntoView(busyDataGrid.SelectedItem);
        }
        #endregion

        #region WORKING WITH PLUGINS & NAVIGATION
        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaElement.Close();
            mediaElement.Source = null;
            ClearControls();
            addressTextBox.Tag = null;
            addressTextBox.Text = string.Empty;
            pluginsManager.Key = ((TextBlock)modeComboBox.SelectedItem).Tag.ToString();
            for (int i = 0; i < navigTabControl.Items.Count; i++)
                ((TabItem)navigTabControl.Items[i]).Header = pluginsManager.GetHeader(i);
            navigTabControl.SelectedIndex = pluginsManager.OpenedTabIndex;
            ShowItems(addressTextBox.Tag?.ToString(), true);
            SetSongsList(pluginsManager.GetDefaultSongsList(), true, true, true);
        }

        private void NavigTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pluginsManager.OpenedTabIndex != navigTabControl.SelectedIndex)
            {
                pluginsManager.OpenedTabIndex = (pluginsManager.OpenedTabIndex == 0) ? 1 : 0;
                ShowItems(addressTextBox.Tag?.ToString(), false);
            }
        }

        private void NavigFavListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && pluginsManager.DoubleClickToOpenItem)
                currentListView.SelectedIndex = -1;
        }

        private void NavigListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && pluginsManager.DoubleClickToOpenItem)
                OpenItem(((ListView)sender).SelectedItem as DockPanel);
        }

        private void NavigListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !pluginsManager.DoubleClickToOpenItem)
                OpenItem((sender as DockPanel)?.Parent as DockPanel);
        }

        private void OpenItem(DockPanel outerPanel)
        {
            DockPanel innerPanel = outerPanel?.Children[outerPanel.Children.Count - 1] as DockPanel;
            if (innerPanel == null)
                return;
            NavigationItem item = innerPanel.Tag as NavigationItem;
            if (item != null && item.CanBeOpened)
                ShowItems(item.Path, true);
            else if (item != null && !pluginsManager.UpdatePlaylistWhenFavoritesChanges)
                SetSongsList(pluginsManager.GetSongsList(item), true, false, (mediaElement.Source == null) ? true : false);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ShowItems(addressTextBox.Tag?.ToString(), false);
        }

        private void ChangeFavorites(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            NavigationItem ni = (NavigationItem)((DockPanel)((Image)sender).Parent).Tag;
            if (pluginsManager.IsFavorite(ni))
                pluginsManager.DeleteFromFavorites(ni);
            else
                pluginsManager.AddToFavorites(ni);
            ShowItems(addressTextBox.Tag?.ToString(), false);
            if (pluginsManager.UpdatePlaylistWhenFavoritesChanges)
            {
                mediaElement.Close();
                mediaElement.Source = null;
                SetSongsList(pluginsManager.GetDefaultSongsList(), true, false, (mediaElement.Source == null) ? true : false);
                if (visibDataGrid.ItemsSource == null || visibDataGrid.Items.Count == 0)
                    ClearControls();
            }
        }

        private void ShowItems(string path, bool isScrollToUp)
        {
            Cursor = Cursors.Wait;
            ShowNavigationItems(path, isScrollToUp);
            ShowFavoriteItems();
            Cursor = Cursors.Arrow;
        }

        private void ShowNavigationItems(string path, bool isScrollToUp)
        {
            navigListView.Items.Clear();
            List<NavigationItem> navItems = pluginsManager.GetNavigationItems(path);
            addressTextBox.Tag = (navItems == null) ? addressTextBox.Tag?.ToString() : path;
            addressTextBox.Text = (addressTextBox.Tag == null) ? string.Empty : addressTextBox.Tag.ToString();
            if (navItems == null)
                navItems = pluginsManager.GetNavigationItems(addressTextBox.Tag?.ToString());
            foreach (var ni in navItems)
                ShowNavigationItem(navigListView, ni);
            if (navigListView.Items.Count > 0 && isScrollToUp)
                navigListView.ScrollIntoView(navigListView.Items[0]);
        }

        void ShowFavoriteItems()
        {
            favoritesListView.Items.Clear();
            List<NavigationItem> favItems = pluginsManager.GetFavoriteItems();
            if (navigTabControl.SelectedIndex == 1)
                addressTextBox.Text = pluginsManager.GetHeader(1);
            foreach (var ni in favItems)
                ShowNavigationItem(favoritesListView, ni);
            if (favoritesListView.Items.Count > 0)
                favoritesListView.ScrollIntoView(favoritesListView.Items[0]);
        }

        void ShowNavigationItem(ListView listView, NavigationItem ni)
        {
            DockPanel outerDP = new DockPanel();
            outerDP.LastChildFill = true;
            DockPanel innerDP = new DockPanel();
            DockPanel.SetDock(innerDP, Dock.Left);
            innerDP.LastChildFill = true;
            outerDP.Tag = innerDP.Tag = ni;
            innerDP.Height = ni.Height;
            innerDP.Width = listView.MinWidth;
            innerDP.Cursor = ni.CursorType;
            try
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/" + ni.ImageSource, UriKind.RelativeOrAbsolute));
                img.Height = ni.Height;
                DockPanel.SetDock(img, Dock.Left);
                innerDP.Children.Add(img);
            }
            catch { }
            Label label = new Label();
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Content = ni.Name;
            label.FontSize = ni.FontSize;
            label.Height = ni.Height;
            label.Padding = new Thickness(5, 0, 0, 0);
            innerDP.Children.Add(label);
            if (ni.CanBeFavorite)
            {
                Image imgAddDel = new Image();
                BitmapImage bmImage = new BitmapImage(new Uri("pack://siteoforigin:,,,/" + pluginsManager.GetItemButtonImage(ni), UriKind.RelativeOrAbsolute)); ;
                imgAddDel.Source = bmImage;
                imgAddDel.ToolTip = "Добавить в избранное / удалить из избранного";
                imgAddDel.Cursor = Cursors.Hand;
                imgAddDel.Width = bmImage.Width;
                imgAddDel.Height = bmImage.Height;
                imgAddDel.MouseDown += ChangeFavorites;
                DockPanel.SetDock(imgAddDel, Dock.Right);
                outerDP.Children.Add(imgAddDel);
            }
            outerDP.Children.Add(innerDP);
            listView.Items.Add(outerDP);
            innerDP.MouseDown += NavigListView_MouseDown;
        }

        private void SetModeComboBoxItems()
        {
            foreach (var kvp in pluginsManager.PluginInstasnces)
            {
                TextBlock tb = new TextBlock();
                tb.Text = kvp.Value.Name;
                tb.Tag = kvp.Key;
                modeComboBox.Items.Add(tb);
            }
        }
        #endregion

        #region WORKING WITH SONGS
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            randLabel.Foreground = Brushes.LightGray;
            if (sortComboBox.SelectedIndex == -1)
                return;
            if (visibDataGrid == busyDataGrid)
                busyDGSortIndex = sortComboBox.SelectedIndex;
            SetSongsList((Song[])visibDataGrid.ItemsSource, true, true, false);
            if (visibDataGrid != busyDataGrid)
                visibDataGrid.SelectedIndex = -1;
        }

        private void playlistDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is DataGrid) || ((DataGrid)sender).SelectedIndex == -1)
                return;
            if ((DataGrid)sender == visibDataGrid)
            {
                if (busyDataGrid != visibDataGrid)
                {
                    busyDataGrid.SelectedIndex = -1;
                    busyDGSortIndex = sortComboBox.SelectedIndex;
                }
            }
            else
                visibDataGrid.SelectedIndex = -1;
            if (busyDataGrid?.ItemsSource == null || busyDataGrid.SelectedIndex == -1)
                return;
            SetMediaElementSource();
            Play();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var input = e as KeyEventArgs;
            if (input != null && input.Key != Key.Enter || searchTextBox.Text == string.Empty)
                return;
            List<Song> resultSongs = new List<Song>();
            if (!pluginsManager.UseDefaultSearch)
                resultSongs.AddRange(pluginsManager.GetSearchResponse(searchTextBox.Text));
            else
                foreach (var currentSong in pluginsManager.GetHomeButtonSongs())
                    if (currentSong.Title.ToLower().Contains(searchTextBox.Text.ToLower())
                        || currentSong.Artist.ToLower().Contains(searchTextBox.Text.ToLower()))
                        resultSongs.Add(currentSong);
            SetSongsList(resultSongs.ToArray(), pluginsManager.SortSearchResults, false, (mediaElement.Source == null) ? true : false);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!pluginsManager.UseDefaultHomeButton)
                SetSongsList(pluginsManager.GetHomeButtonSongs(), true, false, false);
            else
                ShowBusyPlaylist();
        }

        private void ShowBusyPlaylist()
        {
            visibDataGrid.Visibility = Visibility.Collapsed;
            busyDataGrid.Visibility = Visibility.Visible;
            if (busyDGRandMode == Brushes.White)
                randLabel.Foreground = busyDGRandMode;
            else
                sortComboBox.SelectedIndex = busyDGSortIndex;
            numOfAudioTextBlock.Content = string.Format("Песен: {0}", visibDataGrid.Items.Count);
        }

        private void SetSongsList(Song[] list, bool sortList, bool changeCurrentPlaylist, bool moveToFirstSong)
        {
            if (!changeCurrentPlaylist)
            {
                if (songsManager.SortSongs((Song[])visibDataGrid.ItemsSource, 0)
                .SequenceEqual(songsManager.SortSongs(list, 0), new Song() as IEqualityComparer<Song>))
                    return;
                if (visibDataGrid != busyDataGrid && songsManager.SortSongs((Song[])busyDataGrid.ItemsSource, 0)
                    .SequenceEqual(songsManager.SortSongs(list, 0), new Song() as IEqualityComparer<Song>))
                {
                    ShowBusyPlaylist();
                    return;
                }
                busyDGSortIndex = sortComboBox.SelectedIndex;
                busyDGRandMode = randLabel.Foreground;
                busyDataGrid.Visibility = Visibility.Collapsed;
                freeDataGrid.Visibility = Visibility.Visible;
            }
            if (list == null || list.Length == 0)
                visibDataGrid.ItemsSource = new Song[0];
            else
            {
                visibDataGrid.ItemsSource = (randLabel.Foreground == Brushes.White)
                    ? songsManager.MixSongs(list) : (sortList) ? songsManager.SortSongs(list, sortComboBox.SelectedIndex) : list;
                if (moveToFirstSong)
                    MoveToFirstSong();
            }
            numOfAudioTextBlock.Content = string.Format("Песен: {0}", visibDataGrid.Items.Count);
        }

        private void ClearControls()
        {
            playlist1DataGrid.ItemsSource = playlist2DataGrid.ItemsSource = new Song[0];
            playlist1DataGrid.SelectedIndex = playlist2DataGrid.SelectedIndex = -1;
            randLabel.Foreground = Brushes.LightGray;
            numOfAudioTextBlock.Content = "Песен: 0";
            sortComboBox.SelectedIndex = 0;
            searchTextBox.Clear();
            musicTimelineSlider.Value = 0;
            currTimelinePosLabel.Content = maxTimelinePosLabel.Content = "00:00";
            artistLabel.Content = "Исполнитель";
            titleLabel.Content = "Название";
            playPauseLabel.Content = "⏵";
            isPlaying = false;
        }

        private void MoveToFirstSong()
        {
            visibDataGrid.SelectedIndex = 0;
            SetMediaElementSource();
            Pause();
        }

        private void SetMediaElementSource()
        {
            if (busyDataGrid.SelectedIndex == -1)
                return;
            mediaElement.Source = new Uri(((Song)busyDataGrid.SelectedItem).Path);
        }
        #endregion

        #region CONFIG
        private void SaveSettings()
        {
            XmlDocument doc = LoadConfigDocument();
            XmlNode settingsNode = doc.SelectSingleNode("//appSettings");
            if (settingsNode != null)
                doc.SelectSingleNode("//configuration").RemoveChild(settingsNode);
            settingsNode = doc.CreateNode(XmlNodeType.Element, "appSettings", "");
            doc.LastChild.AppendChild(settingsNode);
            List<string> keys = new List<string> {  "Window.Left",
                                                    "Window.Top",
                                                    "Window.Width",
                                                    "Window.Height",
                                                    "Window.WindowState",
                                                    "Plugins.Key" };
            List<string> values = new List<string> {this.Left.ToString(),
                                                    this.Top.ToString(),
                                                    this.Width.ToString(),
                                                    this.Height.ToString(),
                                                    this.WindowState.ToString(),
                                                    pluginsManager.Key };
            string str = "{0}.FavoriteItems.{1}.{2}";
            foreach (var instance in pluginsManager.PluginInstasnces)
            {
                keys.Add(string.Format("{0}.{1}", instance.Key, "OpenedTabIndex"));
                values.Add(instance.Value.OpenedTabIndex.ToString());
                for (int i = 0; i < instance.Value.FavoriteItems.Count; i++)
                {
                    keys.Add(string.Format(str, instance.Key, i, "Name"));
                    keys.Add(string.Format(str, instance.Key, i, "Path"));
                    keys.Add(string.Format(str, instance.Key, i, "Height"));
                    keys.Add(string.Format(str, instance.Key, i, "CanBeOpened"));
                    keys.Add(string.Format(str, instance.Key, i, "CanBeFavorite"));
                    keys.Add(string.Format(str, instance.Key, i, "ImageSource"));
                    keys.Add(string.Format(str, instance.Key, i, "FontSize"));
                    keys.Add(string.Format(str, instance.Key, i, "CursorType"));
                    values.Add(instance.Value.FavoriteItems[i].Name);
                    values.Add(instance.Value.FavoriteItems[i].Path);
                    values.Add(instance.Value.FavoriteItems[i].Height.ToString());
                    values.Add(instance.Value.FavoriteItems[i].CanBeOpened.ToString());
                    values.Add(instance.Value.FavoriteItems[i].CanBeFavorite.ToString());
                    values.Add(instance.Value.FavoriteItems[i].ImageSource);
                    values.Add(instance.Value.FavoriteItems[i].FontSize.ToString());
                    values.Add(new CursorConverter().ConvertToString(instance.Value.FavoriteItems[i].CursorType));
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                XmlElement element = settingsNode.SelectSingleNode(string.Format("//add[@key='{0}']", keys[i])) as XmlElement;
                if (element != null)
                    element.SetAttribute("value", values[i]);
                else
                {
                    element = doc.CreateElement("add");
                    element.SetAttribute("key", keys[i]);
                    element.SetAttribute("value", values[i]);
                    settingsNode.AppendChild(element);
                }
            }
            doc.Save(Assembly.GetExecutingAssembly().Location + ".config");
        }

        private void LoadSettings()
        {
            NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
            if (allAppSettings.Count < 1)
                return;
            foreach (var instance in pluginsManager.PluginInstasnces)
            {
                if (allAppSettings[string.Format("{0}.{1}", instance.Key, "OpenedTabIndex")] == null)
                    continue;
                instance.Value.OpenedTabIndex = Convert.ToInt32(allAppSettings[string.Format("{0}.{1}", instance.Key, "OpenedTabIndex")]);
                string str = "{0}.FavoriteItems.{1}.{2}";
                int i = 0;

                while (true)
                {
                    if (allAppSettings[string.Format(str, instance.Key, i, "Name")] != null)
                    {
                        instance.Value.AddToFavorites(new NavigationItem(allAppSettings[string.Format(str, instance.Key, i, "Name")],
                                                                         allAppSettings[string.Format(str, instance.Key, i, "Path")],
                                                                         Convert.ToDouble(allAppSettings[string.Format(str, instance.Key, i, "Height")]),
                                                                         Convert.ToBoolean(allAppSettings[string.Format(str, instance.Key, i, "CanBeOpened")]),
                                                                         Convert.ToBoolean(allAppSettings[string.Format(str, instance.Key, i, "CanBeFavorite")]),
                                                                         allAppSettings[string.Format(str, instance.Key, i, "ImageSource")],
                                                                         Convert.ToDouble(allAppSettings[string.Format(str, instance.Key, i, "FontSize")]),
                                                                         new CursorConverter().ConvertFromString(allAppSettings[string.Format(str, instance.Key, i, "CursorType")]) as Cursor));
                    }
                    else
                        break;
                    i++;
                }
            }

            this.Left = Convert.ToDouble(allAppSettings["Window.Left"]);
            this.Top = Convert.ToDouble(allAppSettings["Window.Top"]);
            this.Width = Convert.ToDouble(allAppSettings["Window.Width"]);
            this.Height = Convert.ToDouble(allAppSettings["Window.Height"]);
            this.WindowState = (WindowState)WindowState.Parse(WindowState.GetType(), allAppSettings["Window.WindowState"]);

            if (allAppSettings["Plugins.Key"] == string.Empty && modeComboBox.Items.Count > 0)
                allAppSettings["Plugins.Key"] = (modeComboBox.Items[0] as TextBlock).Tag.ToString();
            for (int i = 0; i < modeComboBox.Items.Count; i++)
                if ((modeComboBox.Items[i] as TextBlock).Tag.ToString() == allAppSettings["Plugins.Key"])
                {
                    modeComboBox.SelectedIndex = i;
                    break;
                }
        }

        private XmlDocument LoadConfigDocument()
        {
            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.Load(Assembly.GetExecutingAssembly().Location + ".config");
                return doc;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("Конфигурационный файл не найден.", ex);
            }
        }
        #endregion
    }
}
