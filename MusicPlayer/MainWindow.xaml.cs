﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Reflection;
using System.Xml;
using System.Configuration;
using MusicPlayerAPI;
using System.Threading.Tasks;

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
        private const int numOfSongsToAddWhenScrolling = 5;
        private Song[] playlist1Songs;
        private Song[] playlist2Songs;
        private Song[] visibleSongs
        {
            get { return visibDataGrid == playlist1DataGrid ? playlist1Songs : playlist2Songs; }
            set
            {
                if (visibDataGrid == playlist1DataGrid)
                    playlist1Songs = value;
                else
                    playlist2Songs = value;
            }
        }

        #region WINDOW
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            playlist1DataGrid.Tag = playlist1Songs;
            playlist2DataGrid.Tag = playlist2Songs;
            pluginsManager.LoadPlugin(pluginsDirectory);
            SetModeComboBoxItems();
            try { LoadSettings(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка чтения конфигурационного файла", MessageBoxButton.OK, MessageBoxImage.Error); }
            if (modeComboBox.SelectedIndex == -1 && modeComboBox.Items.Count > 0)
                modeComboBox.SelectedIndex = 0;
            if (modeComboBox.Items.Count > 1)
                modeComboBox.Visibility = Visibility.Visible;
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
            SetSongsList(visibleSongs, true, true, false);
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

        private async void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            loadingProgressBar.IsIndeterminate = true;
            await ShowItems(addressTextBox.Tag?.ToString(), true);
            loadingProgressBar.IsIndeterminate = true;
            SetSongsList(await pluginsManager.GetDefaultSongsList(), true, true, true);
            loadingProgressBar.IsIndeterminate = false;
        }

        private async void NavigTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pluginsManager.OpenedTabIndex != navigTabControl.SelectedIndex)
            {
                pluginsManager.OpenedTabIndex = (pluginsManager.OpenedTabIndex == 0) ? 1 : 0;
                await ShowItems(addressTextBox.Tag?.ToString(), false);
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

        private async void OpenItem(DockPanel outerPanel)
        {
            DockPanel innerPanel = outerPanel?.Children[outerPanel.Children.Count - 1] as DockPanel;
            if (innerPanel == null)
                return;
            NavigationItem item = innerPanel.Tag as NavigationItem;
            if (item.UseAreYouSureMessageBox)
            {
                var answer = MessageBox.Show(this, item.AreYouSureMessageBoxMessage, item.Name, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (answer == MessageBoxResult.No)
                    return;
            }
            if (item != null && item.CanBeOpened)
                await ShowItems(item.Path, true);
            else if (item != null && !pluginsManager.UpdatePlaylistWhenFavoritesChanges)
                SetSongsList(await pluginsManager.GetSongsList(item), true, false, (mediaElement.Source == null) ? true : false);
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowItems(addressTextBox.Tag?.ToString(), false);
        }

        private async Task ShowItems(string path, bool isScrollToUp)
        {
            await ShowNavigationItems(path, isScrollToUp);
            ShowFavoriteItems();
        }

        private async Task ShowNavigationItems(string path, bool isScrollToUp)
        {
            loadingProgressBar.IsIndeterminate = true;
            navigListView.Items.Clear();
            List<NavigationItem> navItems = await pluginsManager.GetNavigationItems(path);
            loadingProgressBar.IsIndeterminate = false;
            addressTextBox.Tag = (navItems == null) ? addressTextBox.Tag?.ToString() : path;
            addressTextBox.Text = (addressTextBox.Tag == null) ? string.Empty : addressTextBox.Tag.ToString();
            if (navItems == null)
                navItems = await pluginsManager.GetNavigationItems(addressTextBox.Tag?.ToString());
            foreach (var ni in navItems)
                ShowNavigationItem(navigListView, ni);

            if (navigListView.Items.Count > 0 && isScrollToUp)
                navigListView.ScrollIntoView(navigListView.Items[0]);
        }

        private void ShowFavoriteItems()
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

        private void ShowNavigationItem(ListView listView, NavigationItem ni)
        {
            DockPanel outerDP = new DockPanel();
            outerDP.LastChildFill = true;
            outerDP.Background = Brushes.Transparent;
            outerDP.MouseEnter += NavigationDockPanel_MouseEnterLeave;
            outerDP.MouseLeave += NavigationDockPanel_MouseEnterLeave;
            DockPanel innerDP = new DockPanel();
            DockPanel.SetDock(innerDP, Dock.Left);
            innerDP.LastChildFill = true;
            innerDP.Background = Brushes.Transparent;
            outerDP.Tag = innerDP.Tag = ni;
            innerDP.Height = ni.Height;
            innerDP.Width = listView.MinWidth;
            innerDP.Cursor = ni.CursorType;
            try
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(ni.ImageSource, UriKind.RelativeOrAbsolute));
                img.Height = ni.Height;
                DockPanel.SetDock(img, Dock.Left);
                innerDP.Children.Add(img);
            }
            catch { }
            Label label = new Label();
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Content = ni.Name;
            label.FontSize = ni.FontSize;
            label.Foreground = ni.Foreground;
            label.Height = ni.Height;
            label.Padding = new Thickness(5, 0, 0, 0);
            innerDP.Children.Add(label);
            if (ni.CanBeFavorite)
            {
                Image imgAddDel = new Image();
                BitmapImage bmImage = new BitmapImage(new Uri(ni.AddRemoveFavoriteImageSource, UriKind.RelativeOrAbsolute));
                imgAddDel.Source = bmImage;
                imgAddDel.Visibility = Visibility.Hidden;
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

        private void NavigationDockPanel_MouseEnterLeave(object sender, MouseEventArgs e)
        {
            var outerDp = (DockPanel)sender;
            if (outerDp.Children.Count == 1 && outerDp.Children[0] as Image == null)
                return;
            var image = (Image)outerDp.Children[0];
            if (e.RoutedEvent.Name == "MouseEnter")
                image.Visibility = Visibility.Visible;
            else if (e.RoutedEvent.Name == "MouseLeave")
                image.Visibility = Visibility.Hidden;
        }

        private async void ChangeFavorites(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            NavigationItem ni = (NavigationItem)((DockPanel)((Image)sender).Parent).Tag;
            if (pluginsManager.IsFavorite(ni))
                pluginsManager.DeleteFromFavorites(ni);
            else
                pluginsManager.AddToFavorites(ni);
            await ShowItems(addressTextBox.Tag?.ToString(), false);
            if (pluginsManager.UpdatePlaylistWhenFavoritesChanges)
            {
                mediaElement.Close();
                mediaElement.Source = null;
                loadingProgressBar.IsIndeterminate = true;
                SetSongsList(await pluginsManager.GetDefaultSongsList(), true, false, (mediaElement.Source == null) ? true : false);
                loadingProgressBar.IsIndeterminate = false;
                if (visibDataGrid.Items.Count == 0)
                    ClearControls();
            }
        }
        #endregion

        #region WORKING WITH SONGS
        private void searchTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "Поиск...";
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = searchTextBox.Text == "Поиск..." && searchTextBox.Foreground == Brushes.LightGray ? string.Empty : searchTextBox.Text;
            searchTextBox.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF2B587A");
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            searchTextBox.Foreground = searchTextBox.Text == string.Empty ? Brushes.LightGray : Brushes.Black;
            searchTextBox.Text = searchTextBox.Text == string.Empty ? "Поиск..." : searchTextBox.Text;
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            randLabel.Foreground = Brushes.LightGray;
            if (sortComboBox.SelectedIndex == -1)
                return;
            if (visibDataGrid == busyDataGrid)
                busyDGSortIndex = sortComboBox.SelectedIndex;
            SetSongsList(visibleSongs, true, true, false);
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
            if (busyDataGrid.SelectedIndex == -1)
                return;
            SetMediaElementSource();
            Play();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Focus();
            var input = e as KeyEventArgs;
            if (input != null && input.Key != Key.Enter || searchTextBox.Text == string.Empty)
                return;
            List<Song> resultSongs = new List<Song>();
            if (!pluginsManager.UseDefaultSearch)
                resultSongs.AddRange(await pluginsManager.GetSearchResponse(searchTextBox.Text));
            else
                foreach (var currentSong in await pluginsManager.GetHomeButtonSongs())
                    if (currentSong.Title.ToLower().Contains(searchTextBox.Text.ToLower())
                        || currentSong.Artist.ToLower().Contains(searchTextBox.Text.ToLower()))
                        resultSongs.Add(currentSong);
            SetSongsList(resultSongs.ToArray(), pluginsManager.SortSearchResults, false, (mediaElement.Source == null) ? true : false);
        }

        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!pluginsManager.UseDefaultHomeButton)
                SetSongsList(await pluginsManager.GetHomeButtonSongs(), true, false, false);
            else
                ShowBusyPlaylist();
        }

        private void ShowBusyPlaylist()
        {
            visibDataGrid.Visibility = Visibility.Collapsed;
            busyDataGrid.Visibility = Visibility.Visible;
            if (busyDGRandMode != Brushes.White)
                sortComboBox.SelectedIndex = busyDGSortIndex;
            randLabel.Foreground = busyDGRandMode;
            numOfAudioTextBlock.Content = string.Format("Песен: {0}", visibleSongs.Length);
        }

        private void SetSongsList(Song[] list, bool sortList, bool changeCurrentPlaylist, bool moveToFirstSong)
        {
            if (!changeCurrentPlaylist)
            {
                if (songsManager.SortSongs(visibleSongs, 0)
                .SequenceEqual(songsManager.SortSongs(list, 0), new Song() as IEqualityComparer<Song>))
                    return;
                Song[] busyDataGridSongs;
                if (busyDataGrid == playlist1DataGrid)
                    busyDataGridSongs = playlist1Songs;
                else
                    busyDataGridSongs = playlist2Songs;
                if (visibDataGrid != busyDataGrid && songsManager.SortSongs(busyDataGridSongs, 0)
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
            {
                visibleSongs = new Song[0];
                visibDataGrid.Items.Clear();
            }
            else
            {
                Song currSong = new Song();
                if (visibDataGrid == busyDataGrid && visibDataGrid.SelectedIndex != -1
                    && (randLabel.Foreground == Brushes.White || sortList))
                {
                    currSong = (Song)visibDataGrid.SelectedItem;
                    int count = visibDataGrid.Items.Count;
                    int i = 0;
                    while (visibDataGrid.Items.Count != 1)
                    {
                        if (((Song)visibDataGrid.Items[i]).Path != currSong.Path)
                            visibDataGrid.Items.RemoveAt(i);
                        else
                            i++;
                    }
                    var songsList = list.ToList();
                    songsList.Remove(currSong);
                    list = songsList.ToArray();
                }
                else
                    visibDataGrid.Items.Clear();

                Song[] songs = (randLabel.Foreground == Brushes.White) ? songsManager.MixSongs(list) : (sortList) ?
                    songsManager.SortSongs(list, sortComboBox.SelectedIndex) : list;
                var newSongsList = new List<Song>();
                if (currSong.Path != null)
                    newSongsList.Add(currSong);
                newSongsList.AddRange(songs);
                visibleSongs = newSongsList.ToArray();
                for (int i = 0; i < songs.Length && i < (currSong.Path != null ? numOfSongsToAddWhenScrolling - 1 : numOfSongsToAddWhenScrolling); i++)
                    visibDataGrid.Items.Add(songs[i]);
                if (moveToFirstSong)
                    MoveToFirstSong();
            }
            numOfAudioTextBlock.Content = string.Format("Песен: {0}", visibleSongs.Length);
        }

        private void ClearControls()
        {
            playlist1Songs = playlist2Songs = new Song[0];
            playlist1DataGrid.Items.Clear();
            playlist2DataGrid.Items.Clear();
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
            if (visibDataGrid.Items.Count != 0)
            {
                visibDataGrid.SelectedIndex = 0;
                visibDataGrid.ScrollIntoView(visibDataGrid.SelectedItem);
            }
            SetMediaElementSource();
            Pause();
        }

        private void SetMediaElementSource()
        {
            if (busyDataGrid.SelectedIndex == -1)
                return;
            mediaElement.Source = new Uri(((Song)busyDataGrid.SelectedItem).Path);
        }

        private void playlistsScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sw = e.OriginalSource as ScrollViewer;
            sw.UpdateLayout();
            if (sw.ExtentHeight != 0 && sw.ExtentHeight - sw.ViewportHeight - sw.VerticalOffset <= 250)
            {
                int maxLength = visibleSongs.Length;
                int curLength = visibDataGrid.Items.Count;
                int n = (maxLength - curLength >= numOfSongsToAddWhenScrolling) ? numOfSongsToAddWhenScrolling + curLength : maxLength;
                for (int i = curLength; i < n; i++)
                    visibDataGrid.Items.Add(visibleSongs[i]);
            }
        }
        #endregion

        #region CONFIG
        private void SaveSettings()
        {
            var doc = LoadConfigDocument();
            var settingsNode = doc.SelectSingleNode("//appSettings");
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
            string pattern = "{0}.FavoriteItems.{1}.{2}";
            foreach (var pluginInstance in pluginsManager.PluginInstasnces)
            {
                keys.Add(string.Format("{0}.{1}", pluginInstance.Key, "OpenedTabIndex"));
                values.Add(pluginInstance.Value.OpenedTabIndex.ToString());
                for (int i = 0; i < pluginInstance.Value.FavoriteItems.Count; i++)
                {
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Name"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Path"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Height"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "CanBeOpened"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "CanBeFavorite"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "AddRemoveFavoriteImageSource"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "FontSize"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Foreground"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "CursorType"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "ImageSource"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "UseAreYouSureMessageBox"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "AreYouSureMessageBoxMessage"));
                    values.Add(pluginInstance.Value.FavoriteItems[i].Name);
                    values.Add(pluginInstance.Value.FavoriteItems[i].Path);
                    values.Add(pluginInstance.Value.FavoriteItems[i].Height.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].CanBeOpened.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].CanBeFavorite.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].AddRemoveFavoriteImageSource);
                    values.Add(pluginInstance.Value.FavoriteItems[i].FontSize.ToString());
                    values.Add(new BrushConverter().ConvertToString(pluginInstance.Value.FavoriteItems[i].Foreground));
                    values.Add(new CursorConverter().ConvertToString(pluginInstance.Value.FavoriteItems[i].CursorType));
                    values.Add(pluginInstance.Value.FavoriteItems[i].ImageSource);
                    values.Add(pluginInstance.Value.FavoriteItems[i].UseAreYouSureMessageBox.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].AreYouSureMessageBoxMessage);
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                var element = settingsNode.SelectSingleNode(string.Format("//add[@key='{0}']", keys[i])) as XmlElement;
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
            var allAppSettings = ConfigurationManager.AppSettings;
            if (allAppSettings.Count < 1)
                return;
            foreach (var pluginInstance in pluginsManager.PluginInstasnces)
            {
                if (allAppSettings[string.Format("{0}.{1}", pluginInstance.Key, "OpenedTabIndex")] == null)
                    continue;
                pluginInstance.Value.OpenedTabIndex = Convert.ToInt32(allAppSettings[string.Format("{0}.{1}", pluginInstance.Key, "OpenedTabIndex")]);
                string pattern = "{0}.FavoriteItems.{1}.{2}";
                int i = 0;

                while (true)
                {
                    if (allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Name")] != null)
                    {
                        pluginInstance.Value.AddToFavorites(new NavigationItem(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Name")],
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Path")],
                                                                         Convert.ToDouble(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Height")]),
                                                                         Convert.ToBoolean(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "CanBeOpened")]),
                                                                         Convert.ToBoolean(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "CanBeFavorite")]),
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "AddRemoveFavoriteImageSource")],
                                                                         Convert.ToDouble(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "FontSize")]),
                                                                         new BrushConverter().ConvertFromString(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Foreground")]) as Brush,
                                                                         new CursorConverter().ConvertFromString(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "CursorType")]) as Cursor,
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "ImageSource")],
                                                                         Convert.ToBoolean(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "UseAreYouSureMessageBox")]),
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "AreYouSureMessageBoxMessage")]));
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
